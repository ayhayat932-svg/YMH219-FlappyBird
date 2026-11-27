namespace FlappyBird.Game;

/// <summary>
/// Oyunun içinde bulunabileceği durumlar.
/// </summary>
public enum GameState
{
    /// <summary>Oyun başlamayı bekliyor (kuş havada süzülüyor).</summary>
    Ready,

    /// <summary>Oyun oynanıyor.</summary>
    Playing,

    /// <summary>Oyun duraklatıldı.</summary>
    Paused,

    /// <summary>Kuş bir engele veya zemine çarptı.</summary>
    GameOver
}
