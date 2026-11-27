namespace FlappyBird.Game;

/// <summary>
/// Zorluk seviyeleri.
/// </summary>
public enum DifficultyLevel
{
    Easy,
    Normal,
    Hard
}

/// <summary>
/// Bir zorluk seviyesine ait oyun ayarlarını tutar (başlangıç hızı, boşluk yüksekliği vb.).
/// Nesneler yalnızca <see cref="For"/> metodu ile oluşturulabilir.
/// </summary>
public sealed class DifficultySettings
{
    public DifficultyLevel Level { get; }
    public string DisplayName { get; }

    /// <summary>Engellerin başlangıçtaki yatay hızı (piksel / kare).</summary>
    public float BaseSpeed { get; }

    /// <summary>Her seviye atlandığında hıza eklenecek miktar.</summary>
    public float SpeedIncreasePerLevel { get; }

    /// <summary>Hızın çıkabileceği en yüksek değer.</summary>
    public float MaxSpeed { get; }

    /// <summary>Üst ve alt engel arasındaki boşluğun yüksekliği (piksel).</summary>
    public int GapHeight { get; }

    private DifficultySettings(DifficultyLevel level, string displayName, float baseSpeed,
        float speedIncreasePerLevel, float maxSpeed, int gapHeight)
    {
        Level = level;
        DisplayName = displayName;
        BaseSpeed = baseSpeed;
        SpeedIncreasePerLevel = speedIncreasePerLevel;
        MaxSpeed = maxSpeed;
        GapHeight = gapHeight;
    }

    public static DifficultySettings For(DifficultyLevel level) => level switch
    {
        DifficultyLevel.Easy => new DifficultySettings(level, "Kolay", 2.2f, 0.25f, 4.0f, 175),
        DifficultyLevel.Normal => new DifficultySettings(level, "Normal", 2.8f, 0.35f, 5.5f, 150),
        DifficultyLevel.Hard => new DifficultySettings(level, "Zor", 3.4f, 0.45f, 7.0f, 128),
        _ => throw new ArgumentOutOfRangeException(nameof(level), level, "Bilinmeyen zorluk seviyesi.")
    };
}
