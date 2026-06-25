using System.Drawing;
using System.Windows.Forms;

namespace LabFileCollector;

partial class HelpForm
{
    private System.ComponentModel.IContainer components = null!;
    private TableLayoutPanel rootLayout;
    private Label lblTitle;
    private TextBox txtHelp;
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
        lblTitle = new Label();
        txtHelp = new TextBox();
        buttonsLayout = new FlowLayoutPanel();
        btnClose = new ModernButton();
        rootLayout.SuspendLayout();
        buttonsLayout.SuspendLayout();
        SuspendLayout();
        // 
        // rootLayout
        // 
        rootLayout.ColumnCount = 1;
        rootLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        rootLayout.Controls.Add(lblTitle, 0, 0);
        rootLayout.Controls.Add(txtHelp, 0, 1);
        rootLayout.Controls.Add(buttonsLayout, 0, 2);
        rootLayout.Dock = DockStyle.Fill;
        rootLayout.Location = new Point(0, 0);
        rootLayout.Name = "rootLayout";
        rootLayout.Padding = new Padding(14);
        rootLayout.RowCount = 3;
        rootLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        rootLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        rootLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        rootLayout.Size = new Size(820, 620);
        rootLayout.TabIndex = 0;
        // 
        // lblTitle
        // 
        lblTitle.AutoSize = true;
        lblTitle.Dock = DockStyle.Fill;
        lblTitle.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
        lblTitle.Location = new Point(17, 14);
        lblTitle.Name = "lblTitle";
        lblTitle.Padding = new Padding(0, 0, 0, 10);
        lblTitle.Size = new Size(786, 47);
        lblTitle.TabIndex = 0;
        lblTitle.Text = "Help - School Lab File Collector";
        // 
        // txtHelp
        // 
        txtHelp.Dock = DockStyle.Fill;
        txtHelp.Font = new Font("Segoe UI", 10F);
        txtHelp.Location = new Point(17, 64);
        txtHelp.Multiline = true;
        txtHelp.Name = "txtHelp";
        txtHelp.ReadOnly = true;
        txtHelp.ScrollBars = ScrollBars.Vertical;
        txtHelp.Size = new Size(786, 493);
        txtHelp.TabIndex = 1;
        // 
        // buttonsLayout
        // 
        buttonsLayout.AutoSize = true;
        buttonsLayout.Controls.Add(btnClose);
        buttonsLayout.Dock = DockStyle.Fill;
        buttonsLayout.FlowDirection = FlowDirection.RightToLeft;
        buttonsLayout.Location = new Point(17, 563);
        buttonsLayout.Name = "buttonsLayout";
        buttonsLayout.Size = new Size(786, 40);
        buttonsLayout.TabIndex = 2;
        // 
        // btnClose
        // 
        btnClose.AutoSize = true;
        btnClose.DialogResult = DialogResult.OK;
        btnClose.Location = new Point(695, 3);
        btnClose.Name = "btnClose";
        btnClose.Size = new Size(88, 30);
        btnClose.TabIndex = 0;
        btnClose.Text = "Κλείσιμο";
        btnClose.UseVisualStyleBackColor = true;
        // 
        // HelpForm
        // 
        AcceptButton = btnClose;
        AutoScaleDimensions = new SizeF(8F, 20F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(820, 620);
        Controls.Add(rootLayout);
        MinimumSize = new Size(720, 520);
        Name = "HelpForm";
        StartPosition = FormStartPosition.CenterParent;
        Text = "Help - School Lab File Collector";
        rootLayout.ResumeLayout(false);
        rootLayout.PerformLayout();
        buttonsLayout.ResumeLayout(false);
        buttonsLayout.PerformLayout();
        ResumeLayout(false);
    }
}
