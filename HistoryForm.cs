using System.Diagnostics;
using System.Globalization;
using System.Text;

namespace LabFileCollector;

public sealed class HistoryForm : Form
{
    private readonly string _destinationRoot;
    private readonly bool _darkTheme;
    private readonly ListView _logs = new();
    private readonly ListView _details = new();
    private readonly TextBox _selectedPath = new();
    private readonly Button _btnRefresh = new();
    private readonly Button _btnOpen = new();
    private readonly Button _btnOpenFolder = new();
    private readonly Button _btnClose = new();
    private readonly Label _summary = new();

    private sealed record LogEntry(string Path, string Type, DateTime Modified, long SizeBytes);

    public HistoryForm(string destinationRoot, bool darkTheme)
    {
        _destinationRoot = destinationRoot;
        _darkTheme = darkTheme;

        Text = "Ιστορικό ενεργειών";
        StartPosition = FormStartPosition.CenterParent;
        MinimumSize = new Size(980, 620);
        Size = new Size(1120, 720);

        var icon = AppVisuals.LoadAppIcon();
        if (icon is not null)
            Icon = icon;

        BuildUi();
        ApplyTheme();
        LoadLogs();
    }

    private void BuildUi()
    {
        var root = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 4,
            Padding = new Padding(14)
        };
        root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        root.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        Controls.Add(root);

        var title = new Label
        {
            Text = "Ιστορικό αντιγραφών / μεταφορών / συντήρησης",
            AutoSize = true,
            Font = new Font("Segoe UI", 14F, FontStyle.Bold),
            Margin = new Padding(0, 0, 0, 10)
        };
        root.Controls.Add(title, 0, 0);

        var split = new SplitContainer
        {
            Dock = DockStyle.Fill,
            SplitterDistance = 360
        };
        root.Controls.Add(split, 0, 1);

        _logs.Dock = DockStyle.Fill;
        _logs.View = View.Details;
        _logs.FullRowSelect = true;
        _logs.GridLines = true;
        _logs.Columns.Add("Log", 190);
        _logs.Columns.Add("Τύπος", 90);
        _logs.Columns.Add("Ημερομηνία", 140);
        _logs.Columns.Add("Μέγεθος", 90);
        _logs.SelectedIndexChanged += (_, _) => LoadSelectedLogDetails();
        split.Panel1.Controls.Add(_logs);

        _details.Dock = DockStyle.Fill;
        _details.View = View.Details;
        _details.FullRowSelect = true;
        _details.GridLines = true;
        split.Panel2.Controls.Add(_details);

        _summary.AutoSize = true;
        _summary.Margin = new Padding(0, 10, 0, 6);
        root.Controls.Add(_summary, 0, 2);

        var bottom = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 5,
            RowCount = 1,
            AutoSize = true
        };
        bottom.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        bottom.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        bottom.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        bottom.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        bottom.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        root.Controls.Add(bottom, 0, 3);

        _selectedPath.Dock = DockStyle.Fill;
        _selectedPath.ReadOnly = true;
        _selectedPath.Font = new Font("Consolas", 9F);
        bottom.Controls.Add(_selectedPath, 0, 0);

        _btnRefresh.Text = "Ανανέωση";
        _btnRefresh.AutoSize = true;
        _btnRefresh.Click += (_, _) => LoadLogs();
        bottom.Controls.Add(_btnRefresh, 1, 0);

        _btnOpen.Text = "Άνοιγμα log";
        _btnOpen.AutoSize = true;
        _btnOpen.Click += (_, _) => OpenSelectedLog();
        bottom.Controls.Add(_btnOpen, 2, 0);

        _btnOpenFolder.Text = "Άνοιγμα φακέλου _logs";
        _btnOpenFolder.AutoSize = true;
        _btnOpenFolder.Click += (_, _) => OpenLogsFolder();
        bottom.Controls.Add(_btnOpenFolder, 3, 0);

        _btnClose.Text = "Κλείσιμο";
        _btnClose.AutoSize = true;
        _btnClose.Click += (_, _) => Close();
        bottom.Controls.Add(_btnClose, 4, 0);
    }

    private void LoadLogs()
    {
        _logs.Items.Clear();
        _details.Items.Clear();
        _selectedPath.Clear();

        var logsRoot = Path.Combine(_destinationRoot, "_logs");
        if (string.IsNullOrWhiteSpace(_destinationRoot) || !Directory.Exists(logsRoot))
        {
            _summary.Text = $"Δεν βρέθηκε φάκελος ιστορικού: {logsRoot}";
            return;
        }

        var entries = Directory.EnumerateFiles(logsRoot, "*.csv", SearchOption.TopDirectoryOnly)
            .Select(path =>
            {
                var info = new FileInfo(path);
                return new LogEntry(path, GetLogType(info.Name), info.LastWriteTime, info.Length);
            })
            .OrderByDescending(x => x.Modified)
            .ToList();

        foreach (var entry in entries)
        {
            var item = new ListViewItem(System.IO.Path.GetFileName(entry.Path)) { Tag = entry };
            item.SubItems.Add(entry.Type);
            item.SubItems.Add(entry.Modified.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.CurrentCulture));
            item.SubItems.Add(FileTransferService.HumanBytes(entry.SizeBytes));
            _logs.Items.Add(item);
        }

        _summary.Text = $"Βρέθηκαν {entries.Count} log αρχεία στο {logsRoot}";
        if (_logs.Items.Count > 0)
            _logs.Items[0].Selected = true;
    }

    private static string GetLogType(string fileName)
    {
        if (fileName.StartsWith("transfer-log", StringComparison.OrdinalIgnoreCase)) return "Transfer";
        if (fileName.StartsWith("undo-log", StringComparison.OrdinalIgnoreCase)) return "Undo";
        if (fileName.StartsWith("maintenance-log", StringComparison.OrdinalIgnoreCase)) return "Maintenance";
        return "Log";
    }

    private void LoadSelectedLogDetails()
    {
        if (_logs.SelectedItems.Count == 0 || _logs.SelectedItems[0].Tag is not LogEntry entry)
            return;

        _selectedPath.Text = entry.Path;
        _details.BeginUpdate();
        try
        {
            _details.Items.Clear();
            _details.Columns.Clear();

            var lines = File.ReadAllLines(entry.Path, Encoding.UTF8);
            if (lines.Length == 0)
                return;

            var headers = SplitCsvLine(lines[0]);
            foreach (var header in headers)
                _details.Columns.Add(string.IsNullOrWhiteSpace(header) ? "—" : header, 150);

            foreach (var line in lines.Skip(1).Take(2000))
            {
                var cells = SplitCsvLine(line);
                if (cells.Count == 0) continue;

                var item = new ListViewItem(cells[0]);
                for (var i = 1; i < headers.Count; i++)
                    item.SubItems.Add(i < cells.Count ? cells[i] : string.Empty);

                _details.Items.Add(item);
            }

            AutoSizeDetailColumns();
            _summary.Text = $"{System.IO.Path.GetFileName(entry.Path)} | Γραμμές: {Math.Max(0, lines.Length - 1)}" +
                            (lines.Length > 2001 ? " | Προβάλλονται οι πρώτες 2000." : string.Empty);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Δεν μπορώ να διαβάσω το log.\n\n{ex.Message}", "Ιστορικό", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            _details.EndUpdate();
        }
    }

    private static List<string> SplitCsvLine(string line)
    {
        // The app writes simple semicolon-separated logs. This parser is intentionally small
        // and tolerant enough for paths/status text without introducing extra dependencies.
        return line.Split(';').Select(x => x.Trim()).ToList();
    }

    private void AutoSizeDetailColumns()
    {
        foreach (ColumnHeader column in _details.Columns)
        {
            column.Width = -2;
            if (column.Width < 120) column.Width = 120;
            if (column.Width > 420) column.Width = 420;
        }
    }

    private void OpenSelectedLog()
    {
        if (_logs.SelectedItems.Count == 0 || _logs.SelectedItems[0].Tag is not LogEntry entry)
        {
            MessageBox.Show("Δεν έχει επιλεγεί log.", "Ιστορικό", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        OpenPath(entry.Path);
    }

    private void OpenLogsFolder()
    {
        var logsRoot = Path.Combine(_destinationRoot, "_logs");
        if (!Directory.Exists(logsRoot))
        {
            MessageBox.Show("Δεν βρέθηκε ο φάκελος _logs.", "Ιστορικό", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        OpenPath(logsRoot);
    }

    private static void OpenPath(string path)
    {
        try
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = path,
                UseShellExecute = true
            });
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Άνοιγμα", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void ApplyTheme()
    {
        var back = _darkTheme ? Color.FromArgb(30, 34, 39) : Color.FromArgb(246, 248, 250);
        var card = _darkTheme ? Color.FromArgb(39, 44, 52) : Color.White;
        var text = _darkTheme ? Color.FromArgb(236, 239, 244) : Color.FromArgb(30, 35, 42);
        var grid = _darkTheme ? Color.FromArgb(72, 78, 88) : Color.FromArgb(210, 216, 224);

        BackColor = back;
        ForeColor = text;

        foreach (Control control in Controls)
            ApplyThemeRecursive(control, back, card, text, grid);
    }

    private static void ApplyThemeRecursive(Control control, Color back, Color card, Color text, Color grid)
    {
        switch (control)
        {
            case ListView listView:
                listView.BackColor = card;
                listView.ForeColor = text;
                break;
            case TextBox textBox:
                textBox.BackColor = card;
                textBox.ForeColor = text;
                break;
            case Button button:
                button.FlatStyle = FlatStyle.Flat;
                button.BackColor = card;
                button.ForeColor = text;
                button.FlatAppearance.BorderColor = grid;
                break;
            default:
                control.BackColor = back;
                control.ForeColor = text;
                break;
        }

        foreach (Control child in control.Controls)
            ApplyThemeRecursive(child, back, card, text, grid);
    }
}
