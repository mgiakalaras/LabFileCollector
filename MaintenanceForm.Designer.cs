using System.Drawing;
using System.Windows.Forms;

namespace LabFileCollector;

partial class MaintenanceForm
{
    private System.ComponentModel.IContainer components = null!;
    private TableLayoutPanel rootLayout;
    private Label lblIntro;
    private GroupBox grpOptions;
    private TableLayoutPanel optionsLayout;
    private CheckBox chkRunAfterTransfer;
    private CheckBox chkCleanUserTemp;
    private CheckBox chkCleanWindowsTemp;
    private CheckBox chkCleanDownloadsPlanned;
    private CheckBox chkEmptyRecycleBin;
    private CheckBox chkOptimizeDrive;
    private CheckBox chkCleanWindowsUpdateComponents;
    private Label lblNote;
    private ProgressBar progressBar;
    private TextBox txtLog;
    private FlowLayoutPanel buttonsLayout;
    private Button btnSave;
    private Button btnRunNow;
    private Button btnClose;

    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
            components.Dispose();
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();
        rootLayout = new TableLayoutPanel();
        lblIntro = new Label();
        grpOptions = new ModernGroupBox();
        optionsLayout = new TableLayoutPanel();
        chkRunAfterTransfer = new CheckBox();
        chkCleanUserTemp = new CheckBox();
        chkCleanWindowsTemp = new CheckBox();
        chkCleanDownloadsPlanned = new CheckBox();
        chkEmptyRecycleBin = new CheckBox();
        chkOptimizeDrive = new CheckBox();
        chkCleanWindowsUpdateComponents = new CheckBox();
        lblNote = new Label();
        progressBar = new ProgressBar();
        txtLog = new TextBox();
        buttonsLayout = new FlowLayoutPanel();
        btnSave = new ModernButton();
        btnRunNow = new ModernButton();
        btnClose = new ModernButton();
        rootLayout.SuspendLayout();
        grpOptions.SuspendLayout();
        optionsLayout.SuspendLayout();
        buttonsLayout.SuspendLayout();
        SuspendLayout();
        // 
        // rootLayout
        // 
        rootLayout.ColumnCount = 1;
        rootLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        rootLayout.Controls.Add(lblIntro, 0, 0);
        rootLayout.Controls.Add(grpOptions, 0, 1);
        rootLayout.Controls.Add(progressBar, 0, 2);
        rootLayout.Controls.Add(txtLog, 0, 3);
        rootLayout.Controls.Add(buttonsLayout, 0, 4);
        rootLayout.Dock = DockStyle.Fill;
        rootLayout.Location = new Point(0, 0);
        rootLayout.Name = "rootLayout";
        rootLayout.Padding = new Padding(14);
        rootLayout.RowCount = 5;
        rootLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        rootLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        rootLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        rootLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        rootLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        rootLayout.Size = new Size(760, 600);
        rootLayout.TabIndex = 0;
        // 
        // lblIntro
        // 
        lblIntro.AutoSize = true;
        lblIntro.Dock = DockStyle.Fill;
        lblIntro.Font = new Font("Segoe UI", 10F);
        lblIntro.Location = new Point(17, 14);
        lblIntro.Name = "lblIntro";
        lblIntro.Padding = new Padding(0, 0, 0, 10);
        lblIntro.Size = new Size(726, 33);
        lblIntro.TabIndex = 0;
        lblIntro.Text = "Συντήρηση υπολογιστή χωρίς σάρωση αρχείων. Διάλεξε εργασίες, αποθήκευσέ τες ή τρέξ’ τες άμεσα.";
        // 
        // grpOptions
        // 
        grpOptions.AutoSize = true;
        grpOptions.Controls.Add(optionsLayout);
        grpOptions.Dock = DockStyle.Fill;
        grpOptions.Location = new Point(17, 50);
        grpOptions.Name = "grpOptions";
        grpOptions.Padding = new Padding(10);
        grpOptions.Size = new Size(726, 246);
        grpOptions.TabIndex = 1;
        grpOptions.TabStop = false;
        grpOptions.Text = "Επιλογές καθαρισμού";
        // 
        // optionsLayout
        // 
        optionsLayout.AutoSize = true;
        optionsLayout.ColumnCount = 2;
        optionsLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
        optionsLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
        optionsLayout.Controls.Add(chkRunAfterTransfer, 0, 0);
        optionsLayout.SetColumnSpan(chkRunAfterTransfer, 2);
        optionsLayout.Controls.Add(chkCleanUserTemp, 0, 1);
        optionsLayout.Controls.Add(chkCleanWindowsTemp, 1, 1);
        optionsLayout.Controls.Add(chkCleanDownloadsPlanned, 0, 2);
        optionsLayout.Controls.Add(chkEmptyRecycleBin, 1, 2);
        optionsLayout.Controls.Add(chkOptimizeDrive, 0, 3);
        optionsLayout.Controls.Add(chkCleanWindowsUpdateComponents, 1, 3);
        optionsLayout.Controls.Add(lblNote, 0, 4);
        optionsLayout.SetColumnSpan(lblNote, 2);
        optionsLayout.Dock = DockStyle.Fill;
        optionsLayout.Location = new Point(10, 30);
        optionsLayout.Name = "optionsLayout";
        optionsLayout.RowCount = 5;
        optionsLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        optionsLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        optionsLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        optionsLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        optionsLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        optionsLayout.Size = new Size(706, 206);
        optionsLayout.TabIndex = 0;
        // 
        // chkRunAfterTransfer
        // 
        chkRunAfterTransfer.AutoSize = true;
        chkRunAfterTransfer.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        chkRunAfterTransfer.Location = new Point(3, 3);
        chkRunAfterTransfer.Name = "chkRunAfterTransfer";
        chkRunAfterTransfer.Size = new Size(394, 24);
        chkRunAfterTransfer.TabIndex = 0;
        chkRunAfterTransfer.Text = "Να τρέχει αυτόματα μετά από επιτυχημένο Copy/Move";
        chkRunAfterTransfer.UseVisualStyleBackColor = true;
        // 
        // chkCleanUserTemp
        // 
        chkCleanUserTemp.AutoSize = true;
        chkCleanUserTemp.Location = new Point(3, 33);
        chkCleanUserTemp.Name = "chkCleanUserTemp";
        chkCleanUserTemp.Size = new Size(250, 24);
        chkCleanUserTemp.TabIndex = 1;
        chkCleanUserTemp.Text = "Καθάρισμα Temp επιλεγμένου χρήστη";
        chkCleanUserTemp.UseVisualStyleBackColor = true;
        // 
        // chkCleanWindowsTemp
        // 
        chkCleanWindowsTemp.AutoSize = true;
        chkCleanWindowsTemp.Location = new Point(356, 33);
        chkCleanWindowsTemp.Name = "chkCleanWindowsTemp";
        chkCleanWindowsTemp.Size = new Size(191, 24);
        chkCleanWindowsTemp.TabIndex = 2;
        chkCleanWindowsTemp.Text = "Καθάρισμα Windows Temp";
        chkCleanWindowsTemp.UseVisualStyleBackColor = true;
        // 
        // chkCleanDownloadsPlanned
        // 
        chkCleanDownloadsPlanned.AutoSize = true;
        chkCleanDownloadsPlanned.Location = new Point(3, 63);
        chkCleanDownloadsPlanned.Name = "chkCleanDownloadsPlanned";
        chkCleanDownloadsPlanned.Size = new Size(333, 24);
        chkCleanDownloadsPlanned.TabIndex = 3;
        chkCleanDownloadsPlanned.Text = "Downloads μόνο για αρχεία του τελευταίου plan";
        chkCleanDownloadsPlanned.UseVisualStyleBackColor = true;
        // 
        // chkEmptyRecycleBin
        // 
        chkEmptyRecycleBin.AutoSize = true;
        chkEmptyRecycleBin.Location = new Point(356, 63);
        chkEmptyRecycleBin.Name = "chkEmptyRecycleBin";
        chkEmptyRecycleBin.Size = new Size(214, 24);
        chkEmptyRecycleBin.TabIndex = 4;
        chkEmptyRecycleBin.Text = "Άδειασμα Κάδου Ανακύκλωσης";
        chkEmptyRecycleBin.UseVisualStyleBackColor = true;
        // 
        // chkOptimizeDrive
        // 
        chkOptimizeDrive.AutoSize = true;
        chkOptimizeDrive.Location = new Point(3, 93);
        chkOptimizeDrive.Name = "chkOptimizeDrive";
        chkOptimizeDrive.Size = new Size(324, 24);
        chkOptimizeDrive.TabIndex = 5;
        chkOptimizeDrive.Text = "Optimize δίσκου συστήματος / SSD TRIM";
        chkOptimizeDrive.UseVisualStyleBackColor = true;
        // 
        // chkCleanWindowsUpdateComponents
        // 
        chkCleanWindowsUpdateComponents.AutoSize = true;
        chkCleanWindowsUpdateComponents.Location = new Point(356, 93);
        chkCleanWindowsUpdateComponents.Name = "chkCleanWindowsUpdateComponents";
        chkCleanWindowsUpdateComponents.Size = new Size(328, 24);
        chkCleanWindowsUpdateComponents.TabIndex = 6;
        chkCleanWindowsUpdateComponents.Text = "Windows Update cleanup μέσω DISM";
        chkCleanWindowsUpdateComponents.UseVisualStyleBackColor = true;
        // 
        // lblNote
        // 
        lblNote.AutoSize = true;
        lblNote.Dock = DockStyle.Fill;
        lblNote.ForeColor = SystemColors.GrayText;
        lblNote.Location = new Point(3, 120);
        lblNote.Name = "lblNote";
        lblNote.Padding = new Padding(0, 10, 0, 0);
        lblNote.Size = new Size(700, 86);
        lblNote.TabIndex = 7;
        lblNote.Text = "Το cleanup μπορεί να τρέξει μόνο του, χωρίς σάρωση αρχείων. Το Windows Update cleanup, ο Κάδος και το Optimize θέλουν δικαιώματα διαχειριστή. Η εφαρμογή ζητά αυτόματα admin στην εκκίνηση.";
        // 
        // progressBar
        // 
        progressBar.Dock = DockStyle.Fill;
        progressBar.Location = new Point(17, 302);
        progressBar.Name = "progressBar";
        progressBar.Size = new Size(726, 10);
        progressBar.TabIndex = 2;
        // 
        // txtLog
        // 
        txtLog.Dock = DockStyle.Fill;
        txtLog.Location = new Point(17, 318);
        txtLog.Multiline = true;
        txtLog.Name = "txtLog";
        txtLog.ReadOnly = true;
        txtLog.ScrollBars = ScrollBars.Vertical;
        txtLog.Size = new Size(726, 223);
        txtLog.TabIndex = 3;
        // 
        // buttonsLayout
        // 
        buttonsLayout.AutoSize = true;
        buttonsLayout.Controls.Add(btnSave);
        buttonsLayout.Controls.Add(btnRunNow);
        buttonsLayout.Controls.Add(btnClose);
        buttonsLayout.Dock = DockStyle.Fill;
        buttonsLayout.FlowDirection = FlowDirection.RightToLeft;
        buttonsLayout.Location = new Point(17, 547);
        buttonsLayout.Name = "buttonsLayout";
        buttonsLayout.Size = new Size(726, 39);
        buttonsLayout.TabIndex = 4;
        // 
        // btnSave
        // 
        btnSave.AutoSize = true;
        btnSave.Location = new Point(593, 3);
        btnSave.Name = "btnSave";
        btnSave.Size = new Size(130, 33);
        btnSave.TabIndex = 0;
        btnSave.Text = "Αποθήκευση";
        btnSave.UseVisualStyleBackColor = true;
        btnSave.Click += btnSave_Click;
        // 
        // btnRunNow
        // 
        btnRunNow.AutoSize = true;
        btnRunNow.Location = new Point(437, 3);
        btnRunNow.Name = "btnRunNow";
        btnRunNow.Size = new Size(150, 33);
        btnRunNow.TabIndex = 1;
        btnRunNow.Text = "Τρέξε καθάρισμα";
        btnRunNow.UseVisualStyleBackColor = true;
        btnRunNow.Click += btnRunNow_Click;
        // 
        // btnClose
        // 
        btnClose.AutoSize = true;
        btnClose.Location = new Point(333, 3);
        btnClose.Name = "btnClose";
        btnClose.Size = new Size(98, 33);
        btnClose.TabIndex = 2;
        btnClose.Text = "Κλείσιμο";
        btnClose.UseVisualStyleBackColor = true;
        btnClose.Click += btnClose_Click;
        // 
        // MaintenanceForm
        // 
        AutoScaleDimensions = new SizeF(8F, 20F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(760, 600);
        Controls.Add(rootLayout);
        MinimumSize = new Size(720, 560);
        Name = "MaintenanceForm";
        StartPosition = FormStartPosition.CenterParent;
        Text = "Συντήρηση συστήματος";
        rootLayout.ResumeLayout(false);
        rootLayout.PerformLayout();
        grpOptions.ResumeLayout(false);
        grpOptions.PerformLayout();
        optionsLayout.ResumeLayout(false);
        optionsLayout.PerformLayout();
        buttonsLayout.ResumeLayout(false);
        buttonsLayout.PerformLayout();
        ResumeLayout(false);
    }
}
