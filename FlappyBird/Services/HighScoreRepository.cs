using System.Globalization;
using FlappyBird.Game;

namespace FlappyBird.Services;

/// <summary>
/// Her zorluk seviyesi için en iyi 10 skoru bir metin dosyasında saklar.
/// Dosya biçimi: her satırda "Seviye;Skor;UlaşılanSeviye;Tarih"
/// (ör. "Normal;12;3;2026-11-27T14:05:00").
/// Dosya bozuk veya erişilemez olsa bile oyun çalışmaya devam eder.
/// </summary>
public sealed class HighScoreRepository
{
    /// <summary>Her zorluk seviyesi için saklanan en fazla kayıt sayısı.</summary>
    public const int MaxEntriesPerLevel = 10;

    private const string DateFormat = "yyyy-MM-ddTHH:mm:ss";

    private readonly string filePath;
    private readonly Dictionary<DifficultyLevel, List<ScoreEntry>> tables = new();

    /// <summary>Son dosya işleminde oluşan hata mesajı (yoksa null).</summary>
    public string? LastError { get; private set; }

    public string FilePath => filePath;

    public HighScoreRepository(string? filePath = null)
    {
        this.filePath = filePath ?? Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "YMH219-FlappyBird", "highscores.txt");

        foreach (DifficultyLevel level in Enum.GetValues<DifficultyLevel>())
            tables[level] = new List<ScoreEntry>();

        LoadAll();
    }

    /// <summary>Verilen zorluk seviyesindeki en yüksek skor (kayıt yoksa 0).</summary>
    public int GetHighScore(DifficultyLevel level) =>
        tables[level].Count > 0 ? tables[level][0].Score : 0;

    /// <summary>Verilen zorluk seviyesinin skor tablosu (büyükten küçüğe sıralı).</summary>
    public IReadOnlyList<ScoreEntry> GetTopScores(DifficultyLevel level) => tables[level];

    /// <summary>
    /// Skoru tabloya eklemeyi dener. Skor 0 ise veya ilk 10'a giremiyorsa eklenmez.
    /// </summary>
    /// <returns>Tablodaki sıra (1 = birinci); tabloya giremediyse 0.</returns>
    public int AddScore(DifficultyLevel level, int score, int levelReached)
    {
        if (score <= 0)
            return 0;

        var table = tables[level];
        var entry = new ScoreEntry(score, levelReached, DateTime.Now);

        // Eşit skorlarda eski kayıt önde kalsın: ilk "daha küçük" skorun önüne ekle.
        int index = table.FindIndex(e => e.Score < score);
        if (index < 0) index = table.Count;

        if (index >= MaxEntriesPerLevel)
            return 0;

        table.Insert(index, entry);
        if (table.Count > MaxEntriesPerLevel)
            table.RemoveAt(table.Count - 1);

        SaveAll();
        return index + 1;
    }

    /// <summary>Verilen zorluk seviyesinin skor tablosunu temizler.</summary>
    public void Clear(DifficultyLevel level)
    {
        tables[level].Clear();
        SaveAll();
    }

    private void LoadAll()
    {
        LastError = null;

        try
        {
            if (!File.Exists(filePath))
                return;

            foreach (string line in File.ReadAllLines(filePath))
            {
                if (TryParseLine(line, out var level, out var entry))
                    tables[level].Add(entry);
            }

            // Dosya elle değiştirilmiş olabilir: sırala ve fazlalıkları at.
            foreach (var level in tables.Keys.ToList())
            {
                tables[level] = tables[level]
                    .OrderByDescending(e => e.Score)
                    .ThenBy(e => e.Date)
                    .Take(MaxEntriesPerLevel)
                    .ToList();
            }
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            LastError = "Skor tablosu dosyası okunamadı: " + ex.Message;
        }
    }

    /// <summary>Bir satırı çözümler; hatalı satırlarda false döner (satır atlanır).</summary>
    private static bool TryParseLine(string line, out DifficultyLevel level, out ScoreEntry entry)
    {
        entry = null!;
        string[] parts = line.Split(';', StringSplitOptions.TrimEntries);

        if (parts.Length != 4
            || !Enum.TryParse(parts[0], ignoreCase: true, out level)
            || !int.TryParse(parts[1], out int score) || score <= 0
            || !int.TryParse(parts[2], out int levelReached) || levelReached <= 0
            || !DateTime.TryParseExact(parts[3], DateFormat, CultureInfo.InvariantCulture,
                DateTimeStyles.None, out DateTime date))
        {
            level = default;
            return false;
        }

        entry = new ScoreEntry(score, levelReached, date);
        return true;
    }

    private void SaveAll()
    {
        LastError = null;

        try
        {
            string? directory = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrEmpty(directory))
                Directory.CreateDirectory(directory);

            var lines = tables.OrderBy(kv => kv.Key)
                .SelectMany(kv => kv.Value.Select(e =>
                    $"{kv.Key};{e.Score};{e.LevelReached};{e.Date.ToString(DateFormat, CultureInfo.InvariantCulture)}"));

            File.WriteAllLines(filePath, lines);
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            LastError = "Skor tablosu kaydedilemedi: " + ex.Message;
        }
    }
}
