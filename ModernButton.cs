using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace LabFileCollector;

public sealed class ModernButton : Button
{
    private bool _hover;
    private bool _pressed;

    public ModernButton()
    {
        SetStyle(ControlStyles.AllPaintingInWmPaint |
                 ControlStyles.UserPaint |
                 ControlStyles.OptimizedDoubleBuffer |
                 ControlStyles.ResizeRedraw, true);

        FlatStyle = FlatStyle.Flat;
        FlatAppearance.BorderSize = 1;
        UseVisualStyleBackColor = false;
        Cursor = Cursors.Hand;
        Padding = new Padding(10, 4, 10, 4);
    }

    protected override void OnMouseEnter(EventArgs e)
    {
        _hover = true;
        Invalidate();
        base.OnMouseEnter(e);
    }

    protected override void OnMouseLeave(EventArgs e)
    {
        _hover = false;
        _pressed = false;
        Invalidate();
        base.OnMouseLeave(e);
    }

    protected override void OnMouseDown(MouseEventArgs e)
    {
        if (e.Button == MouseButtons.Left)
        {
            _pressed = true;
            Invalidate();
        }
        base.OnMouseDown(e);
    }

    protected override void OnMouseUp(MouseEventArgs e)
    {
        _pressed = false;
        Invalidate();
        base.OnMouseUp(e);
    }

    protected override void OnEnabledChanged(EventArgs e)
    {
        Invalidate();
        base.OnEnabledChanged(e);
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
        e.Graphics.Clear(Parent?.BackColor ?? SystemColors.Control);

        var rect = ClientRectangle;
        rect.Inflate(-1, -1);
        var radius = Math.Min(10, Math.Max(4, Height / 3));

        var fill = Enabled
            ? (_pressed ? FlatAppearance.MouseDownBackColor : _hover ? FlatAppearance.MouseOverBackColor : BackColor)
            : ControlPaint.Dark(BackColor, 0.15f);
        var border = Enabled ? FlatAppearance.BorderColor : ControlPaint.Dark(FlatAppearance.BorderColor, 0.25f);
        var textColor = Enabled ? ForeColor : ControlPaint.Dark(ForeColor, 0.25f);

        using var path = RoundedRect(rect, radius);
        using var fillBrush = new SolidBrush(fill);
        using var borderPen = new Pen(border, 1f);
        e.Graphics.FillPath(fillBrush, path);
        e.Graphics.DrawPath(borderPen, path);

        var textRect = new Rectangle(6, 0, Width - 12, Height);
        TextRenderer.DrawText(
            e.Graphics,
            Text,
            Font,
            textRect,
            textColor,
            TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
    }

    private static GraphicsPath RoundedRect(Rectangle rect, int radius)
    {
        var path = new GraphicsPath();
        var diameter = radius * 2;
        var arc = new Rectangle(rect.Location, new Size(diameter, diameter));

        path.AddArc(arc, 180, 90);
        arc.X = rect.Right - diameter;
        path.AddArc(arc, 270, 90);
        arc.Y = rect.Bottom - diameter;
        path.AddArc(arc, 0, 90);
        arc.X = rect.Left;
        path.AddArc(arc, 90, 90);
        path.CloseFigure();
        return path;
    }
}
