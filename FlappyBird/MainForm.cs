using FlappyBird.Game;
using FlappyBird.Rendering;
using FlappyBird.Services;

namespace FlappyBird;

/// <summary>
/// Ana pencere. Klavye/fare olaylarını yakalar, Timer ile oyun döngüsünü çalıştırır
/// ve oyun durumunu menü ile durum çubuğuna yansıtır. Oyun kuralları
/// <see cref="GameEngine"/> sınıfındadır.
/// </summary>
public partial class MainForm : Form
{
    private readonly HighScoreRepository highScores = new();
    private readonly GameEngine engine;
    private readonly GameRenderer renderer = new();
    private readonly SoundEffects sounds = new();

    /// <summary>
    /// Zıplama tuşu basılı tutulduğunda tekrar tekrar zıplamayı önlemek için kullanılır.
    /// KeyDown'da true, KeyUp'ta false yapılır.
    /// </summary>
    private bool flapKeyHeld;

    /// <summary>Dosya hatası uyarısı yalnızca bir kez gösterilsin.</summary>
    private bool storageWarningShown;

    public MainForm()
    {
        InitializeComponent();

        engine = new GameEngine(highScores, DifficultyLevel.Normal);
        engine.StateChanged += Engine_StateChanged;
        engine.ScoreChanged += Engine_ScoreChanged;
        engine.LevelChanged += Engine_LevelChanged;
        engine.Flapped += (_, _) => sounds.PlayFlap();
        engine.Crashed += Engine_Crashed;
    }

    private void MainForm_Load(object? sender, EventArgs e)
    {
        // Oyun alanı her DPI ayarında aynı oranda (400x600) görünsün.
        float scale = DeviceDpi / 96f;
        ClientSize = new Size(
            (int)(GameEngine.WorldWidth * scale),
            menuStrip.Height + (int)(GameEngine.WorldHeight * scale) + statusStrip.Height);

        UpdateDifficultyMenu();
        UpdateStatusBar();

        if (highScores.LastError is not null)
            ShowStorageWarning(highScores.LastError);

        gameTimer.Start();
    }

    // ------------------------------------------------------------------ Oyun döngüsü

    private void GameTimer_Tick(object? sender, EventArgs e)
    {
        engine.Update();
        gamePanel.Invalidate();
    }

    private void GamePanel_Paint(object? sender, PaintEventArgs e)
    {
        // Oyun dünyası 400x600 birim; panel boyutuna göre ölçekle.
        float sx = gamePanel.ClientSize.Width / (float)GameEngine.WorldWidth;
        float sy = gamePanel.ClientSize.Height / (float)GameEngine.WorldHeight;
        e.Graphics.ScaleTransform(sx, sy);

        renderer.Draw(e.Graphics, engine);
    }

    // ------------------------------------------------------------------ Klavye ve fare

    private void MainForm_KeyDown(object? sender, KeyEventArgs e)
    {
        switch (e.KeyCode)
        {
            case Keys.Space:
            case Keys.Up:
            case Keys.W:
                if (!flapKeyHeld)
                {
                    flapKeyHeld = true;
                    engine.Flap();
                }
                break;

            case Keys.Enter:
            case Keys.R:
                if (engine.State == GameState.GameOver)
                {
                    if (engine.CanRestart) engine.NewGame();
                }
                else if (e.KeyCode == Keys.R)
                {
                    engine.NewGame();
                }
                break;

            case Keys.P:
            case Keys.Escape:
                engine.TogglePause();
                break;

            case Keys.M:
                ToggleSound();
                break;

            default:
                return; // Diğer tuşlar (F2, Alt+F4 ...) menüye bırakılır.
        }

        e.Handled = true;
        e.SuppressKeyPress = true; // "ding" sesini ve menüye geçişi engelle
    }

    private void MainForm_KeyUp(object? sender, KeyEventArgs e)
    {
        if (e.KeyCode is Keys.Space or Keys.Up or Keys.W)
            flapKeyHeld = false;
    }

    private void GamePanel_MouseDown(object? sender, MouseEventArgs e)
    {
        if (e.Button == MouseButtons.Left)
            engine.Flap();
    }

    /// <summary>Pencere odağı kaybedince oyun otomatik duraklatılır.</summary>
    private void MainForm_Deactivate(object? sender, EventArgs e)
    {
        flapKeyHeld = false;
        if (engine.State == GameState.Playing)
            engine.TogglePause();
    }

    // ------------------------------------------------------------------ Oyun olayları

    private void Engine_StateChanged(object? sender, EventArgs e) => UpdateStatusBar();

    private void Engine_ScoreChanged(object? sender, EventArgs e)
    {
        if (engine.Score > 0)
            sounds.PlayPoint();
        UpdateStatusBar();
    }

    private void Engine_LevelChanged(object? sender, EventArgs e)
    {
        if (engine.Level > 1)
            sounds.PlayLevelUp();
        UpdateStatusBar();
    }

    private void Engine_Crashed(object? sender, EventArgs e)
    {
        sounds.PlayHit();
        UpdateStatusBar();

        if (engine.StorageError is not null)
            ShowStorageWarning(engine.StorageError);
    }

    // ------------------------------------------------------------------ Menü

    private void MenuNewGame_Click(object? sender, EventArgs e) => engine.NewGame();

    private void MenuPause_Click(object? sender, EventArgs e) => engine.TogglePause();

    private void MenuExit_Click(object? sender, EventArgs e) => Close();

    private void MenuScoreBoard_Click(object? sender, EventArgs e)
    {
        WithPause(() =>
        {
            using var dialog = new ScoreBoardForm(highScores, engine.Difficulty.Level);
            dialog.ShowDialog(this);

            if (dialog.WasCleared)
                UpdateStatusBar();
        });
    }

    private void MenuDifficulty_Click(object? sender, EventArgs e)
    {
        DifficultyLevel level = sender == menuEasy ? DifficultyLevel.Easy
            : sender == menuHard ? DifficultyLevel.Hard
            : DifficultyLevel.Normal;

        engine.SetDifficulty(level);
        UpdateDifficultyMenu();
        UpdateStatusBar();
    }

    private void MenuSound_Click(object? sender, EventArgs e) => ToggleSound();

    private void MenuControls_Click(object? sender, EventArgs e)
    {
        WithPause(() => MessageBox.Show(this,
            "SPACE / ↑ / W / Sol tık : Zıpla (oyunu başlatır)\n" +
            "P veya ESC : Duraklat / Devam\n" +
            "R veya F2 : Yeni oyun\n" +
            "F3 : Skor tablosu (ilk 10)\n" +
            "Enter : Game Over ekranında yeniden başlat\n" +
            "M : Sesi aç / kapat\n\n" +
            "Her engelden geçişte 1 puan kazanılır. Her 5 puanda seviye ve hız artar.",
            "Kontroller", MessageBoxButtons.OK, MessageBoxIcon.Information));
    }

    private void MenuAbout_Click(object? sender, EventArgs e)
    {
        WithPause(() => MessageBox.Show(this,
            "Kanatçık — Flappy Bird benzeri masaüstü oyunu\n\n" +
            "YMH 219 Nesne Tabanlı Programlama\nEk Uygulama Ödevi 3\n\n" +
            "C# · .NET · Windows Forms",
            "Hakkında", MessageBoxButtons.OK, MessageBoxIcon.Information));
    }

    // ------------------------------------------------------------------ Yardımcılar

    private void ToggleSound()
    {
        sounds.Enabled = !sounds.Enabled;
        menuSound.Checked = sounds.Enabled;
        UpdateStatusBar();
    }

    /// <summary>Bir diyalog gösterilirken oyunu duraklatır.</summary>
    private void WithPause(Action action)
    {
        if (engine.State == GameState.Playing)
            engine.TogglePause();
        action();
    }

    private void UpdateDifficultyMenu()
    {
        menuEasy.Checked = engine.Difficulty.Level == DifficultyLevel.Easy;
        menuNormal.Checked = engine.Difficulty.Level == DifficultyLevel.Normal;
        menuHard.Checked = engine.Difficulty.Level == DifficultyLevel.Hard;
    }

    private void UpdateStatusBar()
    {
        lblState.Text = engine.State switch
        {
            GameState.Ready => "● Hazır",
            GameState.Playing => "● Oynanıyor",
            GameState.Paused => "● Duraklatıldı",
            GameState.GameOver => "● Game Over",
            _ => ""
        };
        lblState.ForeColor = engine.State switch
        {
            GameState.Playing => Color.ForestGreen,
            GameState.Paused => Color.DarkOrange,
            GameState.GameOver => Color.Firebrick,
            _ => Color.SteelBlue
        };

        lblScore.Text = $"Skor: {engine.Score}";
        lblHighScore.Text = $"Rekor: {engine.HighScore}";
        lblLevel.Text = $"Seviye: {engine.Level}";
        lblSpeed.Text = $"Hız: {engine.Speed:0.0}" + (sounds.Enabled ? "" : "  (Ses kapalı)");
    }

    /// <summary>Skor dosyası hatasını kullanıcıya bir kez bildirir; oyun çalışmaya devam eder.</summary>
    private void ShowStorageWarning(string message)
    {
        if (storageWarningShown) return;
        storageWarningShown = true;

        WithPause(() => MessageBox.Show(this,
            message + "\n\nOyun çalışmaya devam edecek ancak en yüksek skor kaydedilemeyebilir.",
            "Dosya Uyarısı", MessageBoxButtons.OK, MessageBoxIcon.Warning));
    }
}
