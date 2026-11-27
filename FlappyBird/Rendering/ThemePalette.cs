namespace FlappyBird.Rendering;

/// <summary>
/// Arka plan için renk teması (gündüz / gece). Sadece görünümü değiştirir, oyun kurallarını etkilemez.
/// </summary>
public sealed class ThemePalette
{
    public string Name { get; }
    public Color SkyTop { get; }
    public Color SkyBottom { get; }
    public Color Cloud { get; }
    public Color FarHills { get; }
    public Color NearHills { get; }

    /// <summary>Gece temasında yıldız ve ay çizilir.</summary>
    public bool ShowStars { get; }

    private ThemePalette(string name, Color skyTop, Color skyBottom, Color cloud,
        Color farHills, Color nearHills, bool showStars)
    {
        Name = name;
        SkyTop = skyTop;
        SkyBottom = skyBottom;
        Cloud = cloud;
        FarHills = farHills;
        NearHills = nearHills;
        ShowStars = showStars;
    }

    public static ThemePalette Day { get; } = new("Gündüz",
        Color.FromArgb(92, 172, 238), Color.FromArgb(196, 232, 250),
        Color.FromArgb(235, 255, 255, 255),
        Color.FromArgb(140, 200, 170), Color.FromArgb(110, 180, 140), showStars: false);

    public static ThemePalette Night { get; } = new("Gece",
        Color.FromArgb(14, 22, 58), Color.FromArgb(58, 70, 130),
        Color.FromArgb(90, 200, 210, 240),
        Color.FromArgb(40, 70, 90), Color.FromArgb(30, 58, 70), showStars: true);
}
