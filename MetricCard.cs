using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace LabFileCollector;

public sealed class MetricCard : Control
{
    private string _title = "";
    private string _primaryText = "";
    private string _secondaryText = "";
    private double? _percent;
    private Color _cardBack = Color.White;
    private Color _textColor = Color.Black;
    private Color _mutedTextColor = Color.DimGray;
    private Color _barBack = Color.Gainsboro;
    private Color _barFill = Color.DodgerBlue;
    private Color _borderColor = Color.Silver;

    public MetricCard()
    {
        SetStyle(ControlStyles.AllPaintingInWmPaint |
                 ControlStyles.OptimizedDoubleBuffer |
                 ControlStyles.ResizeRedraw |
                 ControlStyles.UserPaint, true);

        Height = 76;
        Width = 270;
        Margin = new Padding(0, 5, 0, 2);
        Padding = new Padding(10);
        Font = new Font("Segoe UI", 9F, FontStyle.Regular);
    }

    public string Title
    {
        get => _title;
        set { _title = value ?? ""; Invalidate(); }
    }

    public string PrimaryText
    {
        get => _primaryText;
        set { _primaryText = value ?? ""; Invalidate(); }
    }

    public string SecondaryText
    {
        get => _secondaryText;
        set { _secondaryText = value ?? ""; Invalidate(); }
    }

    public double? Percent
    {
        get => _percent;
        set { _percent = value.HasValue ? Math.Clamp(value.Value, 0, 100) : null; Invalidate(); }
    }

    public void ApplyColors(Color cardBack, Color textColor, Color mutedTextColor, Color barBack, Color barFill, Color borderColor)
    {
        _cardBack = cardBack;
        _textColor = textColor;
        _mutedTextColor = mutedTextColor;
        _barBack = barBack;
        _barFill = barFill;
        _borderColor = borderColor;
        BackColor = cardBack;
        ForeColor = textColor;
        Invalidate();
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        var g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;

        var bounds = ClientRectangle;
        bounds.Width -= 1;
        bounds.Height -= 1;

        using var cardPath = RoundedRect(bounds, 10);
        using var cardBrush = new SolidBrush(_cardBack);
        using var borderPen = new Pen(_borderColor);
        g.FillPath(cardBrush, cardPath);
        g.DrawPath(borderPen, cardPath);

        var left = 11;
        var right = Width - 12;
        var titleRect = new Rectangle(left, 8, Math.Max(1, right - left), 18);
        var primaryRect = new Rectangle(left, 28, Math.Max(1, right - left), 20);
        var secondaryRect = new Rectangle(left, 48, Math.Max(1, right - left), 18);

        using var titleFont = new Font(Font.FontFamily, 8.5F, FontStyle.Bold);
        using var primaryFont = new Font(Font.FontFamily, 9.5F, FontStyle.Bold);
        using var secondaryFont = new Font(Font.FontFamily, 8.2F, FontStyle.Regular);

        TextRenderer.DrawText(g, _title, titleFont, titleRect, _mutedTextColor,
            TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
        TextRenderer.DrawText(g, _primaryText, primaryFont, primaryRect, _textColor,
            TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
        TextRenderer.DrawText(g, _secondaryText, secondaryFont, secondaryRect, _mutedTextColor,
            TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);

        var barRect = new Rectangle(left, Height - 13, Math.Max(1, right - left), 5);
        using var barBackPath = RoundedRect(barRect, 3);
        using var barBackBrush = new SolidBrush(_barBack);
        g.FillPath(barBackBrush, barBackPath);

        if (_percent.HasValue)
        {
            var fillWidth = (int)Math.Round(barRect.Width * (_percent.Value / 100.0));
            if (fillWidth > 0)
            {
                var fillRect = new Rectangle(barRect.X, barRect.Y, Math.Max(2, fillWidth), barRect.Height);
                using var fillPath = RoundedRect(fillRect, 3);
                using var fillBrush = new SolidBrush(_barFill);
                g.FillPath(fillBrush, fillPath);
            }
        }
    }

    private static GraphicsPath RoundedRect(Rectangle bounds, int radius)
    {
        var path = new GraphicsPath();
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
