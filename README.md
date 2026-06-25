# School Lab File Collector

Portable Windows Forms εργαλείο για συντήρηση σχολικού εργαστηρίου και συλλογή αρχείων μαθητών από κοινά paths σε δικτυακό ή τοπικό προορισμό.

## Βασική ιδέα

Η εφαρμογή σαρώνει επιλεγμένες πηγές, δείχνει preview, επιτρέπει στον χρήστη να επιλέξει ποια αρχεία ή ποιοι φάκελοι θα περάσουν, και έπειτα κάνει ασφαλή αντιγραφή ή μεταφορά στον τελικό φάκελο του συγκεκριμένου υπολογιστή.

Παράδειγμα προορισμού:

```text
\\192.168.1.37\shared\Εργασίες Μαθητών\Πληροφορική Γ Λυκείου\2026\LAB-PC01
```

## Δυνατότητες

- Αυτόματη ανάγνωση ονόματος υπολογιστή από Windows.
- Editable root προορισμού.
- Αυτόματες πηγές από το τρέχον Windows user profile ή χειροκίνητο `C:\Users\...` path.
- Desktop, Documents, Downloads, Pictures και custom sources.
- Υποστήριξη αρχείων `.glo` και `.psg`.
- Αναδρομική σάρωση και διατήρηση δομής φακέλων.
- Preview με ένδειξη `Χύμα` ή `Σε φάκελο`.
- Checkboxes για επιλογή συγκεκριμένων αρχείων.
- Επιλογή/απόρριψη ολόκληρου φακέλου από το preview.
- Copy ή Move με ασφαλή διαγραφή πηγής μόνο μετά από επιτυχημένη αντιγραφή.
- Stop με επιβεβαίωση.
- Undo τελευταίας αντιγραφής/μεταφοράς για την τρέχουσα συνεδρία.
- Ιστορικό/logs από CSV στον φάκελο `_logs`.
- Ξεχωριστό popup συντήρησης υπολογιστή.
- Προαιρετικό cleanup Temp, Downloads του plan, Recycle Bin, DISM Windows Update cleanup και Optimize/TRIM.
- Sidebar με live CPU/RAM/δίσκους/δίκτυο, λειτουργικό, PC name και logged-in user.
- Themes: Windows/System, Aero Light/Dark, Notepad++ Dark, Nord Dark, Dracula.
- Branding: Nyx Systems / School Lab File Collector.

## Ροή χρήσης

1. Έλεγξε τον προορισμό.
2. Έλεγξε το user root των πηγών ή πάτα `Τρέχων`.
3. Διάλεξε πηγές και φίλτρο αρχείων.
4. Πάτα `Σάρωση`.
5. Στο preview κράτα τσεκαρισμένα μόνο όσα θέλεις.
6. Για ολόκληρο φάκελο μαθητή, επίλεξε ένα αρχείο από τον φάκελο και πάτα `Επιλογή ίδιου φακέλου`.
7. Πάτα `Εκτέλεση` για Copy ή Move.

## Portable build

```powershell
dotnet publish .\LabFileCollector.csproj -c Release -r win-x64 --self-contained true /p:PublishSingleFile=true /p:EnableCompressionInSingleFile=true
```

Το αποτέλεσμα βρίσκεται στο:

```text
bin\Release\net8.0-windows\win-x64\publish
```

## Σημείωση admin

Η εφαρμογή ζητά δικαιώματα διαχειριστή στην εκκίνηση, ώστε οι λειτουργίες cleanup/maintenance να μπορούν να τρέξουν χωρίς να κλείσει και να ξανανοίξει ο χρήστης την εφαρμογή.

## Copyright

© 2026 - Nyx Systems / School Lab File Collector
