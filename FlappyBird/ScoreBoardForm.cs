using FlappyBird.Game;
using FlappyBird.Services;

namespace FlappyBird;

/// <summary>
/// Her zorluk seviyesi için ilk 10 skoru ayrı bir sekmede listeleyen pencere.
/// </summary>
public partial class ScoreBoardForm : Form
{
    private readonly HighScoreRepository repository;
    private readonly Dictionary<DifficultyLevel, ListView> lists = new();

    /// <summary>Tablo temizlendiyse true olur (ana form durum çubuğunu yenilesin diye).</summary>
    public bool WasCleared { get; private set; }

    public ScoreBoardForm(HighScoreRepository repository, DifficultyLevel selectedLevel)
    {
        this.repository = repository ?? throw new ArgumentNullException(nameof(repository));
        InitializeComponent();
        CreateTabs();

        tabControl.SelectedIndex = (int)selectedLevel;
    }

    /// <summary>Her zorluk seviyesi için bir sekme ve içinde bir ListView oluşturur.</summary>
    private void CreateTabs()
    {
        foreach (DifficultyLevel level in Enum.GetValues<DifficultyLevel>())
        {
            var list = new ListView
            {
                Dock = DockStyle.Fill,
                View = View.Details,
                FullRowSelect = true,
                GridLines = true,
                HeaderStyle = ColumnHeaderStyle.Nonclickable,
                MultiSelect = false
            };
            list.Columns.Add("Sıra", 50, HorizontalAlignment.Center);
            list.Columns.Add("Skor", 80, HorizontalAlignment.Center);
            list.Columns.Add("Seviye", 80, HorizontalAlignment.Center);
            list.Columns.Add("Tarih", 190, HorizontalAlignment.Left);

            var page = new TabPage(DifficultySettings.For(level).DisplayName) { Tag = level };
            page.Controls.Add(list);
            tabControl.TabPages.Add(page);

            lists[level] = list;
            FillList(level);
        }
    }

    private void FillList(DifficultyLevel level)
    {
        var list = lists[level];
        list.BeginUpdate();
        list.Items.Clear();

        var scores = repository.GetTopScores(level);
        if (scores.Count == 0)
        {
            list.Items.Add(new ListViewItem(new[] { "-", "-", "-", "Henüz kayıt yok" }));
        }
        else
        {
            for (int i = 0; i < scores.Count; i++)
            {
                var e = scores[i];
                var item = new ListViewItem(new[]
                {
                    (i + 1).ToString(),
                    e.Score.ToString(),
                    e.LevelReached.ToString(),
                    e.Date.ToString("dd.MM.yyyy HH:mm")
                });

                if (i == 0) item.Font = new Font(list.Font, FontStyle.Bold);
                list.Items.Add(item);
            }
        }

        list.EndUpdate();
    }

    private void BtnClear_Click(object? sender, EventArgs e)
    {
        if (tabControl.SelectedTab?.Tag is not DifficultyLevel level)
            return;

        string name = DifficultySettings.For(level).DisplayName;
        var answer = MessageBox.Show(this,
            $"\"{name}\" seviyesinin skor tablosu silinecek. Emin misiniz?",
            "Tabloyu Temizle", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);

        if (answer != DialogResult.Yes)
            return;

        repository.Clear(level);
        WasCleared = true;
        FillList(level);

        if (repository.LastError is not null)
            MessageBox.Show(this, repository.LastError, "Dosya Hatası", MessageBoxButtons.OK, MessageBoxIcon.Warning);
    }
}
