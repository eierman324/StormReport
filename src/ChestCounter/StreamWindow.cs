using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Runtime.Versioning;
using System.Windows.Forms;

namespace ChestCounter;

/// <summary>
/// The overlay as an ordinary window, for streaming apps that cannot show an HTML page (TikTok LIVE Studio):
/// they add it with "Window capture". Normal window (taskbar, not on top, not layered, drawn with GDI+), so every
/// capture method sees it. The figures scale with the window: make it bigger to get sharper text on the stream.
/// Background "dark" (the Obsidian card, crop nothing) or "green" (#00FF00, to cut out with a chroma key).
/// </summary>
[SupportedOSPlatform("windows")]
public static class StreamWindow
{
    public static readonly string[] Backgrounds = { "dark", "green" };

    private static Thread? _thread;
    private static StreamForm? _form;
    private static volatile OverlayView _view = new("0", null, OverlayModel.LastMatchCaption, false, null, Array.Empty<OverlayRow>());
    private static volatile string _background = "dark";

    public static bool IsRunning => _form != null;

    /// <summary>Opens the window (own UI thread). <paramref name="onClosed"/> runs when the player closes it with its X.</summary>
    public static void Start(string background, Rectangle? bounds, Action<Rectangle> onMoved, Action onClosed)
    {
        if (_thread != null)
        {
            return;
        }
        SetBackground(background);
        var ready = new ManualResetEventSlim();
        _thread = new Thread(() =>
        {
            try
            {
                Application.SetHighDpiMode(HighDpiMode.PerMonitorV2);
                var form = new StreamForm(bounds, onMoved, onClosed);
                _form = form;
                form.Shown += (_, _) => ready.Set();
                Application.Run(form);
            }
            catch (Exception ex)
            {
                Console.WriteLine(L.T($"Fenêtre de stream indisponible : {ex.Message}", $"Stream window unavailable: {ex.Message}", $"Ventana de directo no disponible: {ex.Message}"));
            }
            finally
            {
                _form = null;
                _thread = null;
                ready.Set();
            }
        })
        {
            IsBackground = true,
            Name = "Stream window",
        };
        _thread.SetApartmentState(ApartmentState.STA);
        _thread.Start();
        ready.Wait(TimeSpan.FromSeconds(5));
    }

    /// <summary>Closes the window from the counter (setting turned off, app closing).</summary>
    public static void Stop()
    {
        var thread = _thread;
        if (_form is { } f)
        {
            try { f.BeginInvoke(() => { f.ClosingFromApp = true; f.Close(); }); } catch { /* already gone */ }
        }
        thread?.Join(TimeSpan.FromSeconds(3));
    }

    public static void Update(OverlayView view)
    {
        _view = view;
        Repaint();
    }

    public static void SetBackground(string background)
    {
        _background = Array.IndexOf(Backgrounds, background) >= 0 ? background : "dark";
        Repaint();
    }

    private static void Repaint()
    {
        if (_form is { IsHandleCreated: true } f)
        {
            try { f.BeginInvoke(f.Invalidate); } catch { /* closing */ }
        }
    }

    private sealed class StreamForm : Form
    {
        private readonly Action<Rectangle> _onMoved;
        private readonly Action _onClosed;
        public bool ClosingFromApp;

        private static readonly FontFamily Mono = PickFont();
        private static readonly Color Bg = Color.FromArgb(15, 15, 15), Border = Color.FromArgb(40, 40, 40),
            Accent = Color.FromArgb(140, 105, 255), AccentLight = Color.FromArgb(166, 139, 255), Live = Color.FromArgb(47, 210, 138),
            Grey = Color.FromArgb(136, 136, 136), Key = Color.FromArgb(0, 255, 0), Ink = Color.FromArgb(11, 15, 24);

        public StreamForm(Rectangle? bounds, Action<Rectangle> onMoved, Action onClosed)
        {
            _onMoved = onMoved;
            _onClosed = onClosed;
            Text = AppInfo.Name + " – Stream";
            try { Icon = Icon.ExtractAssociatedIcon(Environment.ProcessPath!); } catch { /* default icon */ }
            FormBorderStyle = FormBorderStyle.Sizable;
            MinimizeBox = true;
            MaximizeBox = false;
            ShowInTaskbar = true;
            DoubleBuffered = true;
            SetStyle(ControlStyles.ResizeRedraw | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
            MinimumSize = new Size(220, 120);
            StartPosition = FormStartPosition.Manual;
            var b = bounds ?? new Rectangle(80, 80, 0, 0);
            ClientSize = new Size(b.Width > 0 ? b.Width : 400, b.Height > 0 ? b.Height : 230);
            Location = b.Location;
            // Never off screen (a monitor unplugged since).
            if (!Screen.AllScreens.Any(s => s.WorkingArea.IntersectsWith(new Rectangle(Location, Size))))
            {
                Location = new Point(80, 80);
            }
        }

        private static FontFamily PickFont()
        {
            using var fonts = new InstalledFontCollection();
            foreach (var name in new[] { "Cascadia Mono", "Consolas" })
            {
                if (fonts.Families.Any(f => f.Name == name))
                {
                    return new FontFamily(name);
                }
            }
            return FontFamily.GenericMonospace;
        }

        protected override void OnResizeEnd(EventArgs e)
        {
            base.OnResizeEnd(e);
            _onMoved(new Rectangle(Location, ClientSize));
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            base.OnFormClosed(e);
            if (!ClosingFromApp)
            {
                _onClosed();
            }
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            // everything is drawn in OnPaint (no flicker)
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            var green = _background == "green";
            g.Clear(green ? Key : Bg);
            // Green: no anti-aliasing at all. Every pixel is either pure key green or a solid color; blended edge pixels
            // (half green, half dark) survive the chroma key as dark-green specks around the text.
            g.SmoothingMode = green ? SmoothingMode.None : SmoothingMode.AntiAlias;
            g.PixelOffsetMode = green ? PixelOffsetMode.None : PixelOffsetMode.HighQuality;
            g.TextRenderingHint = green ? TextRenderingHint.SingleBitPerPixelGridFit : TextRenderingHint.AntiAlias;

            var v = _view;
            var state = Rows(v);
            var designH = 112 + state.Count * 30 + 14;
            using var fHeader = new Font(Mono, 13, FontStyle.Regular, GraphicsUnit.Pixel);
            using var fBig = new Font(Mono, 46, FontStyle.Bold, GraphicsUnit.Pixel);
            using var fCap = new Font(Mono, 12, FontStyle.Regular, GraphicsUnit.Pixel);
            using var fRow = new Font(Mono, 16, FontStyle.Regular, GraphicsUnit.Pixel);
            using var fVal = new Font(Mono, 16, FontStyle.Bold, GraphicsUnit.Pixel);
            float W(string t, Font f) => g.MeasureString(t, f, PointF.Empty, StringFormat.GenericTypographic).Width;
            var cur = v.Current ?? "—";

            // Narrowest the content can be (design units): chest + total ... current; the header; the widest row.
            var contentW = Math.Max(Math.Max(42 + W(v.Total, fBig) + 28 + W(cur, fBig),
                    W(L.T("COFFRES", "CHESTS", "COFRES"), fHeader) + 24 + (v.Badge != null ? W(v.Badge, fHeader) : 0)),
                state.Max(r => W(r.Label, fRow) + 40 + W(r.Value, fVal)));

            // The window height sets the size (a narrow window makes it smaller so nothing is cut), and the content spans
            // the whole width: left items on the left edge, right items on the right edge, edgePx pixels from it.
            const float edgePx = 10;
            float cw = ClientSize.Width, ch = ClientSize.Height;
            var scale = Math.Min(ch / designH, (cw - 2 * edgePx) / contentW);
            if (scale <= 0)
            {
                return;
            }

            if (!green)
            {
                using var border = new Pen(Border, 1.5f);
                using var card = Rounded(new RectangleF(1, 1, cw - 3, ch - 3), 10);
                using var fill = new SolidBrush(Bg);
                g.FillPath(fill, card);
                g.DrawPath(border, card);
            }

            g.TranslateTransform(0, (ch - designH * scale) / 2);
            g.ScaleTransform(scale, scale);
            var pad = edgePx / scale;
            var right = cw / scale - pad;

            // Header: CHESTS ............ ● LIVE
            DrawText(g, L.T("COFFRES", "CHESTS", "COFRES"), fHeader, green ? Color.White : Color.FromArgb(153, 153, 153), pad, 16, green);
            if (v.Badge != null)
            {
                DrawText(g, v.Badge, fHeader, Live, right - W(v.Badge, fHeader), 16, green);
            }

            // Big total with the chest, and the match in progress / last match on the right.
            DrawChest(g, pad, 52, 34, 29);
            DrawText(g, v.Total, fBig, green ? Color.White : Accent, pad + 42, 38, green);
            DrawText(g, "total", fCap, green ? Color.White : Grey, pad + 44, 92, green);

            var curW = g.MeasureString(cur, fBig, PointF.Empty, StringFormat.GenericTypographic).Width;
            var capW = g.MeasureString(v.CurrentCaption, fCap, PointF.Empty, StringFormat.GenericTypographic).Width;
            var curColor = v.Current == null ? Color.FromArgb(90, 90, 90) : v.CurrentLive ? Live : Color.White;
            DrawText(g, cur, fBig, curColor, right - curW, 38, green);
            DrawText(g, v.CurrentCaption, fCap, green ? Color.White : Grey, right - capW, 92, green);

            // Rows: session, squad, treasures.
            var y = 118f;
            if (!green)
            {
                using var line = new Pen(Border, 1);
                g.DrawLine(line, pad, y, right, y);
            }
            y += 8;
            foreach (var (label, value, treasure) in state)
            {
                DrawText(g, label, fRow, green ? Color.White : Grey, pad, y, green);
                var vw = g.MeasureString(value, fVal, PointF.Empty, StringFormat.GenericTypographic).Width;
                if (treasure)
                {
                    DrawStar(g, right - vw - 14, y + 10, 8);
                }
                DrawText(g, value, fVal, treasure ? AccentLight : Color.White, right - vw, y, green);
                y += 30;
            }
        }

        private static List<(string Label, string Value, bool Treasure)> Rows(OverlayView v)
        {
            var rows = new List<(string, string, bool)> { (L.T("Session", "Session", "Sesión"), v.Session, false) };
            if (v.Squad != null)
            {
                rows.Add((OverlayModel.SquadLabel, v.Squad, false));
            }
            rows.Add((L.T("Trésors", "Treasures", "Tesoros"), v.Treasures, true));
            return rows;
        }

        /// <summary>Text; on the green background with a dark outline so it stays readable once keyed out.</summary>
        private static void DrawText(Graphics g, string text, Font font, Color color, float x, float y, bool outlined)
        {
            if (outlined)
            {
                // Winding: the glyphs of Cascadia Mono are made of overlapping contours; the default alternate fill leaves
                // holes where they overlap, and stroking the path draws lines inside the letters. Instead, the outline is the
                // filled glyph stamped around in a ring (like the in-game HUD), then the glyph on top.
                using var path = new GraphicsPath(FillMode.Winding);
                path.AddString(text, font.FontFamily, (int)font.Style, font.Size, new PointF(x, y), StringFormat.GenericTypographic);
                var o = Math.Max(1.5f, font.Size / 14f);
                using var ink = new SolidBrush(Ink);
                const int steps = 16;
                for (var i = 0; i < steps; i++)
                {
                    var a = i * Math.PI * 2 / steps;
                    var state = g.Save();
                    g.TranslateTransform((float)(o * Math.Cos(a)), (float)(o * Math.Sin(a)) + o * 0.25f);
                    g.FillPath(ink, path);
                    g.Restore(state);
                }
                using var b = new SolidBrush(color);
                g.FillPath(b, path);
                return;
            }
            using var brush = new SolidBrush(color);
            g.DrawString(text, font, brush, x, y, StringFormat.GenericTypographic);
        }

        private static void DrawChest(Graphics g, float x, float y, float w, float h)
        {
            using var pen = new Pen(Ink, 1.6f) { LineJoin = LineJoin.Round };
            var lidH = h * 0.42f;
            using var lid = new GraphicsPath();
            lid.AddArc(x, y, w, lidH * 2, 180, 180);
            lid.CloseFigure();
            using var bLid = new SolidBrush(Color.FromArgb(98, 62, 230));
            g.FillPath(bLid, lid);
            g.DrawPath(pen, lid);
            using var body = Rounded(new RectangleF(x, y + lidH, w, h - lidH), 3);
            using var bBody = new SolidBrush(Color.FromArgb(125, 85, 255));
            g.FillPath(bBody, body);
            g.DrawPath(pen, body);
            using var latch = new SolidBrush(Ink);
            var lw = w / 5;
            g.FillRectangle(latch, x + (w - lw) / 2, y + lidH - 2, lw, h / 4 + 2);
        }

        private static void DrawStar(Graphics g, float cx, float cy, float r)
        {
            var pts = new PointF[10];
            for (var i = 0; i < 10; i++)
            {
                var a = -Math.PI / 2 + i * Math.PI / 5;
                var rr = i % 2 == 0 ? r : r * 0.45;
                pts[i] = new PointF(cx + (float)(rr * Math.Cos(a)), cy + (float)(rr * Math.Sin(a)));
            }
            using var fill = new SolidBrush(Color.FromArgb(125, 85, 255));
            using var pen = new Pen(Ink, 1.4f) { LineJoin = LineJoin.Round };
            g.FillPolygon(fill, pts);
            g.DrawPolygon(pen, pts);
        }

        private static GraphicsPath Rounded(RectangleF r, float radius)
        {
            var d = radius * 2;
            var p = new GraphicsPath();
            p.AddArc(r.Left, r.Top, d, d, 180, 90);
            p.AddArc(r.Right - d, r.Top, d, d, 270, 90);
            p.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90);
            p.AddArc(r.Left, r.Bottom - d, d, d, 90, 90);
            p.CloseFigure();
            return p;
        }
    }
}
