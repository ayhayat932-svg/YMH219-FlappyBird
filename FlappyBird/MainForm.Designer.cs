namespace FlappyBird
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                components?.Dispose();
                renderer.Dispose();
                sounds.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            menuStrip = new MenuStrip();
            menuGame = new ToolStripMenuItem();
            menuNewGame = new ToolStripMenuItem();
            menuPause = new ToolStripMenuItem();
            menuScoreBoard = new ToolStripMenuItem();
            menuSeparator1 = new ToolStripSeparator();
            menuExit = new ToolStripMenuItem();
            menuDifficulty = new ToolStripMenuItem();
            menuEasy = new ToolStripMenuItem();
            menuNormal = new ToolStripMenuItem();
            menuHard = new ToolStripMenuItem();
            menuSettings = new ToolStripMenuItem();
            menuSound = new ToolStripMenuItem();
            menuNightTheme = new ToolStripMenuItem();
            menuHelp = new ToolStripMenuItem();
            menuControls = new ToolStripMenuItem();
            menuAbout = new ToolStripMenuItem();
            statusStrip = new StatusStrip();
            lblState = new ToolStripStatusLabel();
            lblScore = new ToolStripStatusLabel();
            lblHighScore = new ToolStripStatusLabel();
            lblLevel = new ToolStripStatusLabel();
            lblSpeed = new ToolStripStatusLabel();
            gamePanel = new FlappyBird.Controls.GamePanel();
            gameTimer = new System.Windows.Forms.Timer(components);
            menuStrip.SuspendLayout();
            statusStrip.SuspendLayout();
            SuspendLayout();
            //
            // menuStrip
            //
            menuStrip.Items.AddRange(new ToolStripItem[] { menuGame, menuDifficulty, menuSettings, menuHelp });
            menuStrip.Location = new Point(0, 0);
            menuStrip.Name = "menuStrip";
            menuStrip.Size = new Size(400, 24);
            menuStrip.TabIndex = 0;
            //
            // menuGame
            //
            menuGame.DropDownItems.AddRange(new ToolStripItem[] { menuNewGame, menuPause, menuScoreBoard, menuSeparator1, menuExit });
            menuGame.Name = "menuGame";
            menuGame.Text = "&Oyun";
            //
            // menuNewGame
            //
            menuNewGame.Name = "menuNewGame";
            menuNewGame.ShortcutKeys = Keys.F2;
            menuNewGame.Text = "&Yeni Oyun";
            menuNewGame.Click += MenuNewGame_Click;
            //
            // menuPause
            //
            menuPause.Name = "menuPause";
            menuPause.ShortcutKeyDisplayString = "P";
            menuPause.Text = "&Duraklat / Devam";
            menuPause.Click += MenuPause_Click;
            //
            // menuScoreBoard
            //
            menuScoreBoard.Name = "menuScoreBoard";
            menuScoreBoard.ShortcutKeys = Keys.F3;
            menuScoreBoard.Text = "&Skor Tablosu";
            menuScoreBoard.Click += MenuScoreBoard_Click;
            //
            // menuSeparator1
            //
            menuSeparator1.Name = "menuSeparator1";
            //
            // menuExit
            //
            menuExit.Name = "menuExit";
            menuExit.ShortcutKeys = Keys.Alt | Keys.F4;
            menuExit.Text = "Çı&kış";
            menuExit.Click += MenuExit_Click;
            //
            // menuDifficulty
            //
            menuDifficulty.DropDownItems.AddRange(new ToolStripItem[] { menuEasy, menuNormal, menuHard });
            menuDifficulty.Name = "menuDifficulty";
            menuDifficulty.Text = "&Zorluk";
            //
            // menuEasy
            //
            menuEasy.Name = "menuEasy";
            menuEasy.Text = "&Kolay";
            menuEasy.Click += MenuDifficulty_Click;
            //
            // menuNormal
            //
            menuNormal.Name = "menuNormal";
            menuNormal.Text = "&Normal";
            menuNormal.Click += MenuDifficulty_Click;
            //
            // menuHard
            //
            menuHard.Name = "menuHard";
            menuHard.Text = "&Zor";
            menuHard.Click += MenuDifficulty_Click;
            //
            // menuSettings
            //
            menuSettings.DropDownItems.AddRange(new ToolStripItem[] { menuSound, menuNightTheme });
            menuSettings.Name = "menuSettings";
            menuSettings.Text = "&Ayarlar";
            //
            // menuSound
            //
            menuSound.Checked = true;
            menuSound.CheckState = CheckState.Checked;
            menuSound.Name = "menuSound";
            menuSound.ShortcutKeyDisplayString = "M";
            menuSound.Text = "&Ses Efektleri";
            menuSound.Click += MenuSound_Click;
            //
            // menuNightTheme
            //
            menuNightTheme.Name = "menuNightTheme";
            menuNightTheme.ShortcutKeyDisplayString = "T";
            menuNightTheme.Text = "&Gece Teması";
            menuNightTheme.Click += MenuNightTheme_Click;
            //
            // menuHelp
            //
            menuHelp.DropDownItems.AddRange(new ToolStripItem[] { menuControls, menuAbout });
            menuHelp.Name = "menuHelp";
            menuHelp.Text = "&Yardım";
            //
            // menuControls
            //
            menuControls.Name = "menuControls";
            menuControls.ShortcutKeys = Keys.F1;
            menuControls.Text = "&Kontroller";
            menuControls.Click += MenuControls_Click;
            //
            // menuAbout
            //
            menuAbout.Name = "menuAbout";
            menuAbout.Text = "&Hakkında";
            menuAbout.Click += MenuAbout_Click;
            //
            // statusStrip
            //
            statusStrip.Items.AddRange(new ToolStripItem[] { lblState, lblScore, lblHighScore, lblLevel, lblSpeed });
            statusStrip.Location = new Point(0, 624);
            statusStrip.Name = "statusStrip";
            statusStrip.SizingGrip = false;
            statusStrip.Size = new Size(400, 22);
            statusStrip.TabIndex = 1;
            //
            // lblState
            //
            lblState.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblState.Name = "lblState";
            lblState.Spring = true;
            lblState.Text = "Hazır";
            lblState.TextAlign = ContentAlignment.MiddleLeft;
            //
            // lblScore
            //
            lblScore.Name = "lblScore";
            lblScore.Text = "Skor: 0";
            //
            // lblHighScore
            //
            lblHighScore.Name = "lblHighScore";
            lblHighScore.Text = "Rekor: 0";
            //
            // lblLevel
            //
            lblLevel.Name = "lblLevel";
            lblLevel.Text = "Seviye: 1";
            //
            // lblSpeed
            //
            lblSpeed.Name = "lblSpeed";
            lblSpeed.Text = "Hız: 0";
            //
            // gamePanel
            //
            gamePanel.Dock = DockStyle.Fill;
            gamePanel.Location = new Point(0, 24);
            gamePanel.Name = "gamePanel";
            gamePanel.Size = new Size(400, 600);
            gamePanel.TabIndex = 2;
            gamePanel.Paint += GamePanel_Paint;
            gamePanel.MouseDown += GamePanel_MouseDown;
            //
            // gameTimer
            //
            gameTimer.Interval = 15;
            gameTimer.Tick += GameTimer_Tick;
            //
            // MainForm
            //
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(400, 646);
            Controls.Add(gamePanel);
            Controls.Add(statusStrip);
            Controls.Add(menuStrip);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            KeyPreview = true;
            MainMenuStrip = menuStrip;
            MaximizeBox = false;
            Name = "MainForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Kanatçık - YMH219 Flappy Bird Benzeri Oyun";
            Load += MainForm_Load;
            KeyDown += MainForm_KeyDown;
            KeyUp += MainForm_KeyUp;
            Deactivate += MainForm_Deactivate;
            menuStrip.ResumeLayout(false);
            menuStrip.PerformLayout();
            statusStrip.ResumeLayout(false);
            statusStrip.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private MenuStrip menuStrip;
        private ToolStripMenuItem menuGame;
        private ToolStripMenuItem menuNewGame;
        private ToolStripMenuItem menuPause;
        private ToolStripMenuItem menuScoreBoard;
        private ToolStripSeparator menuSeparator1;
        private ToolStripMenuItem menuExit;
        private ToolStripMenuItem menuDifficulty;
        private ToolStripMenuItem menuEasy;
        private ToolStripMenuItem menuNormal;
        private ToolStripMenuItem menuHard;
        private ToolStripMenuItem menuSettings;
        private ToolStripMenuItem menuSound;
        private ToolStripMenuItem menuNightTheme;
        private ToolStripMenuItem menuHelp;
        private ToolStripMenuItem menuControls;
        private ToolStripMenuItem menuAbout;
        private StatusStrip statusStrip;
        private ToolStripStatusLabel lblState;
        private ToolStripStatusLabel lblScore;
        private ToolStripStatusLabel lblHighScore;
        private ToolStripStatusLabel lblLevel;
        private ToolStripStatusLabel lblSpeed;
        private FlappyBird.Controls.GamePanel gamePanel;
        private System.Windows.Forms.Timer gameTimer;
    }
}
