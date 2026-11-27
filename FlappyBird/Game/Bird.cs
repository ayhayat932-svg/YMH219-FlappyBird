using System.Drawing.Drawing2D;

namespace FlappyBird.Game;

/// <summary>
/// Oyuncunun kontrol ettiği kuş. Yer çekimi ile aşağı düşer, kanat çırpınca yukarı çıkar.
/// </summary>
public sealed class Bird : GameObject
{
    public const float DefaultWidth = 38f;
    public const float DefaultHeight = 28f;

    /// <summary>Her karede düşme hızına eklenen yer çekimi ivmesi.</summary>
    public float Gravity { get; } = 0.42f;

    /// <summary>Kanat çırpıldığında kuşa verilen dikey hız (negatif = yukarı).</summary>
    public float FlapStrength { get; } = -7.4f;

    /// <summary>Kuşun ulaşabileceği en yüksek düşme hızı.</summary>
    public float MaxFallSpeed { get; } = 10f;

    /// <summary>Anlık dikey hız (piksel / kare).</summary>
    public float Velocity { get; private set; }

    /// <summary>Kanat animasyonu için kullanılan faz değeri.</summary>
    private float wingPhase;

    private readonly float startY;

    public Bird(float x, float y) : base(x, y, DefaultWidth, DefaultHeight)
    {
        startY = y;
    }

    /// <summary>Kuşun çizim açısı: yükselirken yukarı, düşerken aşağı bakar.</summary>
    public float Rotation => Math.Clamp(Velocity * 4.5f, -25f, 80f);

    /// <summary>
    /// Çarpışma için kullanılan kutu. Çizimden biraz küçüktür; böylece
    /// oyuncu "değmedim ama yandım" hissi yaşamaz.
    /// </summary>
    public RectangleF HitBox
    {
        get
        {
            var box = Bounds;
            box.Inflate(-4f, -4f);
            return box;
        }
    }

    /// <summary>Kuşa yukarı doğru hız verir.</summary>
    public void Flap()
    {
        Velocity = FlapStrength;
        wingPhase = 0;
    }

    /// <summary>
    /// Yer çekimini uygular ve kuşun konumunu günceller. Ekranın üstünden çıkamaz.
    /// </summary>
    public void ApplyPhysics()
    {
        Velocity = Math.Min(Velocity + Gravity, MaxFallSpeed);
        Y += Velocity;

        if (Y < 0)
        {
            Y = 0;
            Velocity = 0;
        }

        wingPhase += Velocity < 0 ? 0.6f : 0.25f;
    }

    /// <summary>Oyun başlamadan önce kuşun havada hafifçe süzülmesini sağlar.</summary>
    public void Hover(long tick)
    {
        Y = startY + (float)Math.Sin(tick * 0.08) * 6f;
        Velocity = 0;
        wingPhase += 0.3f;
    }

    /// <summary>Kuşu verilen yüksekliğe (zemin) oturtur ve durdurur.</summary>
    public void LandOn(float groundY)
    {
        Y = groundY - Height;
        Velocity = 0;
    }

    public override void Draw(Graphics g)
    {
        var state = g.Save();

        // Çizimi kuşun merkezine taşı ve hıza göre döndür.
        g.TranslateTransform(X + Width / 2f, Y + Height / 2f);
        g.RotateTransform(Rotation);

        float w = Width, h = Height;
        var body = new RectangleF(-w / 2f, -h / 2f, w, h);

        // Gövde
        using (var bodyBrush = new LinearGradientBrush(body, Color.FromArgb(255, 196, 61),
                   Color.FromArgb(238, 120, 40), LinearGradientMode.Vertical))
        {
            g.FillEllipse(bodyBrush, body);
        }

        // Karın
        using (var bellyBrush = new SolidBrush(Color.FromArgb(255, 236, 190)))
        {
            g.FillEllipse(bellyBrush, -w * 0.30f, h * 0.02f, w * 0.55f, h * 0.42f);
        }

        // Kanat (aşağı-yukarı çırpma animasyonu)
        float wingOffset = (float)Math.Sin(wingPhase) * h * 0.22f;
        using (var wingBrush = new SolidBrush(Color.FromArgb(214, 86, 40)))
        {
            g.FillEllipse(wingBrush, -w * 0.42f, -h * 0.12f + wingOffset, w * 0.42f, h * 0.36f);
        }

        // Göz
        g.FillEllipse(Brushes.White, w * 0.06f, -h * 0.40f, w * 0.30f, h * 0.42f);
        g.FillEllipse(Brushes.Black, w * 0.20f, -h * 0.28f, w * 0.11f, h * 0.18f);

        // Gaga
        using (var beakBrush = new SolidBrush(Color.FromArgb(230, 60, 50)))
        {
            PointF[] beak =
            {
                new(w * 0.36f, -h * 0.06f),
                new(w * 0.62f, h * 0.06f),
                new(w * 0.34f, h * 0.20f)
            };
            g.FillPolygon(beakBrush, beak);
        }

        // Dış çizgi
        using (var outline = new Pen(Color.FromArgb(90, 45, 20), 2f))
        {
            g.DrawEllipse(outline, body);
        }

        g.Restore(state);
    }
}
