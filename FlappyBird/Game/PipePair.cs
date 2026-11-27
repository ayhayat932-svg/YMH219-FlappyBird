using System.Drawing.Drawing2D;

namespace FlappyBird.Game;

/// <summary>
/// Üstte ve altta birer sütundan oluşan engel çifti. Aradaki boşluktan kuşun geçmesi gerekir.
/// Ekranda sağdan sola hareket eder; ekran dışına çıkınca yeniden konumlandırılır.
/// </summary>
public sealed class PipePair : GameObject
{
    public const float PipeWidth = 70f;
    private const float CapHeight = 26f;
    private const float CapOverhang = 5f;

    /// <summary>Boşluğun üst kenarının Y koordinatı.</summary>
    public float GapY { get; private set; }

    /// <summary>Boşluğun yüksekliği.</summary>
    public float GapHeight { get; private set; }

    /// <summary>Kuş bu engeli geçtiyse true olur (skor bir kez artsın diye).</summary>
    public bool IsPassed { get; set; }

    /// <param name="x">Engelin sol kenarı.</param>
    /// <param name="gapY">Boşluğun üst kenarı.</param>
    /// <param name="gapHeight">Boşluk yüksekliği.</param>
    /// <param name="playAreaHeight">Zeminin üstünde kalan oyun alanı yüksekliği.</param>
    public PipePair(float x, float gapY, float gapHeight, float playAreaHeight)
        : base(x, 0, PipeWidth, playAreaHeight)
    {
        Reposition(x, gapY, gapHeight);
    }

    public RectangleF TopRect => new(X, 0, Width, GapY);

    public RectangleF BottomRect => new(X, GapY + GapHeight, Width, Height - (GapY + GapHeight));

    /// <summary>Engel tamamen ekranın solundan çıktı mı?</summary>
    public bool IsOffScreen => X + Width < 0;

    /// <summary>Engeli verilen hız kadar sola kaydırır.</summary>
    public void Move(float speed) => X -= speed;

    /// <summary>Engeli yeni bir konuma ve yeni bir boşluk yüksekliğine taşır.</summary>
    public void Reposition(float x, float gapY, float gapHeight)
    {
        if (gapHeight <= 0 || gapY < 0 || gapY + gapHeight > Height)
            throw new ArgumentOutOfRangeException(nameof(gapY), "Boşluk oyun alanının dışında kalamaz.");

        X = x;
        GapY = gapY;
        GapHeight = gapHeight;
        IsPassed = false;
    }

    /// <summary>Verilen dikdörtgen üst veya alt sütuna değiyor mu?</summary>
    public bool CollidesWith(RectangleF rect) =>
        rect.IntersectsWith(TopRect) || rect.IntersectsWith(BottomRect);

    public override void Draw(Graphics g)
    {
        DrawColumn(g, TopRect, capAtBottom: true);
        DrawColumn(g, BottomRect, capAtBottom: false);
    }

    private static void DrawColumn(Graphics g, RectangleF column, bool capAtBottom)
    {
        if (column.Height <= 0) return;

        using var bodyBrush = new LinearGradientBrush(
            new RectangleF(column.X, 0, column.Width, 1),
            Color.FromArgb(70, 160, 120), Color.FromArgb(30, 100, 80), LinearGradientMode.Horizontal);
        using var shine = new SolidBrush(Color.FromArgb(70, 255, 255, 255));
        using var outline = new Pen(Color.FromArgb(20, 60, 50), 2f);

        g.FillRectangle(bodyBrush, column);
        g.FillRectangle(shine, column.X + 8, column.Y, 8, column.Height);
        g.DrawRectangle(outline, column.X, column.Y, column.Width, column.Height);

        // Sütunun boşluğa bakan ucundaki başlık
        float capY = capAtBottom ? column.Bottom - CapHeight : column.Top;
        var cap = new RectangleF(column.X - CapOverhang, capY, column.Width + CapOverhang * 2, CapHeight);

        using var capBrush = new LinearGradientBrush(
            new RectangleF(cap.X, 0, cap.Width, 1),
            Color.FromArgb(90, 190, 140), Color.FromArgb(35, 115, 90), LinearGradientMode.Horizontal);

        g.FillRectangle(capBrush, cap);
        g.FillRectangle(shine, cap.X + 8, cap.Y, 8, cap.Height);
        g.DrawRectangle(outline, cap.X, cap.Y, cap.Width, cap.Height);
    }
}
