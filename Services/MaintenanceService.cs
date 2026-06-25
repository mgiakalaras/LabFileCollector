using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;

namespace LabFileCollector;

public sealed class CleanupOptions
{
    public bool CleanWindowsTemp { get; set; }
    public bool CleanUserTemp { get; set; }
    public bool CleanDownloadsPlannedFiles { get; set; }
    public bool EmptyRecycleBin { get; set; }
    public bool OptimizeSystemDrive { get; set; }
    public bool CleanWindowsUpdateComponents { get; set; }
}

public sealed class CleanupResult
{
    public int DeletedFiles { get; set; }
    public int DeletedDirectories { get; set; }
    public int FailedItems { get; set; }
    public long ReclaimedBytes { get; set; }
    public bool OptimizeStarted { get; set; }
    public int? OptimizeExitCode { get; set; }
    public bool WindowsUpdateCleanupStarted { get; set; }
    public int? WindowsUpdateCleanupExitCode { get; set; }
    public List<string> LogRows { get; } = new();
}

public static class MaintenanceService
{
    private const uint SHERB_NOCONFIRMATION = 0x00000001;
    private const uint SHERB_NOPROGRESSUI = 0x00000002;
    private const uint SHERB_NOSOUND = 0x00000004;

    [DllImport("Shell32.dll", CharSet = CharSet.Unicode)]
    private static extern int SHEmptyRecycleBin(IntPtr hwnd, string? pszRootPath, uint dwFlags);

    public static async Task<CleanupResult> ExecuteAsync(
        CleanupOptions options,
        string sourceUserRoot,
        IEnumerable<string> successfulSourcePaths,
        Action<string>? status,
        CancellationToken cancellationToken)
    {
        var result = new CleanupResult();
        var normalizedSourceUserRoot = NormalizeSourceUserRoot(sourceUserRoot);

        if (options.CleanUserTemp)
        {
            var userTemp = Path.Combine(normalizedSourceUserRoot, "AppData", "Local", "Temp");
            CleanDirectoryContents(userTemp, "USER_TEMP", result, status, cancellationToken);
        }

        if (options.CleanWindowsTemp)
        {
            var windowsTemp = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "Temp");
            CleanDirectoryContents(windowsTemp, "WINDOWS_TEMP", result, status, cancellationToken);
        }

        if (options.CleanDownloadsPlannedFiles)
        {
            var downloadsRoot = Path.Combine(normalizedSourceUserRoot, "Downloads");
            CleanSuccessfulFilesFromDownloads(downloadsRoot, successfulSourcePaths, result, status, cancellationToken);
        }

        if (options.EmptyRecycleBin)
        {
            EmptyRecycleBin(result, status);
        }

        if (options.OptimizeSystemDrive)
        {
            await OptimizeDriveAsync(normalizedSourceUserRoot, result, status, cancellationToken);
        }

        if (options.CleanWindowsUpdateComponents)
        {
            await CleanWindowsUpdateComponentsAsync(result, status, cancellationToken);
        }

        return result;
    }

    public static bool HasAny(CleanupOptions options)
    {
        return options.CleanWindowsTemp || options.CleanUserTemp || options.CleanDownloadsPlannedFiles || options.EmptyRecycleBin || options.OptimizeSystemDrive || options.CleanWindowsUpdateComponents;
    }

    public static string WriteMaintenanceLog(string destinationRoot, string pcName, CleanupResult result)
    {
        var logsRoot = Path.Combine(destinationRoot, "_logs");
        Directory.CreateDirectory(logsRoot);

        var fileName = $"maintenance-log-{pcName}-{DateTime.Now:yyyyMMdd-HHmmss}.csv";
        var path = Path.Combine(logsRoot, fileName);

        var sb = new StringBuilder();
        sb.AppendLine("DateTime;Area;Path;Action;StatusOrError");
        foreach (var row in result.LogRows)
            sb.AppendLine(row);

        File.WriteAllText(path, sb.ToString(), new UTF8Encoding(encoderShouldEmitUTF8Identifier: true));
        return path;
    }

    private static void CleanDirectoryContents(string directoryPath, string area, CleanupResult result, Action<string>? status, CancellationToken cancellationToken)
    {
        if (!Directory.Exists(directoryPath))
        {
            status?.Invoke($"Maintenance: δεν βρέθηκε {directoryPath}");
            result.LogRows.Add(LogRow(area, directoryPath, "SKIPPED", "Directory not found"));
            return;
        }

        status?.Invoke($"Maintenance: καθάρισμα {directoryPath}");

        foreach (var file in SafeEnumerateFiles(directoryPath, result, area, cancellationToken))
            DeleteFile(file, area, result, status, cancellationToken);

        foreach (var dir in SafeEnumerateDirectoriesBottomUp(directoryPath, result, area, cancellationToken))
            DeleteDirectoryIfEmpty(dir, area, result, status, cancellationToken);
    }

    private static void CleanSuccessfulFilesFromDownloads(
        string downloadsRoot,
        IEnumerable<string> successfulSourcePaths,
        CleanupResult result,
        Action<string>? status,
        CancellationToken cancellationToken)
    {
        if (!Directory.Exists(downloadsRoot))
        {
            status?.Invoke($"Maintenance: δεν βρέθηκε Downloads path {downloadsRoot}");
            result.LogRows.Add(LogRow("DOWNLOADS", downloadsRoot, "SKIPPED", "Directory not found"));
            return;
        }

        var files = successfulSourcePaths
            .Where(path => IsPathInside(path, downloadsRoot))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        if (files.Count == 0)
        {
            status?.Invoke("Maintenance: δεν υπάρχουν επιτυχώς αντιγραμμένα αρχεία από Downloads για καθάρισμα.");
            result.LogRows.Add(LogRow("DOWNLOADS", downloadsRoot, "SKIPPED", "No successful planned Downloads files"));
            return;
        }

        status?.Invoke($"Maintenance: καθάρισμα {files.Count} επιτυχώς αντιγραμμένων αρχείων από Downloads.");
        foreach (var file in files)
            DeleteFile(file, "DOWNLOADS", result, status, cancellationToken);

        foreach (var dir in SafeEnumerateDirectoriesBottomUp(downloadsRoot, result, "DOWNLOADS", cancellationToken))
            DeleteDirectoryIfEmpty(dir, "DOWNLOADS", result, status, cancellationToken);
    }

    private static IEnumerable<string> SafeEnumerateFiles(string root, CleanupResult result, string area, CancellationToken cancellationToken)
    {
        var pending = new Stack<string>();
        pending.Push(root);

        while (pending.Count > 0)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var current = pending.Pop();

            IEnumerable<string> files;
            try
            {
                files = Directory.EnumerateFiles(current);
            }
            catch (Exception ex)
            {
                result.FailedItems++;
                result.LogRows.Add(LogRow(area, current, "ENUM_FILES_FAILED", ex.Message));
                continue;
            }

            foreach (var file in files)
                yield return file;

            IEnumerable<string> dirs;
            try
            {
                dirs = Directory.EnumerateDirectories(current);
            }
            catch (Exception ex)
            {
                result.FailedItems++;
                result.LogRows.Add(LogRow(area, current, "ENUM_DIRS_FAILED", ex.Message));
                continue;
            }

            foreach (var dir in dirs)
            {
                try
                {
                    var attributes = File.GetAttributes(dir);
                    if ((attributes & FileAttributes.ReparsePoint) == FileAttributes.ReparsePoint)
                    {
                        result.LogRows.Add(LogRow(area, dir, "SKIPPED", "Reparse point"));
                        continue;
                    }
                }
                catch
                {
                    // Αν δεν μπορώ να διαβάσω attributes, θα το αφήσω για το enumeration error παρακάτω.
                }

                pending.Push(dir);
            }
        }
    }

    private static IEnumerable<string> SafeEnumerateDirectoriesBottomUp(string root, CleanupResult result, string area, CancellationToken cancellationToken)
    {
        var dirs = new List<string>();
        var pending = new Stack<string>();
        pending.Push(root);

        while (pending.Count > 0)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var current = pending.Pop();

            IEnumerable<string> children;
            try
            {
                children = Directory.EnumerateDirectories(current);
            }
            catch (Exception ex)
            {
                result.FailedItems++;
                result.LogRows.Add(LogRow(area, current, "ENUM_DIRS_FAILED", ex.Message));
                continue;
            }

            foreach (var child in children)
            {
                try
                {
                    var attributes = File.GetAttributes(child);
                    if ((attributes & FileAttributes.ReparsePoint) == FileAttributes.ReparsePoint)
                    {
                        result.LogRows.Add(LogRow(area, child, "SKIPPED", "Reparse point"));
                        continue;
                    }
                }
                catch
                {
                    // Συνέχισε και άσε το Delete/Enumerate να αποτύχει αν πρέπει.
                }

                dirs.Add(child);
                pending.Push(child);
            }
        }

        dirs.Reverse();
        return dirs;
    }

    private static void DeleteFile(string filePath, string area, CleanupResult result, Action<string>? status, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        try
        {
            if (!File.Exists(filePath))
            {
                result.LogRows.Add(LogRow(area, filePath, "SKIPPED", "File not found"));
                return;
            }

            long size = 0;
            try { size = new FileInfo(filePath).Length; } catch { /* ignore */ }

            File.SetAttributes(filePath, FileAttributes.Normal);
            File.Delete(filePath);
            result.DeletedFiles++;
            result.ReclaimedBytes += size;
            result.LogRows.Add(LogRow(area, filePath, "DELETE_FILE", "OK"));
        }
        catch (Exception ex)
        {
            result.FailedItems++;
            result.LogRows.Add(LogRow(area, filePath, "DELETE_FILE_FAILED", ex.Message));
        }
    }

    private static void DeleteDirectoryIfEmpty(string dirPath, string area, CleanupResult result, Action<string>? status, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        try
        {
            if (!Directory.Exists(dirPath)) return;
            if (Directory.EnumerateFileSystemEntries(dirPath).Any()) return;

            Directory.Delete(dirPath, recursive: false);
            result.DeletedDirectories++;
            result.LogRows.Add(LogRow(area, dirPath, "DELETE_EMPTY_DIR", "OK"));
        }
        catch (Exception ex)
        {
            result.FailedItems++;
            result.LogRows.Add(LogRow(area, dirPath, "DELETE_EMPTY_DIR_FAILED", ex.Message));
        }
    }

    private static void EmptyRecycleBin(CleanupResult result, Action<string>? status)
    {
        try
        {
            status?.Invoke("Maintenance: άδειασμα κάδου ανακύκλωσης.");
            var flags = SHERB_NOCONFIRMATION | SHERB_NOPROGRESSUI | SHERB_NOSOUND;
            var hr = SHEmptyRecycleBin(IntPtr.Zero, null, flags);
            if (hr == 0)
            {
                result.LogRows.Add(LogRow("RECYCLE_BIN", "ALL_DRIVES", "EMPTY_RECYCLE_BIN", "OK"));
            }
            else
            {
                result.FailedItems++;
                result.LogRows.Add(LogRow("RECYCLE_BIN", "ALL_DRIVES", "EMPTY_RECYCLE_BIN_FAILED", $"HRESULT {hr}"));
            }
        }
        catch (Exception ex)
        {
            result.FailedItems++;
            result.LogRows.Add(LogRow("RECYCLE_BIN", "ALL_DRIVES", "EMPTY_RECYCLE_BIN_FAILED", ex.Message));
        }
    }

    private static async Task OptimizeDriveAsync(string sourceUserRoot, CleanupResult result, Action<string>? status, CancellationToken cancellationToken)
    {
        var root = Path.GetPathRoot(sourceUserRoot);
        if (string.IsNullOrWhiteSpace(root))
            root = Path.GetPathRoot(Environment.GetFolderPath(Environment.SpecialFolder.Windows));

        var drive = (root ?? "C:\\").TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
        if (string.IsNullOrWhiteSpace(drive))
            drive = "C:";

        try
        {
            status?.Invoke($"Maintenance: Windows Optimize στο {drive}. Σε SSD κάνει TRIM, σε HDD defrag/optimize.");
            result.OptimizeStarted = true;

            var psi = new ProcessStartInfo
            {
                FileName = "defrag.exe",
                Arguments = $"{drive} /O /U /V",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };

            using var process = Process.Start(psi);
            if (process is null)
            {
                result.FailedItems++;
                result.LogRows.Add(LogRow("OPTIMIZE", drive, "OPTIMIZE_FAILED", "Could not start defrag.exe"));
                return;
            }

            var outputTask = process.StandardOutput.ReadToEndAsync(cancellationToken);
            var errorTask = process.StandardError.ReadToEndAsync(cancellationToken);
            await process.WaitForExitAsync(cancellationToken);

            var output = await outputTask;
            var error = await errorTask;

            result.OptimizeExitCode = process.ExitCode;
            result.LogRows.Add(LogRow("OPTIMIZE", drive, "DEFRAG_EXE_EXIT_CODE", process.ExitCode.ToString()));

            if (!string.IsNullOrWhiteSpace(output))
                result.LogRows.Add(LogRow("OPTIMIZE", drive, "STDOUT", Compact(output)));
            if (!string.IsNullOrWhiteSpace(error))
                result.LogRows.Add(LogRow("OPTIMIZE", drive, "STDERR", Compact(error)));

            if (process.ExitCode != 0)
                result.FailedItems++;
        }
        catch (Exception ex)
        {
            result.FailedItems++;
            result.LogRows.Add(LogRow("OPTIMIZE", drive, "OPTIMIZE_FAILED", ex.Message));
        }
    }


    private static async Task CleanWindowsUpdateComponentsAsync(CleanupResult result, Action<string>? status, CancellationToken cancellationToken)
    {
        const string area = "WINDOWS_UPDATE_CLEANUP";

        if (!IsRunningAsAdministrator())
        {
            result.FailedItems++;
            result.LogRows.Add(LogRow(area, "DISM", "SKIPPED", "Requires Administrator rights"));
            status?.Invoke("Maintenance: Windows Update cleanup παραλείφθηκε. Χρειάζεται Run as administrator.");
            return;
        }

        try
        {
            status?.Invoke("Maintenance: Windows Update cleanup μέσω DISM /StartComponentCleanup. Μπορεί να πάρει αρκετά λεπτά.");
            result.WindowsUpdateCleanupStarted = true;

            var psi = new ProcessStartInfo
            {
                FileName = "dism.exe",
                Arguments = "/Online /Cleanup-Image /StartComponentCleanup",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };

            using var process = Process.Start(psi);
            if (process is null)
            {
                result.FailedItems++;
                result.LogRows.Add(LogRow(area, "DISM", "DISM_FAILED", "Could not start dism.exe"));
                return;
            }

            var outputTask = process.StandardOutput.ReadToEndAsync(cancellationToken);
            var errorTask = process.StandardError.ReadToEndAsync(cancellationToken);
            await process.WaitForExitAsync(cancellationToken);

            var output = await outputTask;
            var error = await errorTask;

            result.WindowsUpdateCleanupExitCode = process.ExitCode;
            result.LogRows.Add(LogRow(area, "DISM", "DISM_EXIT_CODE", process.ExitCode.ToString()));

            if (!string.IsNullOrWhiteSpace(output))
                result.LogRows.Add(LogRow(area, "DISM", "STDOUT", Compact(output)));
            if (!string.IsNullOrWhiteSpace(error))
                result.LogRows.Add(LogRow(area, "DISM", "STDERR", Compact(error)));

            if (process.ExitCode != 0)
                result.FailedItems++;
        }
        catch (Exception ex)
        {
            result.FailedItems++;
            result.LogRows.Add(LogRow(area, "DISM", "DISM_FAILED", ex.Message));
        }
    }

    private static bool IsRunningAsAdministrator()
    {
        try
        {
            using var identity = WindowsIdentity.GetCurrent();
            var principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }
        catch
        {
            return false;
        }
    }

    private static bool IsPathInside(string path, string root)
    {
        try
        {
            var fullRoot = Path.GetFullPath(root).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar) + Path.DirectorySeparatorChar;
            var fullPath = Path.GetFullPath(path);
            return fullPath.StartsWith(fullRoot, StringComparison.OrdinalIgnoreCase);
        }
        catch
        {
            return false;
        }
    }

    private static string NormalizeSourceUserRoot(string value)
    {
        value = string.IsNullOrWhiteSpace(value) ? AppSettings.DefaultSourceUserRoot : Environment.ExpandEnvironmentVariables(value.Trim());
        return value.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
    }

    private static string Compact(string text)
    {
        return text.Replace("\r", " ").Replace("\n", " ").Trim();
    }

    private static string CsvEscape(string value)
    {
        value = value.Replace("\r", " ").Replace("\n", " ");
        if (value.Contains(';') || value.Contains('"'))
            value = '"' + value.Replace("\"", "\"\"") + '"';
        return value;
    }

    private static string LogRow(string area, string path, string action, string statusOrError)
    {
        return string.Join(';',
            CsvEscape(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")),
            CsvEscape(area),
            CsvEscape(path),
            CsvEscape(action),
            CsvEscape(statusOrError));
    }
}
