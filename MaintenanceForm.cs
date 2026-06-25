using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace LabFileCollector;

public partial class MaintenanceForm : Form
{
    private readonly AppSettings _settings;
    private readonly string _sourceUserRoot;
    private readonly string _destinationRoot;
    private readonly string _pcName;
    private readonly bool _darkTheme;
    private readonly Action<string>? _mainStatus;
    private CancellationTokenSource? _cts;

    public MaintenanceForm(AppSettings settings, string sourceUserRoot, string destinationRoot, string pcName, bool darkTheme, Action<string>? mainStatus)
    {
        _settings = settings;
        _sourceUserRoot = sourceUserRoot;
        _destinationRoot = destinationRoot;
        _pcName = pcName;
        _darkTheme = darkTheme;
        _mainStatus = mainStatus;
        InitializeComponent();
        LoadFromSettings();
        ApplyTheme();
        UpdateDynamicTexts();
    }

    private void UpdateDynamicTexts()
    {
        var folderName = Path.GetFileName(_sourceUserRoot.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));
        if (string.IsNullOrWhiteSpace(folderName))
            folderName = _sourceUserRoot;

        chkCleanUserTemp.Text = $"Καθάρισμα Temp χρήστη ({folderName})";
    }

    private void LoadFromSettings()
    {
        chkRunAfterTransfer.Checked = _settings.RunMaintenanceAfterTransfer;
        chkCleanUserTemp.Checked = _settings.CleanUserTemp;
        chkCleanWindowsTemp.Checked = _settings.CleanWindowsTemp;
        chkCleanDownloadsPlanned.Checked = _settings.CleanDownloadsPlannedFiles;
        chkEmptyRecycleBin.Checked = _settings.EmptyRecycleBin;
        chkOptimizeDrive.Checked = _settings.OptimizeSystemDrive;
        chkCleanWindowsUpdateComponents.Checked = _settings.CleanWindowsUpdateComponents;
    }

    private void SaveToSettings()
    {
        _settings.RunMaintenanceAfterTransfer = chkRunAfterTransfer.Checked;
        _settings.CleanUserTemp = chkCleanUserTemp.Checked;
        _settings.CleanWindowsTemp = chkCleanWindowsTemp.Checked;
        _settings.CleanDownloadsPlannedFiles = chkCleanDownloadsPlanned.Checked;
        _settings.EmptyRecycleBin = chkEmptyRecycleBin.Checked;
        _settings.OptimizeSystemDrive = chkOptimizeDrive.Checked;
        _settings.CleanWindowsUpdateComponents = chkCleanWindowsUpdateComponents.Checked;
        _settings.Save();
    }

    private CleanupOptions GetOptions() => new()
    {
        CleanUserTemp = chkCleanUserTemp.Checked,
        CleanWindowsTemp = chkCleanWindowsTemp.Checked,
        CleanDownloadsPlannedFiles = chkCleanDownloadsPlanned.Checked,
        EmptyRecycleBin = chkEmptyRecycleBin.Checked,
        OptimizeSystemDrive = chkOptimizeDrive.Checked,
        CleanWindowsUpdateComponents = chkCleanWindowsUpdateComponents.Checked
    };

    private void btnSave_Click(object? sender, EventArgs e)
    {
        SaveToSettings();
        _mainStatus?.Invoke("Οι επιλογές καθαρισμού αποθηκεύτηκαν.");
        MessageBox.Show("Οι επιλογές καθαρισμού αποθηκεύτηκαν.", "Καθάρισμα", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    private async void btnRunNow_Click(object? sender, EventArgs e)
    {
        SaveToSettings();
        var options = GetOptions();
        if (!MaintenanceService.HasAny(options))
        {
            MessageBox.Show("Δεν έχεις επιλέξει καμία εργασία καθαρισμού.", "Καθάρισμα", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        var confirm = MessageBox.Show(
            BuildWarning(options),
            "Επιβεβαίωση καθαρισμού",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Warning,
            MessageBoxDefaultButton.Button2);

        if (confirm != DialogResult.Yes)
            return;

        SetBusy(true);
        _cts = new CancellationTokenSource();
        progressBar.Style = ProgressBarStyle.Marquee;
        progressBar.MarqueeAnimationSpeed = 35;
        WriteLog("Ξεκίνησε καθάρισμα συστήματος.");
        _mainStatus?.Invoke("Ξεκίνησε καθάρισμα συστήματος από το popup.");

        try
        {
            // Standalone cleanup: δεν υπάρχει ενεργό transfer plan, άρα τα Downloads planned files απλώς θα παραλειφθούν αν δεν υπάρχουν successful paths.
            var result = await MaintenanceService.ExecuteAsync(
                options,
                _sourceUserRoot,
                Array.Empty<string>(),
                WriteLog,
                _cts.Token);

            var logPath = TryWriteMaintenanceLog(result);
            var message = $"Καθάρισμα τέλος. Deleted files: {result.DeletedFiles}, dirs: {result.DeletedDirectories}, failed: {result.FailedItems}, freed approx: {FileTransferService.HumanBytes(result.ReclaimedBytes)}.";
            WriteLog(message);
            if (!string.IsNullOrWhiteSpace(logPath))
                WriteLog($"Log: {logPath}");
            _mainStatus?.Invoke(message + (string.IsNullOrWhiteSpace(logPath) ? string.Empty : $" Log: {logPath}"));

            MessageBox.Show(
                $"Το καθάρισμα ολοκληρώθηκε.\n\nDeleted files: {result.DeletedFiles}\nDeleted dirs: {result.DeletedDirectories}\nFailed: {result.FailedItems}\nFreed approx: {FileTransferService.HumanBytes(result.ReclaimedBytes)}\n\nLog:\n{logPath}",
                "Καθάρισμα",
                MessageBoxButtons.OK,
                result.FailedItems == 0 ? MessageBoxIcon.Information : MessageBoxIcon.Warning);
        }
        catch (OperationCanceledException)
        {
            WriteLog("Το καθάρισμα ακυρώθηκε.");
            _mainStatus?.Invoke("Το καθάρισμα ακυρώθηκε.");
        }
        catch (Exception ex)
        {
            WriteLog("Σφάλμα: " + ex.Message);
            MessageBox.Show(ex.Message, "Σφάλμα καθαρισμού", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            progressBar.Style = ProgressBarStyle.Blocks;
            progressBar.MarqueeAnimationSpeed = 0;
            progressBar.Value = 0;
            SetBusy(false);
            _cts?.Dispose();
            _cts = null;
        }
    }

    private string? TryWriteMaintenanceLog(CleanupResult result)
    {
        try
        {
            return MaintenanceService.WriteMaintenanceLog(_destinationRoot, _pcName, result);
        }
        catch (Exception ex)
        {
            var fallbackRoot = Path.Combine(AppContext.BaseDirectory, "_logs");
            try
            {
                Directory.CreateDirectory(fallbackRoot);
                var fallbackPath = MaintenanceService.WriteMaintenanceLog(AppContext.BaseDirectory, _pcName, result);
                WriteLog($"Δεν γράφτηκε log στον προορισμό ({ex.Message}). Γράφτηκε τοπικά: {fallbackPath}");
                return fallbackPath;
            }
            catch (Exception fallbackEx)
            {
                WriteLog($"Δεν μπόρεσε να γραφτεί maintenance log: {fallbackEx.Message}");
                return null;
            }
        }
    }

    private static string BuildWarning(CleanupOptions options)
    {
        var lines = new List<string>
        {
            "Θα τρέξει καθάρισμα συστήματος τώρα.",
            "",
            "Εργασίες:"
        };

        if (options.CleanUserTemp)
            lines.Add("• Καθάρισμα Temp του επιλεγμένου προφίλ χρήστη.");
        if (options.CleanWindowsTemp)
            lines.Add("• Καθάρισμα Windows Temp. Κλειδωμένα/προστατευμένα αρχεία θα παραλειφθούν.");
        if (options.CleanDownloadsPlannedFiles)
            lines.Add("• Downloads planned files: στο standalone cleanup θα παραλειφθεί αν δεν υπάρχει ενεργό transfer plan.");
        if (options.EmptyRecycleBin)
            lines.Add("• Άδειασμα Κάδου Ανακύκλωσης.");
        if (options.OptimizeSystemDrive)
            lines.Add("• Windows Optimize στον δίσκο συστήματος: SSD=TRIM, HDD=defrag/optimize.");
        if (options.CleanWindowsUpdateComponents)
            lines.Add("• Windows Update Cleanup μέσω DISM /StartComponentCleanup. Μπορεί να πάρει αρκετή ώρα.");

        lines.Add("");
        lines.Add("Συνέχεια;");
        return string.Join(Environment.NewLine, lines);
    }

    private void btnClose_Click(object? sender, EventArgs e)
    {
        DialogResult = DialogResult.OK;
        Close();
    }

    private void SetBusy(bool busy)
    {
        chkRunAfterTransfer.Enabled = !busy;
        chkCleanUserTemp.Enabled = !busy;
        chkCleanWindowsTemp.Enabled = !busy;
        chkCleanDownloadsPlanned.Enabled = !busy;
        chkEmptyRecycleBin.Enabled = !busy;
        chkOptimizeDrive.Enabled = !busy;
        chkCleanWindowsUpdateComponents.Enabled = !busy;
        btnSave.Enabled = !busy;
        btnRunNow.Enabled = !busy;
        btnClose.Enabled = !busy;
    }

    private void WriteLog(string message)
    {
        if (InvokeRequired)
        {
            BeginInvoke(() => WriteLog(message));
            return;
        }
        txtLog.AppendText($"[{DateTime.Now:HH:mm:ss}] {message}{Environment.NewLine}");
    }

    private void ApplyTheme()
    {
        if (!_darkTheme)
            return;

        var back = Color.FromArgb(24, 28, 31);
        var card = Color.FromArgb(31, 37, 41);
        var input = Color.FromArgb(18, 22, 25);
        var text = Color.FromArgb(235, 240, 245);
        var muted = Color.FromArgb(165, 174, 183);

        BackColor = back;
        ForeColor = text;
        rootLayout.BackColor = back;
        lblIntro.ForeColor = text;
        grpOptions.BackColor = card;
        grpOptions.ForeColor = text;
        if (grpOptions is ModernGroupBox modernGroupBox)
            modernGroupBox.BorderColor = Color.FromArgb(74, 90, 100);
        optionsLayout.BackColor = card;
        lblNote.ForeColor = muted;
        txtLog.BackColor = input;
        txtLog.ForeColor = text;
        foreach (Control c in optionsLayout.Controls)
        {
            c.BackColor = card;
            c.ForeColor = c == lblNote ? muted : text;
        }
    }
}
