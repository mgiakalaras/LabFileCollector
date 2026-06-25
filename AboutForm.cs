using System.Drawing;
using System.Reflection;
using System.Windows.Forms;

namespace LabFileCollector;

public partial class AboutForm : Form
{
    public AboutForm() : this(false)
    {
    }

    public AboutForm(bool darkTheme)
    {
        InitializeComponent();
        var icon = AppVisuals.LoadAppIcon();
        if (icon is not null)
            Icon = icon;

        var logo = AppVisuals.LoadLogoImage();
        if (logo is not null)
            picLogoLarge.Image = logo;

        var version = Assembly.GetExecutingAssembly().GetName().Version?.ToString(3) ?? "1.0.0";
        lblVersion.Text = $"Έκδοση {version} • Windows Forms • .NET 8";
        txtAbout.Text = BuildAboutText();
        ApplyTheme(darkTheme);
    }

    private static string BuildAboutText()
    {
        return string.Join(Environment.NewLine, new[]
        {
            "Εργαλείο συντήρησης σχολικού εργαστηρίου.",
            "",
            "Σκοπός:",
            "• Συλλογή αρχείων μαθητών/εργαστηρίου από επιλεγμένες πηγές.",
            "• Αποθήκευση σε δικτυακό ή τοπικό φάκελο, με υποφάκελο ανά υπολογιστή.",
            "• Preview πριν από την εκτέλεση και CSV logs για έλεγχο.",
            "• Πλαϊνή μπάρα πληροφοριών συστήματος: CPU, RAM, δίσκοι και LAN.",
            "• Προαιρετικός καθαρισμός Temp/Downloads/Recycle Bin και Windows maintenance.",
            "",
            "Προεπιλεγμένες επεκτάσεις:",
            ".doc, .docx, .xls, .xlsx, .ppt, .pptx, .pdf, .txt, .py, .html, .css, .js, .sb3, .glo, .psg",
            "",
            "Σημείωση ασφαλείας:",
            "Το πρόγραμμα δεν σβήνει τυφλά τα Downloads και δεν πειράζει χειροκίνητα τον φάκελο WinSxS. Το Windows Update cleanup γίνεται μέσω DISM.",
            "",
            "© 2026 - School Lab File Collector"
        });
    }

    private void ApplyTheme(bool dark)
    {
        var back = dark ? Color.FromArgb(31, 41, 55) : Color.FromArgb(243, 247, 252);
        var panel = dark ? Color.FromArgb(35, 47, 63) : Color.FromArgb(252, 253, 255);
        var input = dark ? Color.FromArgb(24, 34, 48) : Color.White;
        var fore = dark ? Color.FromArgb(235, 241, 248) : Color.FromArgb(31, 41, 55);
        var muted = dark ? Color.FromArgb(173, 186, 204) : Color.FromArgb(100, 116, 139);
        var title = dark ? Color.FromArgb(216, 232, 255) : Color.FromArgb(20, 60, 120);
        var button = dark ? Color.FromArgb(44, 59, 79) : Color.FromArgb(235, 244, 255);

        BackColor = back;
        ForeColor = fore;
        rootLayout.BackColor = panel;
        rootLayout.ForeColor = fore;
        picLogoLarge.BackColor = panel;
        lblTitle.BackColor = panel;
        lblTitle.ForeColor = title;
        lblVersion.BackColor = panel;
        lblVersion.ForeColor = muted;
        txtAbout.BackColor = input;
        txtAbout.ForeColor = fore;
        buttonsLayout.BackColor = panel;
        btnClose.ForeColor = fore;
        btnClose.BackColor = button;
        btnClose.UseVisualStyleBackColor = !dark;
        btnClose.FlatStyle = dark ? FlatStyle.Flat : FlatStyle.Standard;
        if (dark)
        {
            btnClose.FlatAppearance.BorderColor = Color.FromArgb(80, 150, 230);
            btnClose.FlatAppearance.MouseOverBackColor = Color.FromArgb(57, 78, 105);
        }
    }
}
