using FlappyBird.Services;

namespace FlappyBird.Game;

/// <summary>
/// Oyunun tüm mantığını yöneten sınıf: kuş, engeller, skor, hız, seviye ve oyun durumu.
/// Arayüzden bağımsızdır; form yalnızca <see cref="Update"/> metodunu her Timer
/// tikinde çağırır ve olaylara (event) abone olarak ekranı günceller.
/// </summary>
public sealed class GameEngine
{
    // ---- Dünya ölçüleri (piksel) ----
    public const int WorldWidth = 400;
    public const int WorldHeight = 600;
    public const int GroundHeight = 90;
    public const int PlayAreaHeight = WorldHeight - GroundHeight;

    // ---- Oyun ayarları ----
    private const float BirdStartX = 90f;
    private const float BirdStartY = 240f;
    private const int PipeCount = 3;            // Ekranda aynı anda bulunan engel sayısı (en az 2)
    private const float PipeSpacing = 220f;     // Ardışık engeller arası yatay mesafe
    private const float FirstPipeX = WorldWidth + 80f;
    private const int MinGapMargin = 50;        // Boşluğun ekran üstüne/zemine olan en küçük mesafesi
    private const int PointsPerLevel = 5;       // Kaç puanda bir seviye atlanır
    private const int RestartDelayTicks = 35;   // Game Over'dan sonra yanlışlıkla yeniden başlamayı önler

    private readonly Random random;
    private readonly HighScoreRepository highScores;
    private readonly List<PipePair> pipes = new();
    private long gameOverTick;

    public GameEngine(HighScoreRepository highScores, DifficultyLevel difficulty = DifficultyLevel.Normal,
        Random? random = null)
    {
        this.highScores = highScores ?? throw new ArgumentNullException(nameof(highScores));
        this.random = random ?? new Random();
        Difficulty = DifficultySettings.For(difficulty);
        Bird = new Bird(BirdStartX, BirdStartY);
        NewGame();
    }

    // ---- Durum bilgileri (özellikler) ----
    public Bird Bird { get; private set; }
    public IReadOnlyList<PipePair> Pipes => pipes;
    public GameState State { get; private set; }
    public DifficultySettings Difficulty { get; private set; }
    public int Score { get; private set; }
    public int HighScore => highScores.GetHighScore(Difficulty.Level);
    public int Level { get; private set; }
    public float Speed { get; private set; }
    public bool IsNewHighScore { get; private set; }

    /// <summary>Oyun başladığından beri geçen kare sayısı (animasyonlar için).</summary>
    public long TickCount { get; private set; }

    /// <summary>Zeminin kayma miktarı (zeminin hareket ediyormuş gibi görünmesi için).</summary>
    public float GroundOffset { get; private set; }

    /// <summary>Game Over ekranında yeniden başlatmaya izin verilip verilmediği.</summary>
    public bool CanRestart => State == GameState.GameOver && TickCount - gameOverTick >= RestartDelayTicks;

    /// <summary>Son kayıt hatası (en yüksek skor dosyası yazılamadıysa).</summary>
    public string? StorageError => highScores.LastError;

    // ---- Olaylar ----
    public event EventHandler? StateChanged;
    public event EventHandler? ScoreChanged;
    public event EventHandler? LevelChanged;
    public event EventHandler? Flapped;
    public event EventHandler? Crashed;

    /// <summary>Oyunu başlangıç durumuna getirir (Yeni Oyun / Yeniden Başlat).</summary>
    public void NewGame()
    {
        Bird = new Bird(BirdStartX, BirdStartY);
        Score = 0;
        Level = 1;
        Speed = Difficulty.BaseSpeed;
        IsNewHighScore = false;
        CreatePipes();

        ScoreChanged?.Invoke(this, EventArgs.Empty);
        LevelChanged?.Invoke(this, EventArgs.Empty);
        ChangeState(GameState.Ready);
    }

    /// <summary>Zorluk seviyesini değiştirir ve oyunu sıfırlar.</summary>
    public void SetDifficulty(DifficultyLevel level)
    {
        Difficulty = DifficultySettings.For(level);
        NewGame();
    }

    /// <summary>
    /// Oyuncunun "zıpla" girdisi. Duruma göre oyunu başlatır, kuşu zıplatır
    /// veya Game Over ekranındaysa oyunu yeniden başlatır.
    /// </summary>
    public void Flap()
    {
        switch (State)
        {
            case GameState.Ready:
                ChangeState(GameState.Playing);
                Bird.Flap();
                Flapped?.Invoke(this, EventArgs.Empty);
                break;

            case GameState.Playing:
                Bird.Flap();
                Flapped?.Invoke(this, EventArgs.Empty);
                break;

            case GameState.GameOver when CanRestart:
                NewGame();
                break;
        }
    }

    /// <summary>Oynanıyorsa duraklatır, duraklatılmışsa devam ettirir.</summary>
    public void TogglePause()
    {
        if (State == GameState.Playing)
            ChangeState(GameState.Paused);
        else if (State == GameState.Paused)
            ChangeState(GameState.Playing);
    }

    /// <summary>Oyun döngüsünün tek bir adımı. Timer'ın her tikinde çağrılır.</summary>
    public void Update()
    {
        if (State == GameState.Paused)
            return;

        TickCount++;

        switch (State)
        {
            case GameState.Ready:
                Bird.Hover(TickCount);
                ScrollGround();
                break;

            case GameState.Playing:
                UpdatePlaying();
                break;

            case GameState.GameOver:
                // Kuş çarptıktan sonra zemine düşmeye devam eder.
                if (Bird.Bounds.Bottom < PlayAreaHeight)
                {
                    Bird.ApplyPhysics();
                    if (Bird.Bounds.Bottom >= PlayAreaHeight)
                        Bird.LandOn(PlayAreaHeight);
                }
                break;
        }
    }

    private void UpdatePlaying()
    {
        Bird.ApplyPhysics();
        ScrollGround();

        foreach (var pipe in pipes)
        {
            pipe.Move(Speed);

            // Ekran dışına çıkan engeli en sağdaki engelin arkasına taşı.
            if (pipe.IsOffScreen)
                RecyclePipe(pipe);

            // Kuş engelin sağ kenarını geçtiyse skor artar.
            if (!pipe.IsPassed && pipe.X + pipe.Width < Bird.X)
            {
                pipe.IsPassed = true;
                AddPoint();
            }

            if (pipe.CollidesWith(Bird.HitBox))
            {
                EndGame();
                return;
            }
        }

        // Zemine çarpma kontrolü
        if (Bird.Bounds.Bottom >= PlayAreaHeight)
        {
            Bird.LandOn(PlayAreaHeight);
            EndGame();
        }
    }

    private void AddPoint()
    {
        Score++;
        ScoreChanged?.Invoke(this, EventArgs.Empty);

        int newLevel = 1 + Score / PointsPerLevel;
        if (newLevel != Level)
        {
            Level = newLevel;
            Speed = Math.Min(Difficulty.BaseSpeed + (Level - 1) * Difficulty.SpeedIncreasePerLevel,
                Difficulty.MaxSpeed);
            LevelChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    private void EndGame()
    {
        IsNewHighScore = highScores.TrySaveHighScore(Difficulty.Level, Score);
        gameOverTick = TickCount;
        ChangeState(GameState.GameOver);
        Crashed?.Invoke(this, EventArgs.Empty);
    }

    private void ScrollGround()
    {
        GroundOffset = (GroundOffset + Speed) % 24f;
    }

    private void CreatePipes()
    {
        pipes.Clear();
        for (int i = 0; i < PipeCount; i++)
        {
            pipes.Add(new PipePair(FirstPipeX + i * PipeSpacing, RandomGapY(), Difficulty.GapHeight,
                PlayAreaHeight));
        }
    }

    private void RecyclePipe(PipePair pipe)
    {
        float rightMostX = pipes.Max(p => p.X);
        pipe.Reposition(rightMostX + PipeSpacing, RandomGapY(), Difficulty.GapHeight);
    }

    private float RandomGapY()
    {
        int maxGapY = PlayAreaHeight - Difficulty.GapHeight - MinGapMargin;
        return random.Next(MinGapMargin, maxGapY + 1);
    }

    private void ChangeState(GameState newState)
    {
        if (State == newState && newState != GameState.Ready)
            return;

        State = newState;
        StateChanged?.Invoke(this, EventArgs.Empty);
    }
}
