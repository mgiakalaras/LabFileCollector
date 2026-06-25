using Microsoft.Win32;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace LabFileCollector;

public sealed record SystemInfoSnapshot(
    string ComputerType,
    string ComputerName,
    string OperatingSystem,
    string LoggedInUser,
    string ProcessorName,
    double? CpuUsagePercent,
    ulong TotalMemoryBytes,
    ulong AvailableMemoryBytes,
    IReadOnlyList<DriveInfoSnapshot> Drives,
    IReadOnlyList<NetworkInfoSnapshot> NetworkAdapters);

public sealed record DriveInfoSnapshot(string Name, string Label, string Format, long TotalBytes, long AvailableBytes);

public sealed record NetworkInfoSnapshot(string Name, string Description, string IpAddress, long SpeedBitsPerSecond, string Status);

public sealed class CpuUsageSampler
{
    private ulong? _previousIdle;
    private ulong? _previousKernel;
    private ulong? _previousUser;

    public double? GetUsagePercent()
    {
        try
        {
            if (!GetSystemTimes(out var idleTime, out var kernelTime, out var userTime))
                return null;

            var idle = ToUInt64(idleTime);
            var kernel = ToUInt64(kernelTime);
            var user = ToUInt64(userTime);

            if (_previousIdle is null || _previousKernel is null || _previousUser is null)
            {
                _previousIdle = idle;
                _previousKernel = kernel;
                _previousUser = user;
                return null;
            }

            var idleDelta = idle - _previousIdle.Value;
            var kernelDelta = kernel - _previousKernel.Value;
            var userDelta = user - _previousUser.Value;
            var total = kernelDelta + userDelta;

            _previousIdle = idle;
            _previousKernel = kernel;
            _previousUser = user;

            if (total == 0)
                return null;

            var usage = 100.0 * (1.0 - ((double)idleDelta / total));
            return Math.Clamp(usage, 0, 100);
        }
        catch
        {
            return null;
        }
    }

    private static ulong ToUInt64(FileTime fileTime)
    {
        return ((ulong)fileTime.HighDateTime << 32) | fileTime.LowDateTime;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct FileTime
    {
        public uint LowDateTime;
        public uint HighDateTime;
    }

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool GetSystemTimes(out FileTime idleTime, out FileTime kernelTime, out FileTime userTime);
}

public static class SystemInfoService
{
    public static SystemInfoSnapshot GetSnapshot(CpuUsageSampler cpuSampler)
    {
        var (totalMemory, availableMemory) = GetMemoryStatus();

        return new SystemInfoSnapshot(
            GetComputerTypeText(),
            Environment.MachineName,
            GetOperatingSystemText(),
            GetLoggedInUserText(),
            GetProcessorName(),
            cpuSampler.GetUsagePercent(),
            totalMemory,
            availableMemory,
            GetDrives(),
            GetNetworkAdapters());
    }

    public static string HumanBytes(ulong bytes) => FileTransferService.HumanBytes(bytes > long.MaxValue ? long.MaxValue : (long)bytes);
    public static string HumanBytes(long bytes) => FileTransferService.HumanBytes(bytes);

    private static string GetComputerTypeText()
    {
        try
        {
            var hasBattery = SystemInformation.PowerStatus.BatteryChargeStatus != BatteryChargeStatus.NoSystemBattery;
            var type = hasBattery ? "Laptop / tablet" : "Desktop / σταθερός Η/Υ";
            return $"{type} ({RuntimeInformation.OSArchitecture})";
        }
        catch
        {
            return $"Windows PC ({RuntimeInformation.OSArchitecture})";
        }
    }


    private static string GetOperatingSystemText()
    {
        try
        {
            using var key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion");
            var productName = key?.GetValue("ProductName")?.ToString();
            var displayVersion = key?.GetValue("DisplayVersion")?.ToString();
            var releaseId = key?.GetValue("ReleaseId")?.ToString();
            var build = key?.GetValue("CurrentBuildNumber")?.ToString();

            var version = !string.IsNullOrWhiteSpace(displayVersion) ? displayVersion : releaseId;
            var parts = new List<string>();
            if (!string.IsNullOrWhiteSpace(productName)) parts.Add(productName.Trim());
            if (!string.IsNullOrWhiteSpace(version)) parts.Add(version.Trim());
            if (!string.IsNullOrWhiteSpace(build)) parts.Add($"build {build.Trim()}");

            if (parts.Count > 0)
                return string.Join(" • ", parts);
        }
        catch
        {
            // ignore and fall back
        }

        return RuntimeInformation.OSDescription.Trim();
    }

    private static string GetLoggedInUserText()
    {
        try
        {
            var domain = Environment.UserDomainName;
            var user = Environment.UserName;
            if (!string.IsNullOrWhiteSpace(domain) && !string.Equals(domain, Environment.MachineName, StringComparison.OrdinalIgnoreCase))
                return $@"{domain}\{user}";
            return user;
        }
        catch
        {
            return "Άγνωστος χρήστης";
        }
    }

    private static string GetProcessorName()
    {
        try
        {
            using var key = Registry.LocalMachine.OpenSubKey(@"HARDWARE\DESCRIPTION\System\CentralProcessor\0");
            var value = key?.GetValue("ProcessorNameString")?.ToString();
            if (!string.IsNullOrWhiteSpace(value))
                return value.Trim();
        }
        catch
        {
            // ignore and fall back
        }

        return Environment.GetEnvironmentVariable("PROCESSOR_IDENTIFIER") ?? "Άγνωστος επεξεργαστής";
    }

    private static (ulong Total, ulong Available) GetMemoryStatus()
    {
        try
        {
            var status = new MemoryStatusEx();
            status.dwLength = (uint)Marshal.SizeOf<MemoryStatusEx>();
            if (GlobalMemoryStatusEx(ref status))
                return (status.ullTotalPhys, status.ullAvailPhys);
        }
        catch
        {
            // ignore and fall back
        }

        return (0, 0);
    }

    private static IReadOnlyList<DriveInfoSnapshot> GetDrives()
    {
        var list = new List<DriveInfoSnapshot>();

        try
        {
            foreach (var drive in DriveInfo.GetDrives())
            {
                try
                {
                    // Avoid network drives here; disconnected mapped drives can delay startup badly.
                    if (drive.DriveType == DriveType.Network || !drive.IsReady)
                        continue;

                    list.Add(new DriveInfoSnapshot(
                        drive.Name,
                        drive.VolumeLabel,
                        drive.DriveFormat,
                        drive.TotalSize,
                        drive.AvailableFreeSpace));
                }
                catch
                {
                    // Skip individual drives that cannot be read.
                }
            }
        }
        catch
        {
            // ignore unavailable drive info
        }

        return list;
    }

    private static IReadOnlyList<NetworkInfoSnapshot> GetNetworkAdapters()
    {
        var list = new List<NetworkInfoSnapshot>();
        try
        {
            var adapters = NetworkInterface.GetAllNetworkInterfaces()
                .Where(x => x.NetworkInterfaceType != NetworkInterfaceType.Loopback && x.OperationalStatus == OperationalStatus.Up)
                .OrderByDescending(x => x.NetworkInterfaceType == NetworkInterfaceType.Ethernet)
                .ThenByDescending(x => x.Speed)
                .Take(4);

            foreach (var adapter in adapters)
            {
                var ip = adapter.GetIPProperties().UnicastAddresses
                    .FirstOrDefault(a => a.Address.AddressFamily == AddressFamily.InterNetwork)?.Address.ToString() ?? "χωρίς IPv4";

                list.Add(new NetworkInfoSnapshot(
                    adapter.Name,
                    adapter.Description,
                    ip,
                    adapter.Speed,
                    adapter.OperationalStatus.ToString()));
            }
        }
        catch
        {
            // ignore unavailable network info
        }

        return list;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    private struct MemoryStatusEx
    {
        public uint dwLength;
        public uint dwMemoryLoad;
        public ulong ullTotalPhys;
        public ulong ullAvailPhys;
        public ulong ullTotalPageFile;
        public ulong ullAvailPageFile;
        public ulong ullTotalVirtual;
        public ulong ullAvailVirtual;
        public ulong ullAvailExtendedVirtual;
    }

    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern bool GlobalMemoryStatusEx(ref MemoryStatusEx lpBuffer);
}
