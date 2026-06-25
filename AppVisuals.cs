using System.Drawing;

namespace LabFileCollector;

public static class AppVisuals
{
    public static string AssetsDirectory => Path.Combine(AppContext.BaseDirectory, "Assets");
    public static string IconPath => Path.Combine(AssetsDirectory, "LabFileCollector.ico");
    public static string LogoPath => Path.Combine(AssetsDirectory, "LabFileCollector_logo.png");
    public static string NyxSystemsLogoPath => Path.Combine(AssetsDirectory, "NyxSystems_sidebar_logo.png");

    public static Icon? LoadAppIcon()
    {
        try
        {
            return File.Exists(IconPath) ? new Icon(IconPath) : null;
        }
        catch
        {
            return null;
        }
    }

    public static Image? LoadLogoImage()
    {
        return LoadImage(LogoPath);
    }

    public static Image? LoadNyxSystemsLogoImage()
    {
        return LoadImage(NyxSystemsLogoPath);
    }

    private static Image? LoadImage(string path)
    {
        try
        {
            if (!File.Exists(path))
                return null;

            using var stream = File.OpenRead(path);
            using var image = Image.FromStream(stream);
            return new Bitmap(image);
        }
        catch
        {
            return null;
        }
    }
}
