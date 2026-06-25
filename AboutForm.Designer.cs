using System.Drawing;
using System.Windows.Forms;

namespace LabFileCollector;

partial class AboutForm
{
    private System.ComponentModel.IContainer components = null!;
    private TableLayoutPanel rootLayout;
    private PictureBox picLogoLarge;
    private Label lblTitle;
    private Label lblVersion;
    private TextBox txtAbout;
    private FlowLayoutPanel buttonsLayout;
    private Button btnClose;

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
        picLogoLarge = new PictureBox();
        lblTitle = new Label();
        lblVersion = new Label();
        txtAbout = new TextBox();
        buttonsLayout = new FlowLayoutPanel();
        btnClose = new ModernButton();
        rootLayout.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)picLogoLarge).BeginInit();
        buttonsLayout.SuspendLayout();
        SuspendLayout();
        // 
        // rootLayout
        // 
        rootLayout.ColumnCount = 2;
        rootLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 150F));
        rootLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        rootLayout.Controls.Add(picLogoLarge, 0, 0);
        rootLayout.SetRowSpan(picLogoLarge, 2);
        rootLayout.Controls.Add(lblTitle, 1, 0);
        rootLayout.Controls.Add(lblVersion, 1, 1);
        rootLayout.Controls.Add(txtAbout, 0, 2);
        rootLayout.SetColumnSpan(txtAbout, 2);
        rootLayout.Controls.Add(buttonsLayout, 0, 3);
        rootLayout.SetColumnSpan(buttonsLayout, 2);
        rootLayout.Dock = DockStyle.Fill;
        rootLayout.Location = new Point(0, 0);
        rootLayout.Name = "rootLayout";
        rootLayout.Padding = new Padding(18);
        rootLayout.RowCount = 4;
        rootLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        rootLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        rootLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        rootLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        rootLayout.Size = new Size(720, 500);
        rootLayout.TabIndex = 0;
        // 
        // picLogoLarge
        // 
        picLogoLarge.Dock = DockStyle.Fill;
        picLogoLarge.Location = new Point(21, 21);
        picLogoLarge.Name = "picLogoLarge";
        picLogoLarge.Size = new Size(144, 88);
        picLogoLarge.SizeMode = PictureBoxSizeMode.Zoom;
        picLogoLarge.TabIndex = 0;
        picLogoLarge.TabStop = false;
        // 
        // lblTitle
        // 
        lblTitle.AutoSize = true;
        lblTitle.Dock = DockStyle.Fill;
        lblTitle.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
        lblTitle.Location = new Point(171, 18);
        lblTitle.Name = "lblTitle";
        lblTitle.Size = new Size(528, 41);
        lblTitle.TabIndex = 1;
        lblTitle.Text = "School Lab File Collector";
        // 
        // lblVersion
        // 
        lblVersion.AutoSize = true;
        lblVersion.Dock = DockStyle.Fill;
        lblVersion.Font = new Font("Segoe UI", 10F);
        lblVersion.Location = new Point(171, 59);
        lblVersion.Name = "lblVersion";
        lblVersion.Padding = new Padding(0, 0, 0, 18);
        lblVersion.Size = new Size(528, 53);
        lblVersion.TabIndex = 2;
        lblVersion.Text = "Version";
        // 
        // txtAbout
        // 
        txtAbout.Dock = DockStyle.Fill;
        txtAbout.Font = new Font("Segoe UI", 10F);
        txtAbout.Location = new Point(21, 115);
        txtAbout.Multiline = true;
        txtAbout.Name = "txtAbout";
        txtAbout.ReadOnly = true;
        txtAbout.ScrollBars = ScrollBars.Vertical;
        txtAbout.Size = new Size(678, 320);
        txtAbout.TabIndex = 3;
        // 
        // buttonsLayout
        // 
        buttonsLayout.AutoSize = true;
        buttonsLayout.Controls.Add(btnClose);
        buttonsLayout.Dock = DockStyle.Fill;
        buttonsLayout.FlowDirection = FlowDirection.RightToLeft;
        buttonsLayout.Location = new Point(21, 441);
        buttonsLayout.Name = "buttonsLayout";
        buttonsLayout.Size = new Size(678, 38);
        buttonsLayout.TabIndex = 4;
        // 
        // btnClose
        // 
        btnClose.AutoSize = true;
        btnClose.DialogResult = DialogResult.OK;
        btnClose.Location = new Point(587, 3);
        btnClose.Name = "btnClose";
        btnClose.Size = new Size(88, 30);
        btnClose.TabIndex = 0;
        btnClose.Text = "Κλείσιμο";
        btnClose.UseVisualStyleBackColor = true;
        // 
        // AboutForm
        // 
        AcceptButton = btnClose;
        AutoScaleDimensions = new SizeF(8F, 20F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(720, 500);
        Controls.Add(rootLayout);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "AboutForm";
        StartPosition = FormStartPosition.CenterParent;
        Text = "About - School Lab File Collector";
        rootLayout.ResumeLayout(false);
        rootLayout.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)picLogoLarge).EndInit();
        buttonsLayout.ResumeLayout(false);
        buttonsLayout.PerformLayout();
        ResumeLayout(false);
    }
}
