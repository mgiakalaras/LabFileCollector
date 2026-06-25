using System.Drawing;
using System.Windows.Forms;

namespace LabFileCollector;

public partial class HelpForm : Form
{
    public HelpForm() : this(false)
    {
    }

    public HelpForm(bool darkTheme)
    {
        InitializeComponent();
        var icon = AppVisuals.LoadAppIcon();
        if (icon is not null)
            Icon = icon;

        txtHelp.Text = BuildHelpText();
        ApplyTheme(darkTheme);
    }

    private static string BuildHelpText()
    {
        return string.Join(Environment.NewLine, new[]
        {
            "Βασική ροή χρήσης",
            "",
            "1. Έλεγξε ότι το Όνομα υπολογιστή είναι σωστό. Το κουμπί 'Από Windows' παίρνει το πραγματικό όνομα του PC.",
            "2. Όρισε Root path προορισμού, π.χ. \\\\192.168.1.37\\2026 ή άλλο share/δίσκο.",
            "3. Έλεγξε ότι οι πηγές αρχείων δείχνουν στο σωστό προφίλ χρήστη. Πάτησε Τρέχων για τον χρήστη Windows ή γράψε άλλο C:\\Users\\... path.",
            "4. Διάλεξε τύπους αρχείων. Οι προεπιλογές περιλαμβάνουν .glo και .psg για ασκήσεις ΓΛΩΣΣΑΣ/ψευδογλώσσας.",
            "5. Πάτα Σάρωση για preview. Δεν αλλάζει τίποτα στον δίσκο. Αν πατήσεις Stop, θα ζητηθεί επιβεβαίωση και θα επανέλθει το προηγούμενο preview/plan.",
            "6. Στο preview η στήλη 'Θέση' δείχνει αν ένα αρχείο είναι Χύμα ή Σε φάκελο.",
            "7. Η δομή φακέλων διατηρείται αυτούσια στον προορισμό. Π.χ. Desktop\\Μαθητής\\εργασία.glo γίνεται PC\\Desktop\\Μαθητής\\εργασία.glo.",
            "8. Πάτα Εκτέλεση μόνο όταν είσαι σίγουρος για Copy ή Move.",
            "",
            "Ασφαλής χρήση",
            "",
            "• Για πρώτη δοκιμή βάλε Μόνο σάρωση ή Αντιγραφή, όχι Μεταφορά.",
            "• Αν έχεις ενεργό φίλτρο επεκτάσεων, περνάνε μόνο οι επιλεγμένοι τύποι αρχείων αλλά μέσα στη σωστή δομή φακέλων.",
            "• Αν θέλεις να περάσουν όλα όσα υπάρχουν στους φακέλους μαθητών, διάλεξε 'Όλα τα αρχεία'.",
            "• Η Μεταφορά διαγράφει την πηγή μόνο αφού ολοκληρωθεί επιτυχημένα η αντιγραφή.",
            "• Το Stop ζητά επιβεβαίωση πριν ακυρώσει τη σάρωση, αντιγραφή, μεταφορά ή undo.",
            "• Το κουμπί Undo τελευταίας μπορεί να αναιρέσει την τελευταία αντιγραφή/μεταφορά της τρέχουσας συνεδρίας.",
            "• Το Undo δεν αναιρεί Temp cleanup, άδειασμα κάδου, DISM ή Optimize.",
            "• Τα Downloads δεν καθαρίζονται τυφλά. Διαγράφονται μόνο αρχεία του τρέχοντος plan που πέρασαν επιτυχώς.",
            "• Το Windows Update cleanup χρησιμοποιεί DISM και θέλει εκτέλεση ως διαχειριστής.",
            "• Το Optimize δίσκου καλεί τα Windows με /O: σε SSD κάνει TRIM, σε HDD κάνει defrag/optimize.",
            "",
            "Εμφάνιση / πληροφορίες συστήματος",
            "",
            "• Από το πεδίο Θέμα επιλέγεις Σύστημα/Windows, Φωτεινό/Aero, Σκούρο/Aero, Notepad++ Dark, Nord Dark ή Dracula.",
            "• Η πλαϊνή μπάρα δείχνει βασικά στοιχεία συστήματος: τύπο, όνομα PC, CPU χρήση, RAM, δίσκους και ενεργούς LAN/Wi‑Fi adapters.",
            "• Τα cards ενημερώνονται live χωρίς να ξαναχτίζεται ολόκληρη η sidebar κάθε λίγα δευτερόλεπτα.",
            "• Τα scrollbars ακολουθούν όσο γίνεται το native Windows/DarkMode_Explorer theme. Σε κλασικά WinForms controls δεν γίνεται πάντα πλήρες rounded scrollbar χωρίς custom control.",
            "",
            "Logs",
            "",
            "• Τα transfer logs γράφονται σε φάκελο _logs μέσα στον root προορισμό.",
            "• Τα maintenance logs γράφονται επίσης στο _logs.",
            "• Χρησιμοποίησε το κουμπί 'Άνοιγμα log' για να δεις το τελευταίο log της συνεδρίας.",
            "",
            "Προτεινόμενη δοκιμή στο εργαστήριο",
            "",
            "Clean Solution → Rebuild Solution → F5 → Σάρωση → Copy σε test folder → Copy στο δίκτυο → Move μόνο με δοκιμαστικά αρχεία."
        });
    }

    private void ApplyTheme(bool dark)
    {
        var back = dark ? Color.FromArgb(31, 41, 55) : Color.FromArgb(243, 247, 252);
        var panel = dark ? Color.FromArgb(35, 47, 63) : Color.FromArgb(252, 253, 255);
        var input = dark ? Color.FromArgb(24, 34, 48) : Color.White;
        var fore = dark ? Color.FromArgb(235, 241, 248) : Color.FromArgb(31, 41, 55);
        var title = dark ? Color.FromArgb(216, 232, 255) : Color.FromArgb(20, 60, 120);
        var button = dark ? Color.FromArgb(44, 59, 79) : Color.FromArgb(235, 244, 255);

        BackColor = back;
        ForeColor = fore;
        rootLayout.BackColor = panel;
        rootLayout.ForeColor = fore;
        lblTitle.BackColor = panel;
        lblTitle.ForeColor = title;
        txtHelp.BackColor = input;
        txtHelp.ForeColor = fore;
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
