using FlappyBird.Game;

namespace FlappyBird.Services;

/// <summary>
/// Her zorluk seviyesi için en yüksek skoru bir metin dosyasında saklar.
/// Dosya biçimi: her satırda "Seviye=Skor" (ör. "Normal=12").
/// Dosya bozuk veya erişilemez olsa bile oyun çalışmaya devam eder.
/// </summary>
public sealed class HighScoreRepository
{
    private readonly string filePath;
    private readonly Dictionary<DifficultyLevel, int> scores = new();

    /// <summary>Son dosya işleminde oluşan hata mesajı (yoksa null).</summary>
    public string? LastError { get; private set; }

    public string FilePath => filePath;

    public HighScoreRepository(string? filePath = null)
    {
        this.filePath = filePath ?? Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "YMH219-FlappyBird", "highscores.txt");

        LoadAll();
    }

    public int GetHighScore(DifficultyLevel level) =>
        scores.TryGetValue(level, out int score) ? score : 0;

    /// <summary>
    /// Skor mevcut rekordan büyükse kaydeder.
    /// </summary>
    /// <returns>Yeni rekor kırıldıysa true.</returns>
    public bool TrySaveHighScore(DifficultyLevel level, int score)
    {
        if (score <= GetHighScore(level))
            return false;

        scores[level] = score;
        SaveAll();
        return true;
    }

    private void LoadAll()
    {
        scores.Clear();
        LastError = null;

        try
        {
            if (!File.Exists(filePath))
                return;

            foreach (string line in File.ReadAllLines(filePath))
            {
                string[] parts = line.Split('=', 2, StringSplitOptions.TrimEntries);

                // Hatalı satırları atla, tüm dosyayı geçersiz sayma.
                if (parts.Length == 2
                    && Enum.TryParse(parts[0], ignoreCase: true, out DifficultyLevel level)
                    && int.TryParse(parts[1], out int score)
                    && score >= 0)
                {
                    scores[level] = score;
                }
            }
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            LastError = "En yüksek skor dosyası okunamadı: " + ex.Message;
        }
    }

    private void SaveAll()
    {
        LastError = null;

        try
        {
            string? directory = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrEmpty(directory))
                Directory.CreateDirectory(directory);

            var lines = scores.OrderBy(kv => kv.Key).Select(kv => $"{kv.Key}={kv.Value}");
            File.WriteAllLines(filePath, lines);
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            LastError = "En yüksek skor kaydedilemedi: " + ex.Message;
        }
    }
}
