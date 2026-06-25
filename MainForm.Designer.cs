using System.Drawing;
using System.Windows.Forms;

namespace LabFileCollector;

partial class MainForm
{
    private System.ComponentModel.IContainer components = null!;

    private TableLayoutPanel rootLayout;
    private FlowLayoutPanel titleLayout;
    private PictureBox picLogo;
    private Label lblTitle;
    private GroupBox grpDestination;
    private TableLayoutPanel destinationLayout;
    private Label lblPc;
    private ComboBox cboPc;
    private Button btnDetectPc;
    private Label lblDestinationRoot;
    private TextBox txtDestinationRoot;
    private Button btnBrowseDestination;
    private Button btnTestDestination;
    private Button btnSaveSettings;
    private Label lblFinalPathCaption;
    private Label lblFinalPath;
    private SplitContainer splitOptions;
    private GroupBox grpSources;
    private TableLayoutPanel sourcesLayout;
    private Label lblSourceUserRoot;
    private TextBox txtSourceUserRoot;
    private Button btnApplySourceUserRoot;
    private CheckedListBox clbSources;
    private TextBox txtCustomSource;
    private Button btnAddSource;
    private Button btnRemoveSource;
    private TableLayoutPanel rightOptionsLayout;
    private GroupBox grpFileTypes;
    private TableLayoutPanel fileTypesLayout;
    private RadioButton rbAllFiles;
    private RadioButton rbSelectedExtensions;
    private TextBox txtExtensions;
    private GroupBox grpAction;
    private FlowLayoutPanel actionLayout;
    private RadioButton rbScanOnly;
    private RadioButton rbCopy;
    private RadioButton rbMove;
    private GroupBox grpMaintenance;
    private TableLayoutPanel maintenanceLayout;
    private CheckBox chkCleanUserTemp;
    private CheckBox chkCleanWindowsTemp;
    private CheckBox chkCleanDownloadsPlanned;
    private CheckBox chkEmptyRecycleBin;
    private CheckBox chkOptimizeDrive;
    private CheckBox chkCleanWindowsUpdateComponents;
    private Label lblMaintenanceNote;
    private GroupBox grpPreview;
    private TableLayoutPanel previewLayout;
    private ListView lvPreview;
    private ColumnHeader colFile;
    private ColumnHeader colPlacement;
    private ColumnHeader colFolder;
    private ColumnHeader colSize;
    private ColumnHeader colSource;
    private ColumnHeader colDestination;
    private Label lblSummary;
    private TableLayoutPanel bottomLayout;
    private FlowLayoutPanel buttonsLayout;
    private Button btnScan;
    private Button btnExecute;
    private Button btnCancel;
    private Button btnUndoLast;
    private Button btnOpenDestination;
    private Button btnOpenLog;
    private Button btnHelp;
    private Button btnAbout;
    private Label lblTheme;
    private ComboBox cboTheme;
    private ProgressBar progressBar;
    private TextBox txtStatus;
    private FolderBrowserDialog folderBrowserDialog;

    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();
        rootLayout = new TableLayoutPanel();
        titleLayout = new FlowLayoutPanel();
        picLogo = new PictureBox();
        lblTitle = new Label();
        grpDestination = new GroupBox();
        destinationLayout = new TableLayoutPanel();
        lblPc = new Label();
        cboPc = new ComboBox();
        btnDetectPc = new ModernButton();
        lblDestinationRoot = new Label();
        txtDestinationRoot = new TextBox();
        btnBrowseDestination = new ModernButton();
        btnTestDestination = new ModernButton();
        btnSaveSettings = new ModernButton();
        lblFinalPathCaption = new Label();
        lblFinalPath = new Label();
        splitOptions = new SplitContainer();
        grpSources = new GroupBox();
        sourcesLayout = new TableLayoutPanel();
        lblSourceUserRoot = new Label();
        txtSourceUserRoot = new TextBox();
        btnApplySourceUserRoot = new ModernButton();
        clbSources = new CheckedListBox();
        txtCustomSource = new TextBox();
        btnAddSource = new ModernButton();
        btnRemoveSource = new ModernButton();
        rightOptionsLayout = new TableLayoutPanel();
        grpFileTypes = new GroupBox();
        fileTypesLayout = new TableLayoutPanel();
        rbAllFiles = new RadioButton();
        rbSelectedExtensions = new RadioButton();
        txtExtensions = new TextBox();
        grpAction = new GroupBox();
        actionLayout = new FlowLayoutPanel();
        rbScanOnly = new RadioButton();
        rbCopy = new RadioButton();
        rbMove = new RadioButton();
        grpMaintenance = new GroupBox();
        maintenanceLayout = new TableLayoutPanel();
        chkCleanUserTemp = new CheckBox();
        chkCleanWindowsTemp = new CheckBox();
        chkCleanDownloadsPlanned = new CheckBox();
        chkEmptyRecycleBin = new CheckBox();
        chkOptimizeDrive = new CheckBox();
        chkCleanWindowsUpdateComponents = new CheckBox();
        lblMaintenanceNote = new Label();
        grpPreview = new GroupBox();
        previewLayout = new TableLayoutPanel();
        lvPreview = new ListView();
        colFile = new ColumnHeader();
        colPlacement = new ColumnHeader();
        colFolder = new ColumnHeader();
        colSize = new ColumnHeader();
        colSource = new ColumnHeader();
        colDestination = new ColumnHeader();
        lblSummary = new Label();
        bottomLayout = new TableLayoutPanel();
        buttonsLayout = new FlowLayoutPanel();
        btnScan = new ModernButton();
        btnExecute = new ModernButton();
        btnCancel = new ModernButton();
        btnUndoLast = new ModernButton();
        btnOpenDestination = new ModernButton();
        btnOpenLog = new ModernButton();
        btnHelp = new ModernButton();
        btnAbout = new ModernButton();
        lblTheme = new Label();
        cboTheme = new ComboBox();
        progressBar = new ProgressBar();
        txtStatus = new TextBox();
        folderBrowserDialog = new FolderBrowserDialog();
        rootLayout.SuspendLayout();
        titleLayout.SuspendLayout();
        grpDestination.SuspendLayout();
        destinationLayout.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)splitOptions).BeginInit();
        splitOptions.Panel1.SuspendLayout();
        splitOptions.Panel2.SuspendLayout();
        splitOptions.SuspendLayout();
        grpSources.SuspendLayout();
        sourcesLayout.SuspendLayout();
        rightOptionsLayout.SuspendLayout();
        grpFileTypes.SuspendLayout();
        fileTypesLayout.SuspendLayout();
        grpAction.SuspendLayout();
        actionLayout.SuspendLayout();
        grpMaintenance.SuspendLayout();
        maintenanceLayout.SuspendLayout();
        grpPreview.SuspendLayout();
        previewLayout.SuspendLayout();
        bottomLayout.SuspendLayout();
        buttonsLayout.SuspendLayout();
        SuspendLayout();
        // 
        // rootLayout
        // 
        rootLayout.ColumnCount = 1;
        rootLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        rootLayout.Controls.Add(titleLayout, 0, 0);
        rootLayout.Controls.Add(grpDestination, 0, 1);
        rootLayout.Controls.Add(splitOptions, 0, 2);
        rootLayout.Controls.Add(grpMaintenance, 0, 3);
        rootLayout.Controls.Add(grpPreview, 0, 4);
        rootLayout.Controls.Add(bottomLayout, 0, 5);
        rootLayout.Dock = DockStyle.Fill;
        rootLayout.Location = new Point(0, 0);
        rootLayout.Name = "rootLayout";
        rootLayout.Padding = new Padding(12);
        rootLayout.RowCount = 6;
        rootLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        rootLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        rootLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 275F));
        rootLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 170F));
        rootLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        rootLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        rootLayout.Size = new Size(1224, 930);
        rootLayout.TabIndex = 0;
        // 
        // titleLayout
        // 
        titleLayout.AutoSize = true;
        titleLayout.Controls.Add(picLogo);
        titleLayout.Controls.Add(lblTitle);
        titleLayout.Dock = DockStyle.Fill;
        titleLayout.Location = new Point(15, 15);
        titleLayout.Name = "titleLayout";
        titleLayout.Size = new Size(1194, 55);
        titleLayout.TabIndex = 0;
        titleLayout.WrapContents = false;
        // 
        // picLogo
        // 
        picLogo.Location = new Point(3, 3);
        picLogo.Name = "picLogo";
        picLogo.Size = new Size(44, 44);
        picLogo.SizeMode = PictureBoxSizeMode.Zoom;
        picLogo.TabIndex = 0;
        picLogo.TabStop = false;
        // 
        // lblTitle
        // 
        lblTitle.AutoSize = true;
        lblTitle.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
        lblTitle.Location = new Point(56, 0);
        lblTitle.Margin = new Padding(6, 0, 3, 0);
        lblTitle.Name = "lblTitle";
        lblTitle.Padding = new Padding(0, 4, 0, 8);
        lblTitle.Size = new Size(313, 53);
        lblTitle.TabIndex = 1;
        lblTitle.Text = "School Lab File Collector";
        // 
        // grpDestination
        // 
        grpDestination.AutoSize = true;
        grpDestination.Controls.Add(destinationLayout);
        grpDestination.Dock = DockStyle.Fill;
        grpDestination.Location = new Point(15, 64);
        grpDestination.Name = "grpDestination";
        grpDestination.Padding = new Padding(10);
        grpDestination.Size = new Size(1194, 128);
        grpDestination.TabIndex = 1;
        grpDestination.TabStop = false;
        grpDestination.Text = "Προορισμός / Υπολογιστής";
        // 
        // destinationLayout
        // 
        destinationLayout.AutoSize = true;
        destinationLayout.ColumnCount = 5;
        destinationLayout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        destinationLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        destinationLayout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        destinationLayout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        destinationLayout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        destinationLayout.Controls.Add(lblPc, 0, 0);
        destinationLayout.Controls.Add(cboPc, 1, 0);
        destinationLayout.Controls.Add(btnDetectPc, 2, 0);
        destinationLayout.Controls.Add(btnSaveSettings, 4, 0);
        destinationLayout.Controls.Add(lblDestinationRoot, 0, 1);
        destinationLayout.Controls.Add(txtDestinationRoot, 1, 1);
        destinationLayout.Controls.Add(btnBrowseDestination, 2, 1);
        destinationLayout.Controls.Add(btnTestDestination, 4, 1);
        destinationLayout.Controls.Add(lblFinalPathCaption, 0, 2);
        destinationLayout.Controls.Add(lblFinalPath, 1, 2);
        destinationLayout.Dock = DockStyle.Fill;
        destinationLayout.Location = new Point(10, 30);
        destinationLayout.Name = "destinationLayout";
        destinationLayout.RowCount = 3;
        destinationLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        destinationLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        destinationLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        destinationLayout.Size = new Size(1174, 88);
        destinationLayout.TabIndex = 0;
        // 
        // lblPc
        // 
        lblPc.AutoSize = true;
        lblPc.Dock = DockStyle.Fill;
        lblPc.Location = new Point(3, 0);
        lblPc.Name = "lblPc";
        lblPc.Padding = new Padding(0, 6, 8, 0);
        lblPc.Size = new Size(126, 35);
        lblPc.TabIndex = 0;
        lblPc.Text = "Όνομα υπολογιστή:";
        // 
        // cboPc
        // 
        cboPc.Dock = DockStyle.Left;
        cboPc.DropDownStyle = ComboBoxStyle.DropDown;
        cboPc.FormattingEnabled = true;
        cboPc.Location = new Point(135, 3);
        cboPc.Name = "cboPc";
        cboPc.Size = new Size(260, 28);
        cboPc.TabIndex = 1;
        cboPc.SelectedIndexChanged += cboPc_SelectedIndexChanged;
        cboPc.TextChanged += cboPc_TextChanged;
        // 
        // btnDetectPc
        // 
        btnDetectPc.AutoSize = true;
        btnDetectPc.Location = new Point(295, 3);
        btnDetectPc.Name = "btnDetectPc";
        btnDetectPc.Size = new Size(96, 29);
        btnDetectPc.TabIndex = 2;
        btnDetectPc.Text = "Από Windows";
        btnDetectPc.UseVisualStyleBackColor = true;
        btnDetectPc.Click += btnDetectPc_Click;
        // 
        // lblDestinationRoot
        // 
        lblDestinationRoot.AutoSize = true;
        lblDestinationRoot.Dock = DockStyle.Fill;
        lblDestinationRoot.Location = new Point(3, 35);
        lblDestinationRoot.Name = "lblDestinationRoot";
        lblDestinationRoot.Padding = new Padding(0, 8, 8, 0);
        lblDestinationRoot.Size = new Size(126, 35);
        lblDestinationRoot.TabIndex = 3;
        lblDestinationRoot.Text = "Root path:";
        // 
        // txtDestinationRoot
        // 
        txtDestinationRoot.Dock = DockStyle.Fill;
        txtDestinationRoot.Location = new Point(135, 38);
        txtDestinationRoot.Name = "txtDestinationRoot";
        txtDestinationRoot.PlaceholderText = @"\\192.168.1.37\2026 ή \\192.168.1.37\Backup\2026";
        txtDestinationRoot.Size = new Size(760, 27);
        txtDestinationRoot.TabIndex = 4;
        txtDestinationRoot.TextChanged += txtDestinationRoot_TextChanged;
        // 
        // btnBrowseDestination
        // 
        btnBrowseDestination.AutoSize = true;
        btnBrowseDestination.Location = new Point(295, 38);
        btnBrowseDestination.Name = "btnBrowseDestination";
        btnBrowseDestination.Size = new Size(96, 29);
        btnBrowseDestination.TabIndex = 5;
        btnBrowseDestination.Text = "Αλλαγή...";
        btnBrowseDestination.UseVisualStyleBackColor = true;
        btnBrowseDestination.Click += btnBrowseDestination_Click;
        // 
        // btnTestDestination
        // 
        btnTestDestination.AutoSize = true;
        btnTestDestination.Location = new Point(1064, 38);
        btnTestDestination.Name = "btnTestDestination";
        btnTestDestination.Size = new Size(107, 29);
        btnTestDestination.TabIndex = 6;
        btnTestDestination.Text = "Έλεγχος";
        btnTestDestination.UseVisualStyleBackColor = true;
        btnTestDestination.Click += btnTestDestination_Click;
        // 
        // btnSaveSettings
        // 
        btnSaveSettings.AutoSize = true;
        btnSaveSettings.Location = new Point(1064, 3);
        btnSaveSettings.Name = "btnSaveSettings";
        btnSaveSettings.Size = new Size(107, 29);
        btnSaveSettings.TabIndex = 7;
        btnSaveSettings.Text = "Αποθήκευση";
        btnSaveSettings.UseVisualStyleBackColor = true;
        btnSaveSettings.Click += btnSaveSettings_Click;
        // 
        // lblFinalPathCaption
        // 
        lblFinalPathCaption.AutoSize = true;
        lblFinalPathCaption.Dock = DockStyle.Fill;
        lblFinalPathCaption.Location = new Point(3, 70);
        lblFinalPathCaption.Name = "lblFinalPathCaption";
        lblFinalPathCaption.Padding = new Padding(0, 4, 8, 0);
        lblFinalPathCaption.Size = new Size(126, 24);
        lblFinalPathCaption.TabIndex = 8;
        lblFinalPathCaption.Text = "Τελικός φάκελος:";
        // 
        // lblFinalPath
        // 
        lblFinalPath.AutoEllipsis = true;
        lblFinalPath.AutoSize = true;
        destinationLayout.SetColumnSpan(lblFinalPath, 4);
        lblFinalPath.Dock = DockStyle.Fill;
        lblFinalPath.Font = new Font("Consolas", 9F);
        lblFinalPath.Location = new Point(135, 70);
        lblFinalPath.Name = "lblFinalPath";
        lblFinalPath.Padding = new Padding(0, 4, 0, 0);
        lblFinalPath.Size = new Size(1036, 24);
        lblFinalPath.TabIndex = 9;
        lblFinalPath.Text = @"\\192.168.1.37\2026\PC1";
        // 
        // splitOptions
        // 
        splitOptions.Dock = DockStyle.Fill;
        splitOptions.Location = new Point(15, 198);
        splitOptions.Name = "splitOptions";
        // 
        // splitOptions.Panel1
        // 
        splitOptions.Panel1.Controls.Add(grpSources);
        // 
        // splitOptions.Panel2
        // 
        splitOptions.Panel2.Controls.Add(rightOptionsLayout);
        splitOptions.Size = new Size(1194, 269);
        splitOptions.SplitterDistance = 560;
        splitOptions.TabIndex = 2;
        // 
        // grpSources
        // 
        grpSources.Controls.Add(sourcesLayout);
        grpSources.Dock = DockStyle.Fill;
        grpSources.Location = new Point(0, 0);
        grpSources.Name = "grpSources";
        grpSources.Padding = new Padding(10);
        grpSources.Size = new Size(560, 269);
        grpSources.TabIndex = 0;
        grpSources.TabStop = false;
        grpSources.Text = "Πηγές αρχείων";
        // 
        // sourcesLayout
        // 
        sourcesLayout.ColumnCount = 2;
        sourcesLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        sourcesLayout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        sourcesLayout.Controls.Add(lblSourceUserRoot, 0, 0);
        sourcesLayout.SetColumnSpan(lblSourceUserRoot, 2);
        sourcesLayout.Controls.Add(txtSourceUserRoot, 0, 1);
        sourcesLayout.Controls.Add(btnApplySourceUserRoot, 1, 1);
        sourcesLayout.Controls.Add(clbSources, 0, 2);
        sourcesLayout.Controls.Add(txtCustomSource, 0, 3);
        sourcesLayout.Controls.Add(btnAddSource, 1, 3);
        sourcesLayout.Controls.Add(btnRemoveSource, 0, 4);
        sourcesLayout.Dock = DockStyle.Fill;
        sourcesLayout.Location = new Point(10, 30);
        sourcesLayout.Name = "sourcesLayout";
        sourcesLayout.RowCount = 5;
        sourcesLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        sourcesLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        sourcesLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        sourcesLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        sourcesLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        sourcesLayout.Size = new Size(540, 229);
        sourcesLayout.TabIndex = 0;
        // 
        // lblSourceUserRoot
        // 
        lblSourceUserRoot.AutoSize = true;
        lblSourceUserRoot.Dock = DockStyle.Fill;
        lblSourceUserRoot.Location = new Point(3, 0);
        lblSourceUserRoot.Name = "lblSourceUserRoot";
        lblSourceUserRoot.Size = new Size(534, 20);
        lblSourceUserRoot.TabIndex = 0;
        lblSourceUserRoot.Text = "Βάση χρήστη πηγών:";
        // 
        // txtSourceUserRoot
        // 
        txtSourceUserRoot.Dock = DockStyle.Fill;
        txtSourceUserRoot.Font = new Font("Consolas", 9F);
        txtSourceUserRoot.Location = new Point(3, 23);
        txtSourceUserRoot.Name = "txtSourceUserRoot";
        txtSourceUserRoot.PlaceholderText = @"C:\Users\Student";
        txtSourceUserRoot.Size = new Size(436, 25);
        txtSourceUserRoot.TabIndex = 1;
        // 
        // btnApplySourceUserRoot
        // 
        btnApplySourceUserRoot.AutoSize = true;
        btnApplySourceUserRoot.Location = new Point(445, 23);
        btnApplySourceUserRoot.Name = "btnApplySourceUserRoot";
        btnApplySourceUserRoot.Size = new Size(92, 30);
        btnApplySourceUserRoot.TabIndex = 2;
        btnApplySourceUserRoot.Text = "Εφαρμογή";
        btnApplySourceUserRoot.UseVisualStyleBackColor = true;
        btnApplySourceUserRoot.Click += btnApplySourceUserRoot_Click;
        // 
        // clbSources
        // 
        sourcesLayout.SetColumnSpan(clbSources, 2);
        clbSources.CheckOnClick = true;
        clbSources.Dock = DockStyle.Fill;
        clbSources.FormattingEnabled = true;
        clbSources.HorizontalScrollbar = true;
        clbSources.Location = new Point(3, 59);
        clbSources.Name = "clbSources";
        clbSources.Size = new Size(534, 100);
        clbSources.TabIndex = 3;
        // 
        // txtCustomSource
        // 
        txtCustomSource.Dock = DockStyle.Fill;
        txtCustomSource.Location = new Point(3, 135);
        txtCustomSource.Name = "txtCustomSource";
        txtCustomSource.PlaceholderText = @"Πρόσθεσε custom path, π.χ. D:\Μαθητές";
        txtCustomSource.Size = new Size(436, 27);
        txtCustomSource.TabIndex = 4;
        // 
        // btnAddSource
        // 
        btnAddSource.AutoSize = true;
        btnAddSource.Location = new Point(445, 135);
        btnAddSource.Name = "btnAddSource";
        btnAddSource.Size = new Size(92, 30);
        btnAddSource.TabIndex = 5;
        btnAddSource.Text = "Προσθήκη";
        btnAddSource.UseVisualStyleBackColor = true;
        btnAddSource.Click += btnAddSource_Click;
        // 
        // btnRemoveSource
        // 
        btnRemoveSource.AutoSize = true;
        btnRemoveSource.Location = new Point(3, 171);
        btnRemoveSource.Name = "btnRemoveSource";
        btnRemoveSource.Size = new Size(220, 30);
        btnRemoveSource.TabIndex = 6;
        btnRemoveSource.Text = "Αφαίρεση επιλεγμένου custom path";
        btnRemoveSource.UseVisualStyleBackColor = true;
        btnRemoveSource.Click += btnRemoveSource_Click;
        // 
        // rightOptionsLayout
        // 
        rightOptionsLayout.ColumnCount = 1;
        rightOptionsLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        rightOptionsLayout.Controls.Add(grpFileTypes, 0, 0);
        rightOptionsLayout.Controls.Add(grpAction, 0, 1);
        rightOptionsLayout.Dock = DockStyle.Fill;
        rightOptionsLayout.Location = new Point(0, 0);
        rightOptionsLayout.Name = "rightOptionsLayout";
        rightOptionsLayout.RowCount = 2;
        rightOptionsLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 160F));
        rightOptionsLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        rightOptionsLayout.Size = new Size(630, 269);
        rightOptionsLayout.TabIndex = 0;
        // 
        // grpFileTypes
        // 
        grpFileTypes.Controls.Add(fileTypesLayout);
        grpFileTypes.Dock = DockStyle.Fill;
        grpFileTypes.Location = new Point(3, 3);
        grpFileTypes.Name = "grpFileTypes";
        grpFileTypes.Padding = new Padding(10);
        grpFileTypes.Size = new Size(624, 154);
        grpFileTypes.TabIndex = 0;
        grpFileTypes.TabStop = false;
        grpFileTypes.Text = "Τύποι αρχείων";
        // 
        // fileTypesLayout
        // 
        fileTypesLayout.ColumnCount = 1;
        fileTypesLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        fileTypesLayout.Controls.Add(rbAllFiles, 0, 0);
        fileTypesLayout.Controls.Add(rbSelectedExtensions, 0, 1);
        fileTypesLayout.Controls.Add(txtExtensions, 0, 2);
        fileTypesLayout.Dock = DockStyle.Fill;
        fileTypesLayout.Location = new Point(10, 30);
        fileTypesLayout.Name = "fileTypesLayout";
        fileTypesLayout.RowCount = 3;
        fileTypesLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        fileTypesLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        fileTypesLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        fileTypesLayout.Size = new Size(604, 114);
        fileTypesLayout.TabIndex = 0;
        // 
        // rbAllFiles
        // 
        rbAllFiles.AutoSize = true;
        rbAllFiles.Location = new Point(3, 3);
        rbAllFiles.Name = "rbAllFiles";
        rbAllFiles.Size = new Size(119, 24);
        rbAllFiles.TabIndex = 0;
        rbAllFiles.Text = "Όλα τα αρχεία";
        rbAllFiles.UseVisualStyleBackColor = true;
        rbAllFiles.CheckedChanged += rbAllFiles_CheckedChanged;
        // 
        // rbSelectedExtensions
        // 
        rbSelectedExtensions.AutoSize = true;
        rbSelectedExtensions.Checked = true;
        rbSelectedExtensions.Location = new Point(3, 33);
        rbSelectedExtensions.Name = "rbSelectedExtensions";
        rbSelectedExtensions.Size = new Size(238, 24);
        rbSelectedExtensions.TabIndex = 1;
        rbSelectedExtensions.TabStop = true;
        rbSelectedExtensions.Text = "Μόνο συγκεκριμένες επεκτάσεις";
        rbSelectedExtensions.UseVisualStyleBackColor = true;
        rbSelectedExtensions.CheckedChanged += rbSelectedExtensions_CheckedChanged;
        // 
        // txtExtensions
        // 
        txtExtensions.Dock = DockStyle.Fill;
        txtExtensions.Font = new Font("Consolas", 9F);
        txtExtensions.Location = new Point(3, 63);
        txtExtensions.Multiline = true;
        txtExtensions.Name = "txtExtensions";
        txtExtensions.PlaceholderText = ".docx;.pdf;.py;.html;.css;.js;.sb3;.glo;.psg";
        txtExtensions.ScrollBars = ScrollBars.Vertical;
        txtExtensions.Size = new Size(598, 48);
        txtExtensions.TabIndex = 2;
        // 
        // grpAction
        // 
        grpAction.Controls.Add(actionLayout);
        grpAction.Dock = DockStyle.Fill;
        grpAction.Location = new Point(3, 120);
        grpAction.Name = "grpAction";
        grpAction.Padding = new Padding(10);
        grpAction.Size = new Size(624, 68);
        grpAction.TabIndex = 1;
        grpAction.TabStop = false;
        grpAction.Text = "Ενέργεια";
        // 
        // actionLayout
        // 
        actionLayout.Controls.Add(rbScanOnly);
        actionLayout.Controls.Add(rbCopy);
        actionLayout.Controls.Add(rbMove);
        actionLayout.Dock = DockStyle.Fill;
        actionLayout.FlowDirection = FlowDirection.LeftToRight;
        actionLayout.WrapContents = false;
        actionLayout.Location = new Point(10, 30);
        actionLayout.Name = "actionLayout";
        actionLayout.Size = new Size(604, 63);
        actionLayout.TabIndex = 0;
        // 
        // rbScanOnly
        // 
        rbScanOnly.AutoSize = true;
        rbScanOnly.Location = new Point(3, 3);
        rbScanOnly.Name = "rbScanOnly";
        rbScanOnly.Size = new Size(177, 24);
        rbScanOnly.TabIndex = 0;
        rbScanOnly.Text = "Μόνο σάρωση/preview";
        rbScanOnly.UseVisualStyleBackColor = true;
        // 
        // rbCopy
        // 
        rbCopy.AutoSize = true;
        rbCopy.Checked = true;
        rbCopy.Location = new Point(186, 3);
        rbCopy.Name = "rbCopy";
        rbCopy.Size = new Size(98, 24);
        rbCopy.TabIndex = 1;
        rbCopy.TabStop = true;
        rbCopy.Text = "Αντιγραφή";
        rbCopy.UseVisualStyleBackColor = true;
        // 
        // rbMove
        // 
        rbMove.AutoSize = true;
        rbMove.Location = new Point(290, 3);
        rbMove.Name = "rbMove";
        rbMove.Size = new Size(251, 24);
        rbMove.TabIndex = 2;
        rbMove.Text = "Μεταφορά αφού αντιγραφεί σωστά";
        rbMove.UseVisualStyleBackColor = true;
        // 
        // grpMaintenance
        // 
        grpMaintenance.Controls.Add(maintenanceLayout);
        grpMaintenance.Dock = DockStyle.Fill;
        grpMaintenance.Location = new Point(15, 473);
        grpMaintenance.Name = "grpMaintenance";
        grpMaintenance.Padding = new Padding(10);
        grpMaintenance.Size = new Size(1194, 129);
        grpMaintenance.TabIndex = 3;
        grpMaintenance.TabStop = false;
        grpMaintenance.Text = "Καθάρισμα μετά τη διαδικασία";
        // 
        // maintenanceLayout
        // 
        maintenanceLayout.ColumnCount = 3;
        maintenanceLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.3333321F));
        maintenanceLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.3333321F));
        maintenanceLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.3333321F));
        maintenanceLayout.Controls.Add(chkCleanUserTemp, 0, 0);
        maintenanceLayout.Controls.Add(chkCleanWindowsTemp, 1, 0);
        maintenanceLayout.Controls.Add(chkEmptyRecycleBin, 2, 0);
        maintenanceLayout.Controls.Add(chkCleanDownloadsPlanned, 0, 1);
        maintenanceLayout.SetColumnSpan(chkCleanDownloadsPlanned, 2);
        maintenanceLayout.Controls.Add(chkOptimizeDrive, 2, 1);
        maintenanceLayout.Controls.Add(chkCleanWindowsUpdateComponents, 0, 2);
        maintenanceLayout.SetColumnSpan(chkCleanWindowsUpdateComponents, 2);
        maintenanceLayout.Controls.Add(lblMaintenanceNote, 0, 3);
        maintenanceLayout.SetColumnSpan(lblMaintenanceNote, 3);
        maintenanceLayout.Dock = DockStyle.Fill;
        maintenanceLayout.Location = new Point(10, 30);
        maintenanceLayout.Name = "maintenanceLayout";
        maintenanceLayout.RowCount = 4;
        maintenanceLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        maintenanceLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        maintenanceLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        maintenanceLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        maintenanceLayout.Size = new Size(1174, 89);
        maintenanceLayout.TabIndex = 0;
        // 
        // chkCleanUserTemp
        // 
        chkCleanUserTemp.AutoSize = true;
        chkCleanUserTemp.Checked = true;
        chkCleanUserTemp.CheckState = CheckState.Checked;
        chkCleanUserTemp.Location = new Point(3, 3);
        chkCleanUserTemp.Name = "chkCleanUserTemp";
        chkCleanUserTemp.Size = new Size(250, 24);
        chkCleanUserTemp.TabIndex = 0;
        chkCleanUserTemp.Text = "Καθάρισμα Temp χρήστη Student";
        chkCleanUserTemp.UseVisualStyleBackColor = true;
        // 
        // chkCleanWindowsTemp
        // 
        chkCleanWindowsTemp.AutoSize = true;
        chkCleanWindowsTemp.Location = new Point(394, 3);
        chkCleanWindowsTemp.Name = "chkCleanWindowsTemp";
        chkCleanWindowsTemp.Size = new Size(191, 24);
        chkCleanWindowsTemp.TabIndex = 1;
        chkCleanWindowsTemp.Text = "Καθάρισμα Windows Temp";
        chkCleanWindowsTemp.UseVisualStyleBackColor = true;
        // 
        // chkCleanDownloadsPlanned
        // 
        chkCleanDownloadsPlanned.AutoSize = true;
        chkCleanDownloadsPlanned.Location = new Point(3, 33);
        chkCleanDownloadsPlanned.Name = "chkCleanDownloadsPlanned";
        chkCleanDownloadsPlanned.Size = new Size(371, 24);
        chkCleanDownloadsPlanned.TabIndex = 2;
        chkCleanDownloadsPlanned.Text = "Καθάρισμα Downloads μόνο για αρχεία του plan";
        chkCleanDownloadsPlanned.UseVisualStyleBackColor = true;
        // 
        // chkEmptyRecycleBin
        // 
        chkEmptyRecycleBin.AutoSize = true;
        chkEmptyRecycleBin.Location = new Point(785, 3);
        chkEmptyRecycleBin.Name = "chkEmptyRecycleBin";
        chkEmptyRecycleBin.Size = new Size(214, 24);
        chkEmptyRecycleBin.TabIndex = 3;
        chkEmptyRecycleBin.Text = "Άδειασμα Κάδου Ανακύκλωσης";
        chkEmptyRecycleBin.UseVisualStyleBackColor = true;
        // 
        // chkOptimizeDrive
        // 
        chkOptimizeDrive.AutoSize = true;
        chkOptimizeDrive.Location = new Point(785, 33);
        chkOptimizeDrive.Name = "chkOptimizeDrive";
        chkOptimizeDrive.Size = new Size(338, 24);
        chkOptimizeDrive.TabIndex = 4;
        chkOptimizeDrive.Text = "Optimize δίσκου συστήματος (SSD TRIM / HDD defrag)";
        chkOptimizeDrive.UseVisualStyleBackColor = true;
        // 
        // chkCleanWindowsUpdateComponents
        // 
        chkCleanWindowsUpdateComponents.AutoSize = true;
        chkCleanWindowsUpdateComponents.Location = new Point(3, 63);
        chkCleanWindowsUpdateComponents.Name = "chkCleanWindowsUpdateComponents";
        chkCleanWindowsUpdateComponents.Size = new Size(452, 24);
        chkCleanWindowsUpdateComponents.TabIndex = 5;
        chkCleanWindowsUpdateComponents.Text = "Καθάρισμα παλιών Windows Updates (DISM component cleanup)";
        chkCleanWindowsUpdateComponents.UseVisualStyleBackColor = true;
        // 
        // lblMaintenanceNote
        // 
        lblMaintenanceNote.AutoSize = true;
        lblMaintenanceNote.Dock = DockStyle.Fill;
        lblMaintenanceNote.ForeColor = SystemColors.GrayText;
        lblMaintenanceNote.Location = new Point(3, 90);
        lblMaintenanceNote.Name = "lblMaintenanceNote";
        lblMaintenanceNote.Padding = new Padding(0, 6, 0, 0);
        lblMaintenanceNote.Size = new Size(1168, 49);
        lblMaintenanceNote.TabIndex = 6;
        lblMaintenanceNote.Text = "Το cleanup εκτελείται μόνο μετά από Copy/Move και ζητά επιβεβαίωση. Τα Downloads δεν σβήνονται τυφλά. Το Windows Update cleanup χρειάζεται εκτέλεση ως διαχειριστής.";
        // 
        // grpPreview
        // 
        grpPreview.Controls.Add(previewLayout);
        grpPreview.Dock = DockStyle.Fill;
        grpPreview.Location = new Point(15, 513);
        grpPreview.Name = "grpPreview";
        grpPreview.Padding = new Padding(10);
        grpPreview.Size = new Size(1194, 260);
        grpPreview.TabIndex = 3;
        grpPreview.TabStop = false;
        grpPreview.Text = "Preview αρχείων - διατηρείται η δομή φακέλων";
        // 
        // previewLayout
        // 
        previewLayout.ColumnCount = 1;
        previewLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        previewLayout.Controls.Add(lvPreview, 0, 0);
        previewLayout.Controls.Add(lblSummary, 0, 1);
        previewLayout.Dock = DockStyle.Fill;
        previewLayout.Location = new Point(10, 30);
        previewLayout.Name = "previewLayout";
        previewLayout.RowCount = 2;
        previewLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        previewLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        previewLayout.Size = new Size(1174, 143);
        previewLayout.TabIndex = 0;
        // 
        // lvPreview
        // 
        lvPreview.Columns.AddRange(new ColumnHeader[] { colFile, colPlacement, colFolder, colSize, colSource, colDestination });
        lvPreview.Dock = DockStyle.Fill;
        lvPreview.FullRowSelect = true;
        lvPreview.GridLines = true;
        lvPreview.Location = new Point(3, 3);
        lvPreview.Name = "lvPreview";
        lvPreview.Size = new Size(1168, 107);
        lvPreview.TabIndex = 0;
        lvPreview.UseCompatibleStateImageBehavior = false;
        lvPreview.View = View.Details;
        // 
        // colFile
        // 
        colFile.Text = "Αρχείο";
        colFile.Width = 260;
        // 
        // colPlacement
        // 
        colPlacement.Text = "Θέση";
        colPlacement.Width = 90;
        // 
        // colFolder
        // 
        colFolder.Text = "Φάκελος / σχετικό path";
        colFolder.Width = 220;
        // 
        // colSize
        // 
        colSize.Text = "Μέγεθος";
        colSize.Width = 90;
        // 
        // colSource
        // 
        colSource.Text = "Πηγή";
        colSource.Width = 110;
        // 
        // colDestination
        // 
        colDestination.Text = "Προορισμός";
        colDestination.Width = 560;
        // 
        // lblSummary
        // 
        lblSummary.AutoSize = true;
        lblSummary.Dock = DockStyle.Fill;
        lblSummary.Location = new Point(3, 113);
        lblSummary.Name = "lblSummary";
        lblSummary.Padding = new Padding(0, 8, 0, 0);
        lblSummary.Size = new Size(1168, 28);
        lblSummary.TabIndex = 1;
        lblSummary.Text = "Αρχεία: 0 | Μέγεθος: 0 B";
        // 
        // bottomLayout
        // 
        bottomLayout.AutoSize = true;
        bottomLayout.ColumnCount = 1;
        bottomLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        bottomLayout.Controls.Add(buttonsLayout, 0, 0);
        bottomLayout.Controls.Add(progressBar, 0, 1);
        bottomLayout.Controls.Add(txtStatus, 0, 2);
        bottomLayout.Dock = DockStyle.Fill;
        bottomLayout.Location = new Point(15, 797);
        bottomLayout.Name = "bottomLayout";
        bottomLayout.RowCount = 3;
        bottomLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        bottomLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        bottomLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 82F));
        bottomLayout.Size = new Size(1194, 116);
        bottomLayout.TabIndex = 4;
        // 
        // buttonsLayout
        // 
        buttonsLayout.AutoSize = true;
        buttonsLayout.Controls.Add(btnScan);
        buttonsLayout.Controls.Add(btnExecute);
        buttonsLayout.Controls.Add(btnCancel);
        buttonsLayout.Controls.Add(btnUndoLast);
        buttonsLayout.Controls.Add(btnOpenDestination);
        buttonsLayout.Controls.Add(btnOpenLog);
        buttonsLayout.Controls.Add(btnHelp);
        buttonsLayout.Controls.Add(btnAbout);
        buttonsLayout.Controls.Add(lblTheme);
        buttonsLayout.Controls.Add(cboTheme);
        buttonsLayout.Dock = DockStyle.Fill;
        buttonsLayout.Location = new Point(3, 3);
        buttonsLayout.Name = "buttonsLayout";
        buttonsLayout.Size = new Size(1188, 35);
        buttonsLayout.TabIndex = 0;
        // 
        // btnScan
        // 
        btnScan.AutoSize = true;
        btnScan.Location = new Point(3, 3);
        btnScan.Name = "btnScan";
        btnScan.Size = new Size(90, 29);
        btnScan.TabIndex = 0;
        btnScan.Text = "Σάρωση";
        btnScan.UseVisualStyleBackColor = true;
        btnScan.Click += btnScan_Click;
        // 
        // btnExecute
        // 
        btnExecute.AutoSize = true;
        btnExecute.Location = new Point(99, 3);
        btnExecute.Name = "btnExecute";
        btnExecute.Size = new Size(90, 29);
        btnExecute.TabIndex = 1;
        btnExecute.Text = "Εκτέλεση";
        btnExecute.UseVisualStyleBackColor = true;
        btnExecute.Click += btnExecute_Click;
        // 
        // btnCancel
        // 
        btnCancel.AutoSize = true;
        btnCancel.Enabled = false;
        btnCancel.Location = new Point(195, 3);
        btnCancel.Name = "btnCancel";
        btnCancel.Size = new Size(90, 29);
        btnCancel.TabIndex = 2;
        btnCancel.Text = "Stop";
        btnCancel.UseVisualStyleBackColor = true;
        btnCancel.Click += btnCancel_Click;
        // 
        // btnUndoLast
        // 
        btnUndoLast.AutoSize = true;
        btnUndoLast.Enabled = false;
        btnUndoLast.Location = new Point(291, 3);
        btnUndoLast.Name = "btnUndoLast";
        btnUndoLast.Size = new Size(148, 29);
        btnUndoLast.TabIndex = 3;
        btnUndoLast.Text = "Undo τελευταίας";
        btnUndoLast.UseVisualStyleBackColor = true;
        btnUndoLast.Click += btnUndoLast_Click;
        // 
        // btnOpenDestination
        // 
        btnOpenDestination.AutoSize = true;
        btnOpenDestination.Location = new Point(445, 3);
        btnOpenDestination.Name = "btnOpenDestination";
        btnOpenDestination.Size = new Size(152, 29);
        btnOpenDestination.TabIndex = 4;
        btnOpenDestination.Text = "Άνοιγμα προορισμού";
        btnOpenDestination.UseVisualStyleBackColor = true;
        btnOpenDestination.Click += btnOpenDestination_Click;
        // 
        // btnOpenLog
        // 
        btnOpenLog.AutoSize = true;
        btnOpenLog.Location = new Point(603, 3);
        btnOpenLog.Name = "btnOpenLog";
        btnOpenLog.Size = new Size(147, 29);
        btnOpenLog.TabIndex = 5;
        btnOpenLog.Text = "Άνοιγμα log";
        btnOpenLog.UseVisualStyleBackColor = true;
        btnOpenLog.Click += btnOpenLog_Click;
        // 
        // btnHelp
        // 
        btnHelp.AutoSize = true;
        btnHelp.Location = new Point(756, 3);
        btnHelp.Name = "btnHelp";
        btnHelp.Size = new Size(75, 29);
        btnHelp.TabIndex = 6;
        btnHelp.Text = "Help";
        btnHelp.UseVisualStyleBackColor = true;
        btnHelp.Click += btnHelp_Click;
        // 
        // btnAbout
        // 
        btnAbout.AutoSize = true;
        btnAbout.Location = new Point(837, 3);
        btnAbout.Name = "btnAbout";
        btnAbout.Size = new Size(75, 29);
        btnAbout.TabIndex = 7;
        btnAbout.Text = "About";
        btnAbout.UseVisualStyleBackColor = true;
        btnAbout.Click += btnAbout_Click;
        // 
        // lblTheme
        // 
        lblTheme.AutoSize = true;
        lblTheme.Location = new Point(927, 8);
        lblTheme.Margin = new Padding(12, 8, 3, 0);
        lblTheme.Name = "lblTheme";
        lblTheme.Size = new Size(49, 20);
        lblTheme.TabIndex = 8;
        lblTheme.Text = "Θέμα:";
        // 
        // cboTheme
        // 
        cboTheme.DropDownStyle = ComboBoxStyle.DropDownList;
        cboTheme.FormattingEnabled = true;
        cboTheme.Location = new Point(982, 3);
        cboTheme.Name = "cboTheme";
        cboTheme.Size = new Size(130, 28);
        cboTheme.TabIndex = 9;
        cboTheme.SelectedIndexChanged += cboTheme_SelectedIndexChanged;
        // 
        // progressBar
        // 
        progressBar.Dock = DockStyle.Fill;
        progressBar.Location = new Point(3, 44);
        progressBar.Name = "progressBar";
        progressBar.Size = new Size(1188, 10);
        progressBar.TabIndex = 1;
        // 
        // txtStatus
        // 
        txtStatus.Dock = DockStyle.Fill;
        txtStatus.Location = new Point(3, 60);
        txtStatus.Multiline = true;
        txtStatus.Name = "txtStatus";
        txtStatus.ReadOnly = true;
        txtStatus.ScrollBars = ScrollBars.Vertical;
        txtStatus.Size = new Size(1188, 53);
        txtStatus.TabIndex = 2;
        // 
        // folderBrowserDialog
        // 
        folderBrowserDialog.Description = "Διάλεξε root φάκελο προορισμού ή custom source path";
        folderBrowserDialog.ShowNewFolderButton = true;
        folderBrowserDialog.UseDescriptionForTitle = true;
        // 
        // MainForm
        // 
        AutoScaleDimensions = new SizeF(8F, 20F);
        AutoScaleMode = AutoScaleMode.Font;
        AutoScroll = true;
        ClientSize = new Size(1430, 900);
        Controls.Add(rootLayout);
        MinimumSize = new Size(1280, 780);
        Name = "MainForm";
        StartPosition = FormStartPosition.CenterScreen;
        Text = "School Lab File Collector";
        rootLayout.ResumeLayout(false);
        rootLayout.PerformLayout();
        titleLayout.ResumeLayout(false);
        titleLayout.PerformLayout();
        grpDestination.ResumeLayout(false);
        grpDestination.PerformLayout();
        destinationLayout.ResumeLayout(false);
        destinationLayout.PerformLayout();
        splitOptions.Panel1.ResumeLayout(false);
        splitOptions.Panel2.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)splitOptions).EndInit();
        splitOptions.ResumeLayout(false);
        grpSources.ResumeLayout(false);
        sourcesLayout.ResumeLayout(false);
        sourcesLayout.PerformLayout();
        rightOptionsLayout.ResumeLayout(false);
        grpFileTypes.ResumeLayout(false);
        fileTypesLayout.ResumeLayout(false);
        fileTypesLayout.PerformLayout();
        grpAction.ResumeLayout(false);
        actionLayout.ResumeLayout(false);
        actionLayout.PerformLayout();
        grpMaintenance.ResumeLayout(false);
        maintenanceLayout.ResumeLayout(false);
        maintenanceLayout.PerformLayout();
        grpPreview.ResumeLayout(false);
        previewLayout.ResumeLayout(false);
        previewLayout.PerformLayout();
        bottomLayout.ResumeLayout(false);
        bottomLayout.PerformLayout();
        buttonsLayout.ResumeLayout(false);
        buttonsLayout.PerformLayout();
        ResumeLayout(false);
    }
}
