using System.Diagnostics;
using System.Security.Principal;
using System.Windows.Forms;

namespace LabFileCollector;

internal static class Program
{
    [STAThread]
    private static void Main()
    {
        ApplicationConfiguration.Initialize();

        if (!IsRunningAsAdministrator())
        {
            try
            {
                var exePath = Environment.ProcessPath ?? Application.ExecutablePath;
                Process.Start(new ProcessStartInfo
                {
                    FileName = exePath,
                    UseShellExecute = true,
                    Verb = "runas"
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Η εφαρμογή χρειάζεται δικαιώματα διαχειριστή για πλήρες cleanup/maintenance.\n\n" + ex.Message,
                    "Απαιτούνται δικαιώματα διαχειριστή",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
            return;
        }

        Application.Run(new MainForm());
    }

    private static bool IsRunningAsAdministrator()
    {
        using var identity = WindowsIdentity.GetCurrent();
        var principal = new WindowsPrincipal(identity);
        return principal.IsInRole(WindowsBuiltInRole.Administrator);
    }
}
