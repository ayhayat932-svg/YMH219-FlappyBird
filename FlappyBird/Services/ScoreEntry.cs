namespace FlappyBird.Services;

/// <summary>
/// Skor tablosundaki tek bir kayıt.
/// </summary>
/// <param name="Score">Oyunda yapılan skor.</param>
/// <param name="LevelReached">Oyunda ulaşılan seviye.</param>
/// <param name="Date">Oyunun bittiği tarih ve saat.</param>
public sealed record ScoreEntry(int Score, int LevelReached, DateTime Date);
