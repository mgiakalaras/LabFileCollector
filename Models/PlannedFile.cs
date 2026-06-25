namespace LabFileCollector;

public sealed class PlannedFile
{
    public string SourcePath { get; set; } = string.Empty;
    public string DestinationPath { get; set; } = string.Empty;
    public string SourceRoot { get; set; } = string.Empty;
    public string SourceLabel { get; set; } = string.Empty;

    // Relative path κάτω από την πηγή. Αυτό κρατάει αυτούσια τη δομή φακέλων.
    public string RelativePath { get; set; } = string.Empty;
    public string RelativeFolder { get; set; } = string.Empty;

    // Για το preview: αν είναι αρχείο χύμα στη ρίζα της πηγής ή μέσα σε υποφάκελο.
    public bool IsLooseFile { get; set; }
    public string PlacementLabel => IsLooseFile ? "Χύμα" : "Σε φάκελο";
    public string PreviewFolder => IsLooseFile ? "(ρίζα πηγής)" : RelativeFolder;

    public long SizeBytes { get; set; }
}
