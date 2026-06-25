using System.Text.Json;
using System.Text.Json.Serialization;

namespace LabFileCollector;

public sealed class AppSettings
{
    // Default καρφωτό path. Το αλλάζεις από το UI και αποθηκεύεται στο labfilecollector.settings.json.
    public string DestinationRoot { get; set; } = @"\\192.168.1.37\2026";
    public string SourceUserRoot { get; set; } = @"C:\Users\Student";
    public string LastSelectedPc { get; set; } = Environment.MachineName;
    public List<string> CustomSourcePaths { get; set; } = new();
    public string ExtensionFilter { get; set; } = ".doc;.docx;.xls;.xlsx;.ppt;.pptx;.pdf;.txt;.py;.html;.css;.js;.sb3;.glo;.psg";
    public bool IncludeAllFiles { get; set; } = false;

    // UI theme: Σύστημα / Windows, Φωτεινό / Aero, Σκούρο / Aero, Notepad++ Dark, Nord Dark, Dracula.
    public string ThemeMode { get; set; } = "Σύστημα / Windows";

    // Maintenance cleanup options. Default false για να μη διαγράφει τίποτα χωρίς επιλογή χρήστη.
    public bool CleanWindowsTemp { get; set; } = false;
    public bool CleanUserTemp { get; set; } = true;
    public bool CleanDownloadsPlannedFiles { get; set; } = false;
    public bool EmptyRecycleBin { get; set; } = false;
    public bool OptimizeSystemDrive { get; set; } = false;
    public bool CleanWindowsUpdateComponents { get; set; } = false;

    [JsonIgnore]
    public static string SettingsFilePath => Path.Combine(AppContext.BaseDirectory, "labfilecollector.settings.json");

    public static AppSettings Load()
    {
        try
        {
            if (!File.Exists(SettingsFilePath))
            {
                var defaults = new AppSettings();
                defaults.Save();
                return defaults;
            }

            var json = File.ReadAllText(SettingsFilePath);
            return JsonSerializer.Deserialize<AppSettings>(json, JsonOptions()) ?? new AppSettings();
        }
        catch
        {
            return new AppSettings();
        }
    }

    public void Save()
    {
        var json = JsonSerializer.Serialize(this, JsonOptions());
        File.WriteAllText(SettingsFilePath, json);
    }

    private static JsonSerializerOptions JsonOptions() => new()
    {
        WriteIndented = true,
        PropertyNameCaseInsensitive = true
    };
}
