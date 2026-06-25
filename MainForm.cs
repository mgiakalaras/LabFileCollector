using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using System.Runtime.InteropServices;
using Microsoft.Win32;

namespace LabFileCollector;

public partial class MainForm : Form
{
    private const string ThemeSystem = "Σύστημα / Windows";
    private const string ThemeLight = "Φωτεινό / Aero";
    private const string ThemeDark = "Σκούρο / Aero";
    private const string ThemeNotepadPlusPlus = "Notepad++ Dark";
    private const string ThemeNord = "Nord Dark";
    private const string ThemeDracula = "Dracula";

    private readonly AppSettings _settings = AppSettings.Load();
    private List<PlannedFile> _currentPlan = new();
    private CancellationTokenSource? _cts;
    private string? _lastLogPath;
    private TransferResult? _lastTransferResult;
    private string? _lastUndoLogPath;
    private bool _isLoadingUi;
    private bool _previewDrawingHooked;
    private UiPalette _palette = UiPalette.Light;
    private TableLayoutPanel? _outerLayout;
    private Panel? _systemSidebar;
    private FlowLayoutPanel? _systemInfoFlow;
    private Label? lblSysType;
    private Label? lblSysName;
    private Label? lblSysOs;
    private Label? lblSysUser;
    private MetricCard? cardSysCpu;
    private MetricCard? cardSysRam;
    private FlowLayoutPanel? _diskCardsFlow;
    private FlowLayoutPanel? _networkCardsFlow;
    private Label? lblSysUpdated;
    private PictureBox? _nyxLogoBox;
    private readonly Dictionary<string, MetricCard> _driveCardsByKey = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, MetricCard> _networkCardsByKey = new(StringComparer.OrdinalIgnoreCase);
    private readonly System.Windows.Forms.Timer _systemInfoTimer = new() { Interval = 1500 };
    private readonly CpuUsageSampler _cpuSampler = new();
    private bool _isUpdatingSystemInfo;

    public MainForm()
    {
        InitializeComponent();
        CreateSystemInfoSidebar();
        HookPreviewDrawing();
        lvPreview.Resize += (_, _) => ConfigurePreviewColumnsForScrolling();
        lvPreview.ItemChecked += lvPreview_ItemChecked;
        clbSources.Resize += (_, _) => UpdateSourcesHorizontalExtent();
        LoadApplicationVisuals();
        LoadSettingsToUi();
        AutoDetectPcOnStartup();
        UpdateFinalPathLabel();
        UpdateExtensionsEnabled();
        ApplySelectedTheme();
        UpdateSummary();
        ConfigurePreviewColumnsForScrolling();
        UpdateSourcesHorizontalExtent();

        // Do not read system information synchronously in the constructor.
        // On some lab PCs, disk/network enumeration can take a while and the form
        // appears to never open. Start it after the window is visible instead.
        Shown += (_, _) => StartSystemInfoTimer();
    }

    private void LoadApplicationVisuals()
    {
        var appIcon = AppVisuals.LoadAppIcon();
        if (appIcon is not null)
            Icon = appIcon;

        var logo = AppVisuals.LoadLogoImage();
        if (logo is not null)
            picLogo.Image = logo;

        var nyxLogo = AppVisuals.LoadNyxSystemsLogoImage();
        if (nyxLogo is not null && _nyxLogoBox is not null)
            _nyxLogoBox.Image = nyxLogo;
    }



    private void CreateSystemInfoSidebar()
    {
        Controls.Remove(rootLayout);

        _outerLayout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 1,
            Padding = new Padding(0),
            Margin = new Padding(0)
        };
        _outerLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        _outerLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 330F));
        _outerLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

        _systemSidebar = new Panel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(12),
            Margin = new Padding(0)
        };

        _systemInfoFlow = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.TopDown,
            WrapContents = false,
            AutoScroll = true,
            Padding = new Padding(0),
            Margin = new Padding(0)
        };

        var sidebarLayout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 2,
            Margin = new Padding(0),
            Padding = new Padding(0),
            Tag = "sidebar-layout"
        };
        sidebarLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        sidebarLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        sidebarLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));

        _nyxLogoBox = new PictureBox
        {
            Dock = DockStyle.Fill,
            Height = 74,
            SizeMode = PictureBoxSizeMode.Zoom,
            Margin = new Padding(0, 10, 0, 0),
            Padding = new Padding(4),
            Tag = "nyx-logo"
        };

        sidebarLayout.Controls.Add(_systemInfoFlow, 0, 0);
        sidebarLayout.Controls.Add(_nyxLogoBox, 0, 1);
        _systemSidebar.Controls.Add(sidebarLayout);
        _outerLayout.Controls.Add(rootLayout, 0, 0);
        _outerLayout.Controls.Add(_systemSidebar, 1, 0);
        Controls.Add(_outerLayout);

        AddSystemInfoHeader("Σύστημα");
        lblSysType = AddSystemInfoValue("Τύπος: —", 44);
        lblSysName = AddSystemInfoValue("Όνομα PC: —", 34);
        lblSysOs = AddSystemInfoValue("Λειτουργικό: —", 56);
        lblSysUser = AddSystemInfoValue("Συνδεδεμένος χρήστης: —", 34);

        AddSystemInfoHeader("Live γραφήματα");
        cardSysCpu = AddMetricCard("CPU", "—", "υπολογισμός...", null);
        cardSysRam = AddMetricCard("RAM", "—", "—", null);

        AddSystemInfoHeader("Δίσκοι");
        _diskCardsFlow = AddCardsFlow();

        AddSystemInfoHeader("Δίκτυο LAN / Wi‑Fi");
        _networkCardsFlow = AddCardsFlow();

        lblSysUpdated = AddSystemInfoValue("Live παρακολούθηση ενεργή", 34);
    }

    private void AddSystemInfoHeader(string text)
    {
        if (_systemInfoFlow is null) return;
        var label = new Label
        {
            Text = text,
            AutoSize = false,
            Width = 270,
            Height = 34,
            Font = new Font("Segoe UI", 10.5F, FontStyle.Bold),
            Padding = new Padding(0, 10, 0, 0),
            Margin = new Padding(0, 8, 0, 0),
            Tag = "system-header"
        };
        _systemInfoFlow.Controls.Add(label);
    }

    private Label AddSystemInfoValue(string text, int height = 48)
    {
        var label = new Label
        {
            Text = text,
            AutoSize = false,
            Width = 292,
            Height = height,
            Font = new Font("Segoe UI", 9F),
            Padding = new Padding(8, 6, 8, 6),
            Margin = new Padding(0, 4, 0, 0),
            Tag = "system-value"
        };
        _systemInfoFlow?.Controls.Add(label);
        return label;
    }

    private MetricCard AddMetricCard(string title, string primaryText, string secondaryText, double? percent)
    {
        var card = new MetricCard
        {
            Width = 292,
            Title = title,
            PrimaryText = primaryText,
            SecondaryText = secondaryText,
            Percent = percent,
            Tag = "system-metric"
        };
        _systemInfoFlow?.Controls.Add(card);
        return card;
    }

    private FlowLayoutPanel AddCardsFlow()
    {
        var flow = new FlowLayoutPanel
        {
            Width = 292,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            FlowDirection = FlowDirection.TopDown,
            WrapContents = false,
            Margin = new Padding(0, 0, 0, 0),
            Padding = new Padding(0),
            Tag = "system-flow"
        };
        _systemInfoFlow?.Controls.Add(flow);
        return flow;
    }

    private void StartSystemInfoTimer()
    {
        _systemInfoTimer.Tick += (_, _) => BeginUpdateSystemInfoSidebar();
        _systemInfoTimer.Start();
        BeginUpdateSystemInfoSidebar();
    }

    private void BeginUpdateSystemInfoSidebar()
    {
        if (_isUpdatingSystemInfo || lblSysType is null || IsDisposed)
            return;

        _isUpdatingSystemInfo = true;
        _ = Task.Run(() =>
        {
            try
            {
                return (Snapshot: SystemInfoService.GetSnapshot(_cpuSampler), Error: (Exception?)null);
            }
            catch (Exception ex)
            {
                return (Snapshot: (SystemInfoSnapshot?)null, Error: ex);
            }
        }).ContinueWith(task =>
        {
            try
            {
                if (IsDisposed || !IsHandleCreated)
                    return;

                BeginInvoke(() =>
                {
                    try
                    {
                        if (task.Result.Error is not null || task.Result.Snapshot is null)
                        {
                            if (lblSysUpdated is not null)
                                lblSysUpdated.Text = "Σφάλμα ανάγνωσης συστήματος";
                            return;
                        }

                        ApplySystemInfoSnapshot(task.Result.Snapshot);
                    }
                    finally
                    {
                        _isUpdatingSystemInfo = false;
                    }
                });
            }
            catch
            {
                _isUpdatingSystemInfo = false;
            }
        });
    }

    private void ApplySystemInfoSnapshot(SystemInfoSnapshot snapshot)
    {
        if (lblSysType is null)
            return;

        lblSysType.Text = $"Τύπος: {snapshot.ComputerType}";
        lblSysName!.Text = $"Όνομα PC: {snapshot.ComputerName}";
        lblSysOs!.Text = $"Λειτουργικό: {Shorten(snapshot.OperatingSystem, 74)}";
        lblSysUser!.Text = $"Συνδεδεμένος χρήστης: {snapshot.LoggedInUser}";

        var cpuPercent = snapshot.CpuUsagePercent;
        cardSysCpu!.Title = "CPU χρήση";
        cardSysCpu.PrimaryText = cpuPercent.HasValue ? $"{cpuPercent.Value:0}%" : "υπολογισμός...";
        cardSysCpu.SecondaryText = Shorten(snapshot.ProcessorName, 46);
        cardSysCpu.Percent = cpuPercent;

        var usedMemory = snapshot.TotalMemoryBytes > snapshot.AvailableMemoryBytes
            ? snapshot.TotalMemoryBytes - snapshot.AvailableMemoryBytes
            : 0;
        var ramPercent = snapshot.TotalMemoryBytes > 0 ? usedMemory * 100.0 / snapshot.TotalMemoryBytes : (double?)null;
        cardSysRam!.Title = "RAM";
        cardSysRam.PrimaryText = snapshot.TotalMemoryBytes == 0
            ? "άγνωστο"
            : $"{ramPercent:0}% χρήση";
        cardSysRam.SecondaryText = snapshot.TotalMemoryBytes == 0
            ? "Δεν ήταν δυνατή η ανάγνωση"
            : $"{SystemInfoService.HumanBytes(snapshot.AvailableMemoryBytes)} ελεύθερα / {SystemInfoService.HumanBytes(snapshot.TotalMemoryBytes)}";
        cardSysRam.Percent = ramPercent;

        RenderDriveCards(snapshot.Drives);
        RenderNetworkCards(snapshot.NetworkAdapters);

        if (lblSysUpdated is not null)
            lblSysUpdated.Text = "Live παρακολούθηση ενεργή";
    }

    private void RenderDriveCards(IReadOnlyList<DriveInfoSnapshot> drives)
    {
        if (_diskCardsFlow is null)
            return;

        var currentKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        _diskCardsFlow.SuspendLayout();
        try
        {
            if (drives.Count == 0)
            {
                if (_driveCardsByKey.Count > 0)
                    ClearSidebarFlow(_diskCardsFlow);

                _driveCardsByKey.Clear();
                if (_diskCardsFlow.Controls.Count == 0)
                    _diskCardsFlow.Controls.Add(CreateSidebarNote("Δεν βρέθηκαν διαθέσιμοι τοπικοί δίσκοι."));
                return;
            }

            foreach (var note in _diskCardsFlow.Controls.OfType<Label>().Where(x => x.Tag?.ToString() == "system-value").ToList())
            {
                _diskCardsFlow.Controls.Remove(note);
                note.Dispose();
            }

            foreach (var drive in drives)
            {
                var key = drive.Name;
                currentKeys.Add(key);

                if (!_driveCardsByKey.TryGetValue(key, out var card))
                {
                    card = CreateDriveCard(drive);
                    _driveCardsByKey[key] = card;
                    _diskCardsFlow.Controls.Add(card);
                }
                else
                {
                    UpdateDriveCard(card, drive);
                }
            }

            foreach (var stale in _driveCardsByKey.Keys.Where(k => !currentKeys.Contains(k)).ToList())
            {
                var card = _driveCardsByKey[stale];
                _diskCardsFlow.Controls.Remove(card);
                card.Dispose();
                _driveCardsByKey.Remove(stale);
            }
        }
        finally
        {
            _diskCardsFlow.ResumeLayout();
        }
    }



    private static void ClearSidebarFlow(FlowLayoutPanel flow)
    {
        foreach (Control control in flow.Controls.Cast<Control>().ToList())
        {
            flow.Controls.Remove(control);
            control.Dispose();
        }
    }

    private MetricCard CreateDriveCard(DriveInfoSnapshot drive)
    {
        var used = drive.TotalBytes - drive.AvailableBytes;
        var percent = drive.TotalBytes > 0 ? used * 100.0 / drive.TotalBytes : 0;
        var label = string.IsNullOrWhiteSpace(drive.Label) ? drive.Name.TrimEnd('\\') : drive.Label;
        var card = new MetricCard
        {
            Width = 292,
            Title = $"Δίσκος {drive.Name.TrimEnd('\\')} • {drive.Format}",
            PrimaryText = $"{percent:0}% δεσμευμένο",
            SecondaryText = $"{SystemInfoService.HumanBytes(drive.AvailableBytes)} ελεύθερα / {SystemInfoService.HumanBytes(drive.TotalBytes)} • {Shorten(label, 18)}",
            Percent = percent,
            Tag = "system-metric"
        };
        StyleMetricCard(card);
        return card;
    }


    private void UpdateDriveCard(MetricCard card, DriveInfoSnapshot drive)
    {
        var used = drive.TotalBytes - drive.AvailableBytes;
        var percent = drive.TotalBytes > 0 ? used * 100.0 / drive.TotalBytes : 0;
        var label = string.IsNullOrWhiteSpace(drive.Label) ? drive.Name.TrimEnd('\\') : drive.Label;
        card.Title = $"Δίσκος {drive.Name.TrimEnd('\\')} • {drive.Format}";
        card.PrimaryText = $"{percent:0}% δεσμευμένο";
        card.SecondaryText = $"{SystemInfoService.HumanBytes(drive.AvailableBytes)} ελεύθερα / {SystemInfoService.HumanBytes(drive.TotalBytes)} • {Shorten(label, 18)}";
        card.Percent = percent;
        StyleMetricCard(card);
    }

    private void RenderNetworkCards(IReadOnlyList<NetworkInfoSnapshot> adapters)
    {
        if (_networkCardsFlow is null)
            return;

        var currentKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        _networkCardsFlow.SuspendLayout();
        try
        {
            if (adapters.Count == 0)
            {
                if (_networkCardsByKey.Count > 0)
                    ClearSidebarFlow(_networkCardsFlow);

                _networkCardsByKey.Clear();
                if (_networkCardsFlow.Controls.Count == 0)
                    _networkCardsFlow.Controls.Add(CreateSidebarNote("Δεν βρέθηκε ενεργός LAN/Wi‑Fi adapter."));
                return;
            }

            foreach (var note in _networkCardsFlow.Controls.OfType<Label>().Where(x => x.Tag?.ToString() == "system-value").ToList())
            {
                _networkCardsFlow.Controls.Remove(note);
                note.Dispose();
            }

            foreach (var adapter in adapters)
            {
                var key = adapter.Name;
                currentKeys.Add(key);

                if (!_networkCardsByKey.TryGetValue(key, out var card))
                {
                    card = CreateNetworkCard(adapter);
                    _networkCardsByKey[key] = card;
                    _networkCardsFlow.Controls.Add(card);
                }
                else
                {
                    UpdateNetworkCard(card, adapter);
                }
            }

            foreach (var stale in _networkCardsByKey.Keys.Where(k => !currentKeys.Contains(k)).ToList())
            {
                var card = _networkCardsByKey[stale];
                _networkCardsFlow.Controls.Remove(card);
                card.Dispose();
                _networkCardsByKey.Remove(stale);
            }
        }
        finally
        {
            _networkCardsFlow.ResumeLayout();
        }
    }

    private MetricCard CreateNetworkCard(NetworkInfoSnapshot adapter)
    {
        var speed = adapter.SpeedBitsPerSecond > 0
            ? $"{adapter.SpeedBitsPerSecond / 1_000_000.0:0} Mbps"
            : "άγνωστη ταχύτητα";
        var percent = adapter.SpeedBitsPerSecond > 0
            ? Math.Clamp(adapter.SpeedBitsPerSecond / 1_000_000_000.0 * 100.0, 0, 100)
            : (double?)null;
        var card = new MetricCard
        {
            Width = 292,
            Title = Shorten(adapter.Name, 32),
            PrimaryText = speed,
            SecondaryText = $"{adapter.IpAddress} • {adapter.Status}",
            Percent = percent,
            Tag = "system-metric"
        };
        StyleMetricCard(card);
        return card;
    }


    private void UpdateNetworkCard(MetricCard card, NetworkInfoSnapshot adapter)
    {
        var speed = adapter.SpeedBitsPerSecond > 0
            ? $"{adapter.SpeedBitsPerSecond / 1_000_000.0:0} Mbps"
            : "άγνωστη ταχύτητα";
        var percent = adapter.SpeedBitsPerSecond > 0
            ? Math.Clamp(adapter.SpeedBitsPerSecond / 1_000_000_000.0 * 100.0, 0, 100)
            : (double?)null;
        card.Title = Shorten(adapter.Name, 32);
        card.PrimaryText = speed;
        card.SecondaryText = $"{adapter.IpAddress} • {adapter.Status}";
        card.Percent = percent;
        StyleMetricCard(card);
    }

    private Label CreateSidebarNote(string text)
    {
        return new Label
        {
            Text = text,
            AutoSize = false,
            Width = 292,
            Height = 44,
            Padding = new Padding(8, 6, 8, 6),
            Margin = new Padding(0, 4, 0, 0),
            Font = new Font("Segoe UI", 9F),
            Tag = "system-value",
            BackColor = _palette.CardBack,
            ForeColor = _palette.MutedText
        };
    }

    private static string Shorten(string value, int maxLength)
    {
        if (string.IsNullOrWhiteSpace(value) || value.Length <= maxLength)
            return value;
        return value[..Math.Max(0, maxLength - 1)] + "…";
    }

    protected override void OnFormClosed(FormClosedEventArgs e)
    {
        _systemInfoTimer.Stop();
        _systemInfoTimer.Dispose();
        base.OnFormClosed(e);
    }


    private void LoadSettingsToUi()
    {
        _isLoadingUi = true;
        try
        {
        txtComputerName.Text = GetSanitizedMachineName();

        txtDestinationRoot.Text = _settings.DestinationRoot;
        txtSourceUserRoot.Text = ResolveInitialSourceUserRoot(_settings.SourceUserRoot);
        txtExtensions.Text = EnsureRequiredExtensions(_settings.ExtensionFilter, ".glo", ".psg");
        rbAllFiles.Checked = _settings.IncludeAllFiles;
        rbSelectedExtensions.Checked = !_settings.IncludeAllFiles;
        LoadMaintenanceSettingsToHiddenControls();

        cboTheme.Items.Clear();
        cboTheme.Items.AddRange(new object[] { ThemeSystem, ThemeLight, ThemeDark, ThemeNotepadPlusPlus, ThemeNord, ThemeDracula });
        cboTheme.SelectedItem = NormalizeThemeMode(_settings.ThemeMode);

        clbSources.Items.Clear();
        AddDefaultSourceItems();
        foreach (var custom in _settings.CustomSourcePaths)
        {
            if (!string.IsNullOrWhiteSpace(custom) && !clbSources.Items.Cast<object>().Any(x => string.Equals(x.ToString(), custom, StringComparison.OrdinalIgnoreCase)))
                clbSources.Items.Add(custom, true);
        }
        UpdateSourcesHorizontalExtent();
        }
        finally
        {
            _isLoadingUi = false;
        }
    }

    private void AddDefaultSourceItems()
    {
        foreach (var path in GetDefaultSourcePaths())
        {
            var checkedByDefault = path.EndsWith("Pictures", StringComparison.OrdinalIgnoreCase) == false;
            clbSources.Items.Add(path, checkedByDefault);
        }
    }

    private IEnumerable<string> GetDefaultSourcePaths()
    {
        return GetDefaultSourcePathsForRoot(txtSourceUserRoot.Text);
    }

    private static IEnumerable<string> GetDefaultSourcePathsForRoot(string sourceUserRoot)
    {
        var userRoot = NormalizeSourceUserRoot(sourceUserRoot);
        return new[]
        {
            Path.Combine(userRoot, "Desktop"),
            Path.Combine(userRoot, "Documents"),
            Path.Combine(userRoot, "Downloads"),
            Path.Combine(userRoot, "Pictures")
        };
    }

    private static string ResolveInitialSourceUserRoot(string? savedValue)
    {
        if (string.IsNullOrWhiteSpace(savedValue))
            return AppSettings.DefaultSourceUserRoot;

        var normalized = Environment.ExpandEnvironmentVariables(savedValue.Trim()).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

        if (string.Equals(normalized, AppSettings.LegacyStudentSourceRoot, StringComparison.OrdinalIgnoreCase)
            && !Directory.Exists(AppSettings.LegacyStudentSourceRoot)
            && Directory.Exists(AppSettings.DefaultSourceUserRoot))
        {
            return AppSettings.DefaultSourceUserRoot;
        }

        return normalized;
    }

    private static string NormalizeSourceUserRoot(string value)
    {
        value = string.IsNullOrWhiteSpace(value) ? AppSettings.DefaultSourceUserRoot : Environment.ExpandEnvironmentVariables(value.Trim());
        return value.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
    }

    private void SaveSettingsFromUi(bool showMessage)
    {
        _settings.DestinationRoot = txtDestinationRoot.Text.Trim();
        _settings.SourceUserRoot = NormalizeSourceUserRoot(txtSourceUserRoot.Text);
        _settings.LastSelectedPc = GetComputerFolderName(); // stored only for compatibility; PC name is always read from Windows
        _settings.ExtensionFilter = txtExtensions.Text.Trim();
        _settings.IncludeAllFiles = rbAllFiles.Checked;
        _settings.ThemeMode = GetSelectedThemeMode();
        _settings.CustomSourcePaths = clbSources.Items
            .Cast<object>()
            .Select(x => x.ToString() ?? string.Empty)
            .Where(IsCustomSourcePath)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        _settings.Save();
        if (showMessage)
            MessageBox.Show("Οι ρυθμίσεις αποθηκεύτηκαν.", "OK", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    private void LoadMaintenanceSettingsToHiddenControls()
    {
        chkCleanUserTemp.Checked = _settings.CleanUserTemp;
        chkCleanWindowsTemp.Checked = _settings.CleanWindowsTemp;
        chkCleanDownloadsPlanned.Checked = _settings.CleanDownloadsPlannedFiles;
        chkEmptyRecycleBin.Checked = _settings.EmptyRecycleBin;
        chkOptimizeDrive.Checked = _settings.OptimizeSystemDrive;
        chkCleanWindowsUpdateComponents.Checked = _settings.CleanWindowsUpdateComponents;
    }

    private bool IsCustomSourcePath(string path)
    {
        var defaults = new HashSet<string>(GetDefaultSourcePaths(), StringComparer.OrdinalIgnoreCase);
        return !defaults.Contains(path);
    }

    private void btnUseCurrentUserRoot_Click(object? sender, EventArgs e)
    {
        txtSourceUserRoot.Text = AppSettings.DefaultSourceUserRoot;
        btnApplySourceUserRoot_Click(sender, e);
    }

    private void btnApplySourceUserRoot_Click(object? sender, EventArgs e)
    {
        var oldDefaults = new HashSet<string>(GetDefaultSourcePathsForRoot(_settings.SourceUserRoot), StringComparer.OrdinalIgnoreCase);
        var customPaths = clbSources.Items
            .Cast<object>()
            .Select(x => x.ToString() ?? string.Empty)
            .Where(x => !oldDefaults.Contains(x))
            .ToList();

        txtSourceUserRoot.Text = NormalizeSourceUserRoot(txtSourceUserRoot.Text);

        clbSources.Items.Clear();
        AddDefaultSourceItems();
        foreach (var custom in customPaths)
        {
            if (!string.IsNullOrWhiteSpace(custom) && !clbSources.Items.Cast<object>().Any(x => string.Equals(x.ToString(), custom, StringComparison.OrdinalIgnoreCase)))
                clbSources.Items.Add(custom, true);
        }
        UpdateSourcesHorizontalExtent();

        SaveSettingsFromUi(showMessage: false);
        WriteStatus($"Οι βασικές πηγές ενημερώθηκαν σε {txtSourceUserRoot.Text}.");
    }

    private void btnDetectPc_Click(object? sender, EventArgs e)
    {
        SetComputerNameFromWindows(showStatus: true);
        UpdateFinalPathLabel();
        UpdateSummary();
    }

    private void btnBrowseDestination_Click(object? sender, EventArgs e)
    {
        folderBrowserDialog.Description = @"Διάλεξε root φάκελο προορισμού, π.χ. \\192.168.1.37\Backup\2026";
        folderBrowserDialog.SelectedPath = Directory.Exists(txtDestinationRoot.Text.Trim()) ? txtDestinationRoot.Text.Trim() : string.Empty;

        if (folderBrowserDialog.ShowDialog(this) == DialogResult.OK)
            txtDestinationRoot.Text = folderBrowserDialog.SelectedPath;
    }

    private void btnTestDestination_Click(object? sender, EventArgs e)
    {
        TestDestination();
    }

    private void btnSaveSettings_Click(object? sender, EventArgs e)
    {
        SaveSettingsFromUi(showMessage: true);
    }

    private void btnAddSource_Click(object? sender, EventArgs e)
    {
        var path = txtCustomSource.Text.Trim();
        if (string.IsNullOrWhiteSpace(path))
        {
            folderBrowserDialog.Description = "Διάλεξε φάκελο πηγής αρχείων";
            folderBrowserDialog.SelectedPath = string.Empty;
            if (folderBrowserDialog.ShowDialog(this) != DialogResult.OK)
                return;
            path = folderBrowserDialog.SelectedPath;
        }

        if (!clbSources.Items.Cast<object>().Any(x => string.Equals(x.ToString(), path, StringComparison.OrdinalIgnoreCase)))
            clbSources.Items.Add(path, true);

        UpdateSourcesHorizontalExtent();
        txtCustomSource.Clear();
    }

    private void btnRemoveSource_Click(object? sender, EventArgs e)
    {
        if (clbSources.SelectedItem is null) return;
        var selected = clbSources.SelectedItem.ToString() ?? string.Empty;
        if (!IsCustomSourcePath(selected))
        {
            MessageBox.Show("Οι βασικές πηγές δεν αφαιρούνται. Μπορείς απλά να τις ξετσεκάρεις.", "Προσοχή", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        clbSources.Items.Remove(clbSources.SelectedItem);
        UpdateSourcesHorizontalExtent();
    }

    private async void btnScan_Click(object? sender, EventArgs e)
    {
        await ScanAsync();
    }

    private async void btnExecute_Click(object? sender, EventArgs e)
    {
        await ExecuteAsync();
    }

    private void btnCancel_Click(object? sender, EventArgs e)
    {
        if (_cts is null || _cts.IsCancellationRequested)
            return;

        var confirm = MessageBox.Show(
            "Να σταματήσει η τρέχουσα διαδικασία;\n\n" +
            "Στη σάρωση θα επανέλθει το προηγούμενο preview/plan.\n" +
            "Σε Copy/Move θα ολοκληρωθεί ή θα καθαριστεί το τρέχον ασφαλές βήμα και μετά θα σταματήσει.\n\n" +
            "Συνέχεια με Stop;",
            "Επιβεβαίωση Stop",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Warning,
            MessageBoxDefaultButton.Button2);

        if (confirm != DialogResult.Yes)
            return;

        _cts.Cancel();
        btnCancel.Enabled = false;
        WriteStatus("Ζητήθηκε διακοπή. Ολοκληρώνεται το τρέχον ασφαλές βήμα και σταματά.");
    }

    private async void btnUndoLast_Click(object? sender, EventArgs e)
    {
        await UndoLastTransferAsync();
    }

    private void btnOpenDestination_Click(object? sender, EventArgs e)
    {
        OpenPath(GetPcDestinationRoot());
    }

    private void btnOpenLog_Click(object? sender, EventArgs e)
    {
        if (!string.IsNullOrWhiteSpace(_lastLogPath))
            OpenPath(_lastLogPath);
        else
            MessageBox.Show("Δεν υπάρχει ακόμα log από αυτή τη συνεδρία.", "Log", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    private void btnMaintenance_Click(object? sender, EventArgs e)
    {
        SaveSettingsFromUi(showMessage: false);
        using var form = new MaintenanceForm(
            _settings,
            NormalizeSourceUserRoot(txtSourceUserRoot.Text),
            txtDestinationRoot.Text.Trim(),
            GetComputerFolderName(),
            ShouldUseDarkTheme(),
            WriteStatus);

        form.ShowDialog(this);
        LoadMaintenanceSettingsToHiddenControls();
        UpdateSummary();
    }

    private void btnHistory_Click(object? sender, EventArgs e)
    {
        using var form = new HistoryForm(txtDestinationRoot.Text.Trim(), ShouldUseDarkTheme());
        form.ShowDialog(this);
    }

    private void btnHelp_Click(object? sender, EventArgs e)
    {
        using var form = new HelpForm(ShouldUseDarkTheme());
        form.ShowDialog(this);
    }

    private void btnAbout_Click(object? sender, EventArgs e)
    {
        using var form = new AboutForm(ShouldUseDarkTheme());
        form.ShowDialog(this);
    }


    private void txtDestinationRoot_TextChanged(object? sender, EventArgs e)
    {
        UpdateFinalPathLabel();
        UpdateSummary();
    }

    private void rbAllFiles_CheckedChanged(object? sender, EventArgs e)
    {
        UpdateExtensionsEnabled();
    }

    private void rbSelectedExtensions_CheckedChanged(object? sender, EventArgs e)
    {
        UpdateExtensionsEnabled();
    }

    private void cboTheme_SelectedIndexChanged(object? sender, EventArgs e)
    {
        if (_isLoadingUi) return;
        ApplySelectedTheme();
        SaveSettingsFromUi(showMessage: false);
    }

    private void UpdateExtensionsEnabled()
    {
        txtExtensions.Enabled = rbSelectedExtensions.Checked;
    }


    private void AutoDetectPcOnStartup()
    {
        SetComputerNameFromWindows(showStatus: false);
        WriteStatus($"Αυτόματη ανίχνευση ονόματος Windows: {GetComputerFolderName()}");
    }

    private void SetComputerNameFromWindows(bool showStatus)
    {
        var machineName = GetSanitizedMachineName();
        txtComputerName.Text = machineName;

        if (showStatus)
            WriteStatus($"Ορίστηκε όνομα υπολογιστή από Windows: {machineName}");
    }

    private static string GetSanitizedMachineName()
    {
        var machineName = (Environment.MachineName ?? string.Empty).Trim();
        if (string.IsNullOrWhiteSpace(machineName))
            machineName = "PC";

        foreach (var c in Path.GetInvalidFileNameChars())
            machineName = machineName.Replace(c, '_');

        machineName = machineName.Trim().TrimEnd('.');
        return string.IsNullOrWhiteSpace(machineName) ? "PC" : machineName;
    }

    private static string NormalizeThemeMode(string? value)
    {
        if (string.Equals(value, ThemeDracula, StringComparison.OrdinalIgnoreCase)) return ThemeDracula;
        if (string.Equals(value, ThemeNord, StringComparison.OrdinalIgnoreCase)) return ThemeNord;
        if (string.Equals(value, ThemeNotepadPlusPlus, StringComparison.OrdinalIgnoreCase) || string.Equals(value, "Notepad++", StringComparison.OrdinalIgnoreCase)) return ThemeNotepadPlusPlus;
        if (string.Equals(value, ThemeDark, StringComparison.OrdinalIgnoreCase) || string.Equals(value, "Dark", StringComparison.OrdinalIgnoreCase) || string.Equals(value, "Σκούρο", StringComparison.OrdinalIgnoreCase)) return ThemeDark;
        if (string.Equals(value, ThemeLight, StringComparison.OrdinalIgnoreCase) || string.Equals(value, "Light", StringComparison.OrdinalIgnoreCase) || string.Equals(value, "Φωτεινό", StringComparison.OrdinalIgnoreCase)) return ThemeLight;
        return ThemeSystem;
    }

    private string GetSelectedThemeMode()
    {
        return NormalizeThemeMode(cboTheme.SelectedItem?.ToString());
    }

    private bool ShouldUseDarkTheme()
    {
        return GetSelectedThemeMode() switch
        {
            ThemeDark or ThemeNotepadPlusPlus or ThemeNord or ThemeDracula => true,
            ThemeLight => false,
            _ => IsWindowsAppDarkTheme()
        };
    }

    private static bool IsWindowsAppDarkTheme()
    {
        try
        {
            using var key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize");
            var appsUseLightTheme = key?.GetValue("AppsUseLightTheme");
            return appsUseLightTheme is int value && value == 0;
        }
        catch
        {
            return false;
        }
    }

    private void ApplySelectedTheme()
    {
        var theme = GetSelectedThemeMode();
        var dark = ShouldUseDarkTheme();
        _palette = UiPalette.ForTheme(theme, dark, GetWindowsAccentColor());

        ApplyThemeRecursive(this, _palette);
        TrySetWindowsTitleBarMode(this, dark);
        ApplyModernScrollbarTheme(this, dark);

        lvPreview.OwnerDraw = true;
        lvPreview.BackColor = _palette.InputBack;
        lvPreview.ForeColor = _palette.Text;
        lvPreview.GridLines = true;
        ConfigurePreviewColumnsForScrolling();
        UpdateSourcesHorizontalExtent();
        lvPreview.Invalidate();

        txtStatus.BackColor = _palette.StatusBack;
        txtStatus.ForeColor = _palette.Text;
        lblMaintenanceNote.ForeColor = _palette.MutedText;
        lblTitle.ForeColor = _palette.TitleText;
        lblTitle.BackColor = _palette.HeaderSurface;
        titleLayout.BackColor = _palette.HeaderSurface;
        picLogo.BackColor = _palette.HeaderSurface;
        rootLayout.BackColor = _palette.FormBack;
        if (_outerLayout is not null) _outerLayout.BackColor = _palette.FormBack;
        if (_systemSidebar is not null)
        {
            _systemSidebar.BackColor = _palette.SidebarBack;
            _systemSidebar.ForeColor = _palette.Text;
        }
        StyleSystemInfoSidebar();
    }

    private void StyleSystemInfoSidebar()
    {
        if (_systemInfoFlow is null) return;

        _systemInfoFlow.BackColor = _palette.SidebarBack;
        StyleSidebarControlTree(_systemInfoFlow);
    }

    private void StyleSidebarControlTree(Control parent)
    {
        foreach (Control child in parent.Controls)
        {
            switch (child)
            {
                case MetricCard metricCard:
                    StyleMetricCard(metricCard);
                    break;
                case FlowLayoutPanel flow when child.Tag?.ToString() == "system-flow":
                    flow.BackColor = _palette.SidebarBack;
                    flow.ForeColor = _palette.Text;
                    StyleSidebarControlTree(flow);
                    break;
                case Label label when child.Tag?.ToString() == "system-header":
                    label.ForeColor = _palette.TitleText;
                    label.BackColor = _palette.SidebarBack;
                    break;
                case Label label when child.Tag?.ToString() == "system-value":
                    label.ForeColor = _palette.Text;
                    label.BackColor = _palette.CardBack;
                    break;
                case PictureBox picture when child.Tag?.ToString() == "nyx-logo":
                    picture.BackColor = _palette.SidebarBack;
                    break;
                case TableLayoutPanel layout when child.Tag?.ToString() == "sidebar-layout":
                    layout.BackColor = _palette.SidebarBack;
                    StyleSidebarControlTree(layout);
                    break;
                default:
                    child.ForeColor = _palette.Text;
                    child.BackColor = _palette.SidebarBack;
                    break;
            }
        }
    }

    private void StyleMetricCard(MetricCard metricCard)
    {
        metricCard.ApplyColors(
            _palette.CardBack,
            _palette.Text,
            _palette.MutedText,
            _palette.StatusBack,
            _palette.SelectionBack,
            _palette.ButtonBorder);
    }

    private static void ApplyThemeRecursive(Control control, UiPalette palette)
    {
        switch (control)
        {
            case Form:
                control.BackColor = palette.FormBack;
                control.ForeColor = palette.Text;
                break;
            case TextBox textBox:
                textBox.BackColor = palette.InputBack;
                textBox.ForeColor = palette.InputText;
                textBox.BorderStyle = BorderStyle.FixedSingle;
                break;
            case ComboBox comboBox:
                comboBox.BackColor = palette.InputBack;
                comboBox.ForeColor = palette.InputText;
                comboBox.FlatStyle = FlatStyle.Flat;
                break;
            case CheckedListBox checkedListBox:
                checkedListBox.BackColor = palette.InputBack;
                checkedListBox.ForeColor = palette.InputText;
                checkedListBox.BorderStyle = BorderStyle.FixedSingle;
                break;
            case ListView listView:
                listView.BackColor = palette.InputBack;
                listView.ForeColor = palette.InputText;
                listView.BorderStyle = BorderStyle.FixedSingle;
                break;
            case Button button:
                button.UseVisualStyleBackColor = false;
                button.BackColor = palette.ButtonBack;
                button.ForeColor = palette.Text;
                button.FlatStyle = FlatStyle.Flat;
                button.FlatAppearance.BorderColor = palette.ButtonBorder;
                button.FlatAppearance.MouseOverBackColor = palette.ButtonHover;
                button.FlatAppearance.MouseDownBackColor = palette.ButtonPressed;
                break;
            case ModernGroupBox modernGroupBox:
                modernGroupBox.BackColor = palette.CardBack;
                modernGroupBox.ForeColor = palette.Text;
                modernGroupBox.BorderColor = palette.ButtonBorder;
                modernGroupBox.Invalidate();
                break;
            case GroupBox:
                control.BackColor = palette.CardBack;
                control.ForeColor = palette.Text;
                break;
            case Panel or SplitContainer or TableLayoutPanel or FlowLayoutPanel:
                control.BackColor = palette.PanelBack;
                control.ForeColor = palette.Text;
                break;
            case CheckBox or RadioButton:
                control.BackColor = control.Parent?.BackColor ?? palette.CardBack;
                control.ForeColor = palette.Text;
                break;
            case Label label:
                label.BackColor = label.Parent?.BackColor ?? palette.PanelBack;
                label.ForeColor = palette.Text;
                break;
            case PictureBox:
                control.BackColor = control.Parent?.BackColor ?? palette.PanelBack;
                break;
            default:
                control.BackColor = control.Parent?.BackColor ?? palette.PanelBack;
                control.ForeColor = palette.Text;
                break;
        }

        foreach (Control child in control.Controls)
            ApplyThemeRecursive(child, palette);
    }

    private static Color GetWindowsAccentColor()
    {
        try
        {
            using var key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\DWM");
            var raw = key?.GetValue("ColorizationColor");
            if (raw is int value)
            {
                var bytes = BitConverter.GetBytes(value);
                return Color.FromArgb(bytes[2], bytes[1], bytes[0]);
            }
        }
        catch
        {
            // Ignore and fall back to Windows blue.
        }

        return Color.FromArgb(0, 120, 215);
    }

    private static Color Blend(Color a, Color b, double amount)
    {
        amount = Math.Clamp(amount, 0, 1);
        var r = (int)Math.Round(a.R + (b.R - a.R) * amount);
        var g = (int)Math.Round(a.G + (b.G - a.G) * amount);
        var bl = (int)Math.Round(a.B + (b.B - a.B) * amount);
        return Color.FromArgb(r, g, bl);
    }

    private static void TrySetWindowsTitleBarMode(Form form, bool dark)
    {
        try
        {
            if (!OperatingSystem.IsWindowsVersionAtLeast(10, 0, 17763))
                return;

            var useDark = dark ? 1 : 0;
            _ = DwmSetWindowAttribute(form.Handle, 20, ref useDark, sizeof(int));
            _ = DwmSetWindowAttribute(form.Handle, 19, ref useDark, sizeof(int));
        }
        catch
        {
            // Older Windows versions simply keep the normal title bar.
        }
    }

    [DllImport("dwmapi.dll")]
    private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int attrValue, int attrSize);

    [DllImport("uxtheme.dll", CharSet = CharSet.Unicode)]
    private static extern int SetWindowTheme(IntPtr hWnd, string? pszSubAppName, string? pszSubIdList);

    private static void ApplyModernScrollbarTheme(Control control, bool dark)
    {
        try
        {
            if (control is TextBox or ListView or CheckedListBox or ComboBox)
                _ = SetWindowTheme(control.Handle, dark ? "DarkMode_Explorer" : "Explorer", null);
        }
        catch
        {
            // Native scrollbar theming is best-effort only.
        }

        foreach (Control child in control.Controls)
            ApplyModernScrollbarTheme(child, dark);
    }

    private void ConfigurePreviewColumnsForScrolling()
    {
        if (lvPreview.Columns.Count == 0)
            return;

        // Keep the important path columns wide enough. When the sum of columns is
        // wider than the visible area, ListView shows the horizontal scrollbar,
        // which is much better for long school/network paths than squeezing text.
        colFile.Width = 260;
        colPlacement.Width = 90;
        colFolder.Width = 260;
        colSize.Width = 90;
        colSource.Width = 260;
        colDestination.Width = Math.Max(900, lvPreview.ClientSize.Width - 420);

        lvPreview.Scrollable = true;
        lvPreview.HideSelection = false;
    }

    private void UpdateSourcesHorizontalExtent()
    {
        try
        {
            var maxWidth = 0;
            foreach (var item in clbSources.Items)
            {
                var text = item?.ToString() ?? string.Empty;
                var measured = TextRenderer.MeasureText(text, clbSources.Font).Width + 42;
                if (measured > maxWidth)
                    maxWidth = measured;
            }

            clbSources.HorizontalExtent = Math.Max(clbSources.ClientSize.Width + 40, maxWidth);
        }
        catch
        {
            // Scrollbar sizing is purely cosmetic. Ignore any GDI/handle timing issue.
        }
    }

    private void HookPreviewDrawing()
    {
        if (_previewDrawingHooked)
            return;

        _previewDrawingHooked = true;

        lvPreview.DrawColumnHeader += (_, e) =>
        {
            var headerText = e.Header?.Text ?? string.Empty;
            using var backBrush = new SolidBrush(_palette.ListHeaderBack);
            using var borderPen = new Pen(_palette.GridLine);
            e.Graphics.FillRectangle(backBrush, e.Bounds);
            e.Graphics.DrawRectangle(borderPen, e.Bounds);
            TextRenderer.DrawText(
                e.Graphics,
                headerText,
                e.Font,
                new Rectangle(e.Bounds.X + 8, e.Bounds.Y, e.Bounds.Width - 10, e.Bounds.Height),
                _palette.TitleText,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
        };

        lvPreview.DrawItem += (_, _) =>
        {
            // Sub-items are drawn in DrawSubItem.
        };

        lvPreview.DrawSubItem += (_, e) =>
        {
            var item = e.Item;
            var subItem = e.SubItem;
            if (item is null || subItem is null)
                return;

            var selected = item.Selected;
            var backColor = selected ? _palette.SelectionBack : (e.ItemIndex % 2 == 0 ? _palette.InputBack : _palette.RowAltBack);
            var foreColor = selected ? _palette.SelectionText : _palette.InputText;
            using var backBrush = new SolidBrush(backColor);
            using var gridPen = new Pen(_palette.GridLine);

            e.Graphics.FillRectangle(backBrush, e.Bounds);
            e.Graphics.DrawRectangle(gridPen, e.Bounds);

            var textBounds = new Rectangle(e.Bounds.X + 6, e.Bounds.Y, e.Bounds.Width - 8, e.Bounds.Height);
            if (e.ColumnIndex == 0)
            {
                var checkState = item.Checked ? CheckBoxState.CheckedNormal : CheckBoxState.UncheckedNormal;
                var glyph = CheckBoxRenderer.GetGlyphSize(e.Graphics, checkState);
                var checkLocation = new Point(e.Bounds.X + 5, e.Bounds.Y + Math.Max(0, (e.Bounds.Height - glyph.Height) / 2));
                CheckBoxRenderer.DrawCheckBox(e.Graphics, checkLocation, checkState);
                textBounds = new Rectangle(e.Bounds.X + glyph.Width + 12, e.Bounds.Y, Math.Max(1, e.Bounds.Width - glyph.Width - 14), e.Bounds.Height);
            }

            TextRenderer.DrawText(
                e.Graphics,
                subItem.Text,
                item.Font,
                textBounds,
                foreColor,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
        };
    }

    private sealed class UiPalette
    {
        public required Color FormBack { get; init; }
        public required Color PanelBack { get; init; }
        public required Color CardBack { get; init; }
        public required Color SidebarBack { get; init; }
        public required Color HeaderSurface { get; init; }
        public required Color InputBack { get; init; }
        public required Color StatusBack { get; init; }
        public required Color ListHeaderBack { get; init; }
        public required Color RowAltBack { get; init; }
        public required Color ButtonBack { get; init; }
        public required Color ButtonHover { get; init; }
        public required Color ButtonPressed { get; init; }
        public required Color ButtonBorder { get; init; }
        public required Color SelectionBack { get; init; }
        public required Color SelectionText { get; init; }
        public required Color GridLine { get; init; }
        public required Color Text { get; init; }
        public required Color InputText { get; init; }
        public required Color MutedText { get; init; }
        public required Color TitleText { get; init; }

        public static UiPalette Light { get; } = For(false, Color.FromArgb(0, 120, 215));

        public static UiPalette ForTheme(string themeMode, bool dark, Color accent)
        {
            return themeMode switch
            {
                ThemeNotepadPlusPlus => ForCustomDark(
                    Color.FromArgb(30, 32, 30),
                    Color.FromArgb(35, 38, 35),
                    Color.FromArgb(39, 42, 39),
                    Color.FromArgb(18, 20, 18),
                    Color.FromArgb(118, 185, 0)),
                ThemeNord => ForCustomDark(
                    Color.FromArgb(46, 52, 64),
                    Color.FromArgb(59, 66, 82),
                    Color.FromArgb(67, 76, 94),
                    Color.FromArgb(36, 41, 51),
                    Color.FromArgb(136, 192, 208)),
                ThemeDracula => ForCustomDark(
                    Color.FromArgb(40, 42, 54),
                    Color.FromArgb(48, 50, 65),
                    Color.FromArgb(55, 57, 73),
                    Color.FromArgb(32, 34, 45),
                    Color.FromArgb(189, 147, 249)),
                _ => For(dark, accent)
            };
        }

        public static UiPalette For(bool dark, Color accent)
        {
            var accentSoftLight = Blend(Color.White, accent, 0.10);
            var accentMidLight = Blend(Color.White, accent, 0.18);
            var accentSoftDark = Blend(Color.FromArgb(31, 41, 55), accent, 0.20);
            var accentMidDark = Blend(Color.FromArgb(31, 41, 55), accent, 0.32);

            if (!dark)
            {
                return new UiPalette
                {
                    FormBack = Color.FromArgb(243, 247, 252),
                    PanelBack = Color.FromArgb(243, 247, 252),
                    CardBack = Color.FromArgb(252, 253, 255),
                    SidebarBack = Color.FromArgb(235, 242, 250),
                    HeaderSurface = accentSoftLight,
                    InputBack = Color.White,
                    StatusBack = Color.FromArgb(248, 251, 255),
                    ListHeaderBack = accentMidLight,
                    RowAltBack = Color.FromArgb(247, 250, 255),
                    ButtonBack = accentSoftLight,
                    ButtonHover = accentMidLight,
                    ButtonPressed = Blend(Color.White, accent, 0.30),
                    ButtonBorder = Blend(Color.FromArgb(180, 190, 205), accent, 0.25),
                    SelectionBack = accent,
                    SelectionText = Color.White,
                    GridLine = Color.FromArgb(218, 226, 238),
                    Text = Color.FromArgb(31, 41, 55),
                    InputText = Color.FromArgb(17, 24, 39),
                    MutedText = Color.FromArgb(100, 116, 139),
                    TitleText = Blend(Color.FromArgb(17, 24, 39), accent, 0.25)
                };
            }

            return new UiPalette
            {
                FormBack = Color.FromArgb(31, 41, 55),
                PanelBack = Color.FromArgb(35, 47, 63),
                CardBack = Color.FromArgb(39, 52, 70),
                SidebarBack = Color.FromArgb(27, 37, 51),
                HeaderSurface = accentSoftDark,
                InputBack = Color.FromArgb(24, 34, 48),
                StatusBack = Color.FromArgb(21, 30, 43),
                ListHeaderBack = accentMidDark,
                RowAltBack = Color.FromArgb(28, 40, 56),
                ButtonBack = Color.FromArgb(44, 59, 79),
                ButtonHover = accentSoftDark,
                ButtonPressed = accentMidDark,
                ButtonBorder = Blend(Color.FromArgb(92, 113, 140), accent, 0.35),
                SelectionBack = Blend(Color.FromArgb(30, 64, 175), accent, 0.25),
                SelectionText = Color.White,
                GridLine = Color.FromArgb(70, 88, 112),
                Text = Color.FromArgb(235, 241, 248),
                InputText = Color.FromArgb(241, 245, 249),
                MutedText = Color.FromArgb(173, 186, 204),
                TitleText = Blend(Color.FromArgb(235, 241, 248), accent, 0.22)
            };
        }

        private static UiPalette ForCustomDark(Color formBack, Color panelBack, Color cardBack, Color inputBack, Color accent)
        {
            return new UiPalette
            {
                FormBack = formBack,
                PanelBack = panelBack,
                CardBack = cardBack,
                SidebarBack = Blend(formBack, Color.Black, 0.18),
                HeaderSurface = Blend(cardBack, accent, 0.18),
                InputBack = inputBack,
                StatusBack = Blend(inputBack, Color.Black, 0.15),
                ListHeaderBack = Blend(cardBack, accent, 0.24),
                RowAltBack = Blend(inputBack, cardBack, 0.45),
                ButtonBack = Blend(cardBack, accent, 0.12),
                ButtonHover = Blend(cardBack, accent, 0.25),
                ButtonPressed = Blend(cardBack, accent, 0.38),
                ButtonBorder = Blend(cardBack, accent, 0.55),
                SelectionBack = Blend(accent, Color.Black, 0.10),
                SelectionText = Color.White,
                GridLine = Blend(cardBack, accent, 0.20),
                Text = Color.FromArgb(238, 243, 248),
                InputText = Color.FromArgb(245, 247, 250),
                MutedText = Color.FromArgb(176, 188, 204),
                TitleText = Blend(Color.White, accent, 0.20)
            };
        }
    }

    private void TestDestination()
    {
        try
        {
            var root = txtDestinationRoot.Text.Trim();
            if (string.IsNullOrWhiteSpace(root))
            {
                MessageBox.Show("Δεν έχει οριστεί προορισμός.", "Σφάλμα", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Directory.CreateDirectory(GetPcDestinationRoot());
            var free = FileTransferService.TryGetAvailableBytes(root);
            var freeText = free.HasValue ? FileTransferService.HumanBytes(free.Value) : "άγνωστο για αυτό το path";
            MessageBox.Show($"Ο προορισμός είναι διαθέσιμος.\n\nΤελικός φάκελος PC:\n{GetPcDestinationRoot()}\n\nΕλεύθερος χώρος: {freeText}", "OK", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Δεν μπορώ να προσπελάσω τον προορισμό.\n\n{ex.Message}", "Σφάλμα", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void lvPreview_ItemChecked(object? sender, ItemCheckedEventArgs e)
    {
        if (e.Item.Tag is PlannedFile plannedFile)
        {
            plannedFile.IsSelected = e.Item.Checked;
            UpdateSummary();
        }
    }

    private void btnSelectAllPreview_Click(object? sender, EventArgs e)
    {
        SetAllPreviewItemsChecked(true);
    }

    private void btnClearPreviewSelection_Click(object? sender, EventArgs e)
    {
        SetAllPreviewItemsChecked(false);
    }

    private void btnSelectFolderPreview_Click(object? sender, EventArgs e)
    {
        SetSameFolderAsSelected(true);
    }

    private void btnClearFolderPreview_Click(object? sender, EventArgs e)
    {
        SetSameFolderAsSelected(false);
    }

    private void SetAllPreviewItemsChecked(bool isChecked)
    {
        lvPreview.BeginUpdate();
        try
        {
            foreach (ListViewItem item in lvPreview.Items)
                item.Checked = isChecked;

            foreach (var item in _currentPlan)
                item.IsSelected = isChecked;
        }
        finally
        {
            lvPreview.EndUpdate();
        }
        UpdateSummary();
    }

    private void SetSameFolderAsSelected(bool isChecked)
    {
        if (lvPreview.SelectedItems.Count == 0 || lvPreview.SelectedItems[0].Tag is not PlannedFile selected)
        {
            MessageBox.Show("Επίλεξε πρώτα ένα αρχείο από τον φάκελο που θέλεις να αλλάξεις.", "Επιλογή φακέλου", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        var sourceRoot = selected.SourceRoot;
        var relativeFolder = selected.RelativeFolder;
        var sourceLabel = selected.SourceLabel;

        var matches = _currentPlan.Where(x =>
            string.Equals(x.SourceRoot, sourceRoot, StringComparison.OrdinalIgnoreCase) &&
            string.Equals(x.SourceLabel, sourceLabel, StringComparison.OrdinalIgnoreCase) &&
            string.Equals(x.RelativeFolder, relativeFolder, StringComparison.OrdinalIgnoreCase)).ToList();

        foreach (var planned in matches)
            planned.IsSelected = isChecked;

        lvPreview.BeginUpdate();
        try
        {
            foreach (ListViewItem row in lvPreview.Items)
            {
                if (row.Tag is PlannedFile planned && matches.Contains(planned))
                    row.Checked = isChecked;
            }
        }
        finally
        {
            lvPreview.EndUpdate();
        }

        var action = isChecked ? "επιλέχθηκαν" : "απορρίφθηκαν";
        var folderName = string.IsNullOrWhiteSpace(relativeFolder) ? "χύμα αρχεία στη ρίζα της πηγής" : relativeFolder;
        WriteStatus($@"{matches.Count} αρχεία {action} από: {sourceLabel}\{folderName}");
        UpdateSummary();
    }

    private void SyncPreviewSelectionToPlan()
    {
        foreach (ListViewItem row in lvPreview.Items)
        {
            if (row.Tag is PlannedFile planned)
                planned.IsSelected = row.Checked;
        }
    }

    private List<PlannedFile> GetSelectedPlan()
    {
        SyncPreviewSelectionToPlan();
        return _currentPlan.Where(x => x.IsSelected).ToList();
    }

    private async Task<bool> ScanAsync()
    {
        SaveSettingsFromUi(showMessage: false);
        var previousPlan = _currentPlan.ToList();
        lvPreview.Items.Clear();
        _currentPlan.Clear();
        progressBar.Value = 0;
        progressBar.Style = ProgressBarStyle.Marquee;
        WriteStatus("Σάρωση... Πάτα Stop αν διάλεξες λάθος φάκελο.");

        var selectedSources = clbSources.CheckedItems.Cast<object>().Select(x => x.ToString() ?? string.Empty).ToList();
        var extensions = SplitExtensions(txtExtensions.Text).ToList();
        var destinationRoot = txtDestinationRoot.Text.Trim();
        var pc = GetComputerFolderName();

        if (string.IsNullOrWhiteSpace(destinationRoot))
        {
            progressBar.Style = ProgressBarStyle.Blocks;
            MessageBox.Show("Όρισε root προορισμού πρώτα.", "Σφάλμα", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            RestorePlan(previousPlan);
            return false;
        }

        if (selectedSources.Count == 0)
        {
            progressBar.Style = ProgressBarStyle.Blocks;
            MessageBox.Show("Διάλεξε τουλάχιστον μία πηγή αρχείων.", "Πηγές", MessageBoxButtons.OK, MessageBoxIcon.Information);
            RestorePlan(previousPlan);
            return false;
        }

        SetBusy(true);
        _cts = new CancellationTokenSource();

        try
        {
            var token = _cts.Token;
            var plan = await Task.Run(() => FileTransferService.BuildPlan(
                selectedSources,
                destinationRoot,
                pc,
                rbAllFiles.Checked,
                extensions,
                WriteStatus,
                token), CancellationToken.None);

            if (token.IsCancellationRequested)
            {
                WriteStatus("Η σάρωση ακυρώθηκε. Επαναφέρθηκε το προηγούμενο preview/plan.");
                RestorePlan(previousPlan);
                return false;
            }

            _currentPlan = plan;
            PopulatePreview();

            if (_currentPlan.Count > 5000)
                WriteStatus("Στο preview εμφανίζονται τα πρώτα 5000 αρχεία για λόγους ταχύτητας.");

            var looseCount = _currentPlan.Count(x => x.IsLooseFile);
            var folderCount = _currentPlan.Count - looseCount;
            WriteStatus($"Βρέθηκαν {_currentPlan.Count} αρχεία. Χύμα: {looseCount}, σε φακέλους: {folderCount}.");
            WriteStatus(@"Η δομή φακέλων διατηρείται στον προορισμό κάτω από τον φάκελο της πηγής, π.χ. Desktop\ΌνομαΦακέλου\αρχείο.glo.");
            UpdateSummary();
            return true;
        }
        catch (OperationCanceledException)
        {
            WriteStatus("Η σάρωση ακυρώθηκε. Επαναφέρθηκε το προηγούμενο preview/plan.");
            RestorePlan(previousPlan);
            return false;
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Σφάλμα σάρωσης", MessageBoxButtons.OK, MessageBoxIcon.Error);
            RestorePlan(previousPlan);
            return false;
        }
        finally
        {
            progressBar.Style = ProgressBarStyle.Blocks;
            progressBar.Value = 0;
            SetBusy(false);
            _cts?.Dispose();
            _cts = null;
            UpdateSummary();
        }
    }

    private void RestorePlan(List<PlannedFile> previousPlan)
    {
        _currentPlan = previousPlan;
        PopulatePreview();
        UpdateSummary();
    }

    private void PopulatePreview()
    {
        lvPreview.BeginUpdate();
        try
        {
            lvPreview.Items.Clear();
            foreach (var item in _currentPlan.Take(5000))
            {
                var row = new ListViewItem(Path.GetFileName(item.SourcePath))
                {
                    Checked = item.IsSelected,
                    Tag = item
                };
                row.SubItems.Add(item.PlacementLabel);
                row.SubItems.Add(item.PreviewFolder);
                row.SubItems.Add(FileTransferService.HumanBytes(item.SizeBytes));
                row.SubItems.Add(item.SourceLabel);
                row.SubItems.Add(item.DestinationPath);
                lvPreview.Items.Add(row);
            }
        }
        finally
        {
            lvPreview.EndUpdate();
        }

        ConfigurePreviewColumnsForScrolling();
    }

    private async Task ExecuteAsync()
    {
        if (_currentPlan.Count == 0)
        {
            var scanOk = await ScanAsync();
            if (!scanOk) return;
        }

        SyncPreviewSelectionToPlan();
        var selectedPlan = GetSelectedPlan();

        if (selectedPlan.Count == 0)
        {
            MessageBox.Show("Δεν έχουν επιλεγεί αρχεία για εκτέλεση. Τσέκαρε αρχεία ή χρησιμοποίησε τα κουμπιά επιλογής στο preview.", "Πληροφορία", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        var mode = GetTransferMode();
        if (mode == TransferMode.Move)
        {
            var confirm = MessageBox.Show(
                "Έχεις επιλέξει ΜΕΤΑΦΟΡΑ. Τα αρχικά αρχεία θα διαγραφούν μόνο αφού αντιγραφούν και ελεγχθούν.\n\nΣυνέχεια;",
                "Επιβεβαίωση μεταφοράς",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);
            if (confirm != DialogResult.Yes) return;
        }

        var totalBytes = selectedPlan.Sum(x => x.SizeBytes);
        var availableBytes = FileTransferService.TryGetAvailableBytes(txtDestinationRoot.Text.Trim());
        if (availableBytes.HasValue && availableBytes.Value < totalBytes)
        {
            MessageBox.Show(
                $"Δεν φαίνεται να υπάρχει αρκετός χώρος.\n\nΑπαιτούνται: {FileTransferService.HumanBytes(totalBytes)}\nΔιαθέσιμα: {FileTransferService.HumanBytes(availableBytes.Value)}",
                "Χώρος δίσκου",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
            return;
        }

        SetBusy(true);
        _cts = new CancellationTokenSource();
        progressBar.Maximum = Math.Max(selectedPlan.Count, 1);
        progressBar.Value = 0;

        try
        {
            Directory.CreateDirectory(GetPcDestinationRoot());
            var progress = new Progress<TransferProgress>(p =>
            {
                progressBar.Value = Math.Min(p.Current, progressBar.Maximum);
                lblSummary.Text = $"{p.Current}/{p.Total} - {p.Status} - {p.CurrentFile}";
            });

            var result = await FileTransferService.ExecuteAsync(selectedPlan, mode, progress, _cts.Token);
            _lastLogPath = FileTransferService.WriteCsvLog(txtDestinationRoot.Text.Trim(), GetComputerFolderName(), result);
            _lastTransferResult = result.Operations.Count > 0 ? result : null;

            string? maintenanceLogPath = null;
            if (result.Canceled)
            {
                WriteStatus("Maintenance: παραλείφθηκε επειδή η διαδικασία σταμάτησε με Stop.");
            }
            else if (mode != TransferMode.ScanOnly)
            {
                maintenanceLogPath = await RunMaintenanceIfRequestedAsync(result, _cts.Token);
            }
            else if (_settings.RunMaintenanceAfterTransfer && MaintenanceService.HasAny(GetCleanupOptions()))
            {
                WriteStatus("Maintenance: παραλείφθηκε επειδή η ενέργεια είναι Μόνο σάρωση/preview.");
            }

            var finishedText = result.Canceled ? "Σταμάτησε" : "Ολοκληρώθηκε";
            WriteStatus($"{finishedText}. Copied: {result.Copied}, Moved: {result.Moved}, Skipped: {result.Skipped}, Failed: {result.Failed}. Log: {_lastLogPath}");
            var maintenanceText = string.IsNullOrWhiteSpace(maintenanceLogPath) ? string.Empty : $"\nMaintenance log:\n{maintenanceLogPath}";
            var undoText = _lastTransferResult is null ? string.Empty : "\n\nΜπορείς να πατήσεις 'Undo τελευταίας' για αναίρεση των αρχείων που πρόλαβαν να περάσουν.";
            MessageBox.Show($"{finishedText}.\n\nCopied: {result.Copied}\nMoved: {result.Moved}\nSkipped: {result.Skipped}\nFailed: {result.Failed}\nCanceled: {result.Canceled}\n\nTransfer log:\n{_lastLogPath}{maintenanceText}{undoText}", "Τέλος", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (OperationCanceledException)
        {
            WriteStatus("Η διαδικασία ακυρώθηκε από τον χρήστη.");
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Σφάλμα", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            SetBusy(false);
            _cts?.Dispose();
            _cts = null;
            UpdateSummary();
        }
    }

    private async Task UndoLastTransferAsync()
    {
        if (_lastTransferResult is null || _lastTransferResult.Operations.Count == 0)
        {
            MessageBox.Show("Δεν υπάρχει διαθέσιμη τελευταία αντιγραφή/μεταφορά για αναίρεση.", "Undo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        var copied = _lastTransferResult.Operations.Count(x => string.Equals(x.Action, "COPIED", StringComparison.OrdinalIgnoreCase));
        var moved = _lastTransferResult.Operations.Count(x => string.Equals(x.Action, "MOVED", StringComparison.OrdinalIgnoreCase));
        var confirm = MessageBox.Show(
            $"Θα γίνει αναίρεση της τελευταίας εκτέλεσης.\n\nΑντιγραμμένα αρχεία που θα διαγραφούν από τον προορισμό: {copied}\nΜεταφερμένα αρχεία που θα επιστρέψουν στην αρχική θέση: {moved}\n\nΔεν αναιρούνται Temp cleanup, άδειασμα κάδου, DISM ή Optimize.\n\nΣυνέχεια;",
            "Αναίρεση τελευταίας εκτέλεσης",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Warning);

        if (confirm != DialogResult.Yes)
            return;

        SetBusy(true);
        _cts = new CancellationTokenSource();
        progressBar.Maximum = Math.Max(_lastTransferResult.Operations.Count, 1);
        progressBar.Value = 0;

        try
        {
            var progress = new Progress<TransferProgress>(p =>
            {
                progressBar.Value = Math.Min(p.Current, progressBar.Maximum);
                lblSummary.Text = $"Undo {p.Current}/{p.Total} - {p.CurrentFile}";
            });

            var undo = await FileTransferService.UndoAsync(_lastTransferResult.Operations, progress, _cts.Token);
            _lastUndoLogPath = FileTransferService.WriteUndoCsvLog(txtDestinationRoot.Text.Trim(), GetComputerFolderName(), undo);
            _lastLogPath = _lastUndoLogPath;

            var undoFinishedText = undo.Canceled ? "Το Undo σταμάτησε" : "Η αναίρεση ολοκληρώθηκε";
            WriteStatus($"{undoFinishedText}. Deleted copied: {undo.DeletedCopiedFiles}, moved back: {undo.MovedBackFiles}, skipped: {undo.Skipped}, failed: {undo.Failed}. Log: {_lastUndoLogPath}");
            MessageBox.Show($"{undoFinishedText}.\n\nΔιαγράφηκαν αντιγραμμένα από προορισμό: {undo.DeletedCopiedFiles}\nΕπιστράφηκαν μεταφερμένα: {undo.MovedBackFiles}\nSkipped: {undo.Skipped}\nFailed: {undo.Failed}\nCanceled: {undo.Canceled}\n\nUndo log:\n{_lastUndoLogPath}", "Undo", MessageBoxButtons.OK, MessageBoxIcon.Information);

            if (!undo.Canceled && undo.Failed == 0)
                _lastTransferResult = null;
        }
        catch (OperationCanceledException)
        {
            WriteStatus("Το Undo ακυρώθηκε από τον χρήστη. Ό,τι είχε ήδη αναιρεθεί παραμένει αναιρεμένο.");
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Σφάλμα Undo", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            SetBusy(false);
            _cts?.Dispose();
            _cts = null;
            UpdateSummary();
        }
    }

    private async Task<string?> RunMaintenanceIfRequestedAsync(TransferResult transferResult, CancellationToken cancellationToken)
    {
        if (!_settings.RunMaintenanceAfterTransfer)
            return null;

        var options = GetCleanupOptions();
        if (!MaintenanceService.HasAny(options))
            return null;

        if (options.CleanDownloadsPlannedFiles && transferResult.Failed > 0)
        {
            WriteStatus("Maintenance: το καθάρισμα Downloads παραλείφθηκε επειδή υπάρχουν αποτυχημένες μεταφορές/αντιγραφές.");
            options.CleanDownloadsPlannedFiles = false;
        }

        if (!MaintenanceService.HasAny(options))
            return null;

        var warning = BuildMaintenanceWarning(options);
        var confirm = MessageBox.Show(
            warning,
            "Επιβεβαίωση καθαρισμού",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Warning);

        if (confirm != DialogResult.Yes)
        {
            WriteStatus("Maintenance: ακυρώθηκε από τον χρήστη.");
            return null;
        }

        var cleanupResult = await MaintenanceService.ExecuteAsync(
            options,
            txtSourceUserRoot.Text.Trim(),
            transferResult.SuccessfulSourcePaths,
            WriteStatus,
            cancellationToken);

        var maintenanceLogPath = MaintenanceService.WriteMaintenanceLog(
            txtDestinationRoot.Text.Trim(),
            GetComputerFolderName(),
            cleanupResult);

        WriteStatus($"Maintenance τέλος. Deleted files: {cleanupResult.DeletedFiles}, dirs: {cleanupResult.DeletedDirectories}, failed: {cleanupResult.FailedItems}, freed approx: {FileTransferService.HumanBytes(cleanupResult.ReclaimedBytes)}. Log: {maintenanceLogPath}");
        return maintenanceLogPath;
    }

    private CleanupOptions GetCleanupOptions()
    {
        return new CleanupOptions
        {
            CleanUserTemp = _settings.CleanUserTemp,
            CleanWindowsTemp = _settings.CleanWindowsTemp,
            CleanDownloadsPlannedFiles = _settings.CleanDownloadsPlannedFiles,
            EmptyRecycleBin = _settings.EmptyRecycleBin,
            OptimizeSystemDrive = _settings.OptimizeSystemDrive,
            CleanWindowsUpdateComponents = _settings.CleanWindowsUpdateComponents
        };
    }

    private static string BuildMaintenanceWarning(CleanupOptions options)
    {
        var lines = new List<string>
        {
            "Έχεις ενεργοποιήσει καθάρισμα μετά τη διαδικασία.",
            "",
            "Θα εκτελεστούν:"
        };

        if (options.CleanUserTemp)
            lines.Add("• Καθάρισμα Temp του επιλεγμένου προφίλ χρήστη.");
        if (options.CleanWindowsTemp)
            lines.Add("• Καθάρισμα Windows Temp. Κλειδωμένα/προστατευμένα αρχεία θα παραλειφθούν.");
        if (options.CleanDownloadsPlannedFiles)
            lines.Add("• Καθάρισμα Downloads ΜΟΝΟ για αρχεία που αντιγράφηκαν/μεταφέρθηκαν επιτυχώς από το τρέχον plan.");
        if (options.EmptyRecycleBin)
            lines.Add("• Άδειασμα Κάδου Ανακύκλωσης.");
        if (options.OptimizeSystemDrive)
            lines.Add("• Windows Optimize στον δίσκο συστήματος: SSD=TRIM, HDD=defrag/optimize.");
        if (options.CleanWindowsUpdateComponents)
            lines.Add("• Windows Update Cleanup μέσω DISM /StartComponentCleanup. Θέλει Run as administrator και μπορεί να πάρει αρκετή ώρα.");

        lines.Add("");
        lines.Add("Συνέχεια;");
        return string.Join(Environment.NewLine, lines);
    }

    private TransferMode GetTransferMode()
    {
        if (rbScanOnly.Checked) return TransferMode.ScanOnly;
        if (rbMove.Checked) return TransferMode.Move;
        return TransferMode.Copy;
    }

    private static IEnumerable<string> SplitExtensions(string text)
    {
        return text.Split([';', ',', ' ', '\r', '\n', '\t'], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
    }

    private static string EnsureRequiredExtensions(string text, params string[] requiredExtensions)
    {
        var parts = SplitExtensions(text ?? string.Empty)
            .Select(NormalizeExtensionText)
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        foreach (var required in requiredExtensions.Select(NormalizeExtensionText))
        {
            if (!parts.Contains(required, StringComparer.OrdinalIgnoreCase))
                parts.Add(required);
        }

        return string.Join(";", parts);
    }

    private static string NormalizeExtensionText(string extension)
    {
        extension = (extension ?? string.Empty).Trim();
        if (string.IsNullOrWhiteSpace(extension))
            return string.Empty;

        return extension.StartsWith('.') ? extension : $".{extension}";
    }

    private string GetComputerFolderName()
    {
        var name = string.IsNullOrWhiteSpace(txtComputerName.Text) ? GetSanitizedMachineName() : txtComputerName.Text.Trim();
        foreach (var c in Path.GetInvalidFileNameChars())
            name = name.Replace(c, '_');
        name = name.Trim().TrimEnd('.');
        return string.IsNullOrWhiteSpace(name) ? "PC" : name;
    }

    private string GetPcDestinationRoot()
    {
        return Path.Combine(txtDestinationRoot.Text.Trim(), GetComputerFolderName());
    }

    private void UpdateFinalPathLabel()
    {
        lblFinalPath.Text = GetPcDestinationRoot();
    }

    private void UpdateSummary()
    {
        var selected = _currentPlan.Where(x => x.IsSelected).ToList();
        var totalBytes = selected.Sum(x => x.SizeBytes);
        var looseCount = selected.Count(x => x.IsLooseFile);
        var folderCount = selected.Count - looseCount;
        var free = FileTransferService.TryGetAvailableBytes(txtDestinationRoot.Text.Trim());
        var freeText = free.HasValue ? FileTransferService.HumanBytes(free.Value) : "άγνωστο";
        lblSummary.Text = $"Επιλεγμένα: {selected.Count}/{_currentPlan.Count} | Χύμα: {looseCount} | Σε φακέλους: {folderCount} | Μέγεθος: {FileTransferService.HumanBytes(totalBytes)} | Τελικός φάκελος: {GetPcDestinationRoot()} | Ελεύθερος χώρος: {freeText}";
    }

    private void WriteStatus(string message)
    {
        if (InvokeRequired)
        {
            BeginInvoke(() => WriteStatus(message));
            return;
        }
        txtStatus.AppendText($"[{DateTime.Now:HH:mm:ss}] {message}{Environment.NewLine}");
    }

    private void SetBusy(bool busy)
    {
        btnScan.Enabled = !busy;
        btnExecute.Enabled = !busy;
        btnCancel.Enabled = busy;
        btnUndoLast.Enabled = !busy && _lastTransferResult is not null && _lastTransferResult.Operations.Count > 0;
        btnSaveSettings.Enabled = !busy;
        btnDetectPc.Enabled = !busy;
        btnBrowseDestination.Enabled = !busy;
        btnTestDestination.Enabled = !busy;
        btnApplySourceUserRoot.Enabled = !busy;
        btnAddSource.Enabled = !busy;
        btnRemoveSource.Enabled = !busy;
        btnMaintenance.Enabled = !busy;
        btnSelectAllPreview.Enabled = !busy;
        btnClearPreviewSelection.Enabled = !busy;
        btnSelectFolderPreview.Enabled = !busy;
        btnClearFolderPreview.Enabled = !busy;
        chkCleanUserTemp.Enabled = !busy;
        chkCleanWindowsTemp.Enabled = !busy;
        chkCleanDownloadsPlanned.Enabled = !busy;
        chkEmptyRecycleBin.Enabled = !busy;
        chkOptimizeDrive.Enabled = !busy;
        chkCleanWindowsUpdateComponents.Enabled = !busy;
        rbAllFiles.Enabled = !busy;
        rbSelectedExtensions.Enabled = !busy;
        txtExtensions.Enabled = !busy && rbSelectedExtensions.Checked;
        cboTheme.Enabled = !busy;
    }

    private void OpenPath(string path)
    {
        try
        {
            if (File.Exists(path))
            {
                Process.Start(new ProcessStartInfo("explorer.exe", $"/select,\"{path}\"") { UseShellExecute = true });
            }
            else
            {
                Directory.CreateDirectory(path);
                Process.Start(new ProcessStartInfo(path) { UseShellExecute = true });
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Σφάλμα", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
