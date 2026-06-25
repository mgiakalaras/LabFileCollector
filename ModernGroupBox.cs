using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace LabFileCollector;

public class ModernGroupBox : GroupBox
{
    public ModernGroupBox()
    {
        SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
    }

    public Color BorderColor { get; set; } = Color.FromArgb(80, 95, 115);
    public int CornerRadius { get; set; } = 12;

    protected override void OnPaint(PaintEventArgs e)
    {
        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
        e.Graphics.Clear(Parent?.BackColor ?? BackColor);

        var textSize = TextRenderer.MeasureText(Text, Font);
        var rect = new Rectangle(0, textSize.Height / 2, Width - 1, Height - textSize.Height / 2 - 1);
        var radius = Math.Min(CornerRadius, Math.Min(rect.Width, rect.Height) / 2);

        using var path = RoundedRect(rect, radius);
        using var backBrush = new SolidBrush(BackColor);
        using var borderPen = new Pen(BorderColor, 1F);
        e.Graphics.FillPath(backBrush, path);
        e.Graphics.DrawPath(borderPen, path);

        if (!string.IsNullOrWhiteSpace(Text))
        {
            var textRect = new Rectangle(14, 0, Math.Min(textSize.Width + 12, Math.Max(0, Width - 28)), textSize.Height + 2);
            using var textBack = new SolidBrush(Parent?.BackColor ?? BackColor);
            e.Graphics.FillRectangle(textBack, textRect);
            TextRenderer.DrawText(
                e.Graphics,
                Text,
                Font,
                new Rectangle(textRect.X + 4, textRect.Y, textRect.Width - 8, textRect.Height),
                ForeColor,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
        }
    }

    private static GraphicsPath RoundedRect(Rectangle bounds, int radius)
    {
        var path = new GraphicsPath();
        if (radius <= 0)
        {
            path.AddRectangle(bounds);
            return path;
        }

        var diameter = radius * 2;
        var arc = new Rectangle(bounds.Location, new Size(diameter, diameter));

        path.AddArc(arc, 180, 90);
        arc.X = bounds.Right - diameter;
        path.AddArc(arc, 270, 90);
        arc.Y = bounds.Bottom - diameter;
        path.AddArc(arc, 0, 90);
        arc.X = bounds.Left;
        path.AddArc(arc, 90, 90);
        path.CloseFigure();
        return path;
    }
}
