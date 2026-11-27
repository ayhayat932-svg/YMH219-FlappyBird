using System.Drawing.Drawing2D;
using FlappyBird.Game;

namespace FlappyBird.Rendering;

/// <summary>
/// Oyun ekranını çizen sınıf. Oyun mantığından ayrıdır: yalnızca <see cref="GameEngine"/>
/// üzerindeki bilgileri okur ve ekrana çizer.
/// </summary>
public sealed class GameRenderer : IDisposable
{
    private readonly FontFamily fontFamily = new("Segoe UI");
    private readonly Font smallFont = new("Segoe UI", 15f, FontStyle.Bold, GraphicsUnit.Pixel);
    private readonly Font infoFont = new("Segoe UI", 18f, FontStyle.Bold, GraphicsUnit.Pixel);

    private static readonly Color TextOutline = Color.FromArgb(60, 35, 20);

    // Bulutların sabit başlangıç konumları (x, y, ölçek)
    private static readonly (float X, float Y, float Scale)[] Clouds =
    {
        (40, 70, 1.0f), (210, 120, 0.7f), (330, 50, 1.2f), (120, 190, 0.6f), (470, 150, 0.9f)
    };

    // Yıldızların sabit konumları (rastgele ama her çizimde aynı olsun diye sabit tohum)
    private static readonly PointF[] Stars = CreateStars(45);

    /// <summary>Kullanılan renk teması (gündüz / gece).</summary>
    public ThemePalette Palette { get; set; } = ThemePalette.Day;

    public void Draw(Graphics g, GameEngine engine)
    {
        g.SmoothingMode = SmoothingMode.AntiAlias;
        g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;

        DrawSky(g);
        if (Palette.ShowStars)
            DrawStarsAndMoon(g, engine.TickCount);
        DrawClouds(g, engine.TickCount, engine.State);
        DrawHills(g);

        foreach (var pipe in engine.Pipes)
            pipe.Draw(g);

        DrawGround(g, engine.GroundOffset);
        engine.Bird.Draw(g);

        switch (engine.State)
        {
            case GameState.Ready:
                DrawReadyOverlay(g, engine);
                break;
            case GameState.Playing:
                DrawHud(g, engine);
                break;
            case GameState.Paused:
                DrawHud(g, engine);
                DrawPausedOverlay(g);
                break;
            case GameState.GameOver:
                DrawGameOverOverlay(g, engine);
                break;
        }
    }

    // ------------------------------------------------------------------ Arka plan

    private void DrawSky(Graphics g)
    {
        var rect = new Rectangle(0, 0, GameEngine.WorldWidth, GameEngine.PlayAreaHeight);
        using var sky = new LinearGradientBrush(rect, Palette.SkyTop, Palette.SkyBottom, LinearGradientMode.Vertical);
        g.FillRectangle(sky, rect);
    }

    private static PointF[] CreateStars(int count)
    {
        var rnd = new Random(7);
        var stars = new PointF[count];
        for (int i = 0; i < count; i++)
            stars[i] = new PointF(rnd.Next(0, GameEngine.WorldWidth), rnd.Next(0, GameEngine.PlayAreaHeight - 150));
        return stars;
    }

    private static void DrawStarsAndMoon(Graphics g, long tick)
    {
        for (int i = 0; i < Stars.Length; i++)
        {
            // Her yıldız farklı hızda parıldar.
            int alpha = 120 + (int)(Math.Sin(tick * 0.05 + i) * 100);
            using var brush = new SolidBrush(Color.FromArgb(Math.Clamp(alpha, 30, 255), 255, 255, 230));
            float size = i % 3 == 0 ? 3f : 2f;
            g.FillEllipse(brush, Stars[i].X, Stars[i].Y, size, size);
        }

        // Hilal: bir daireden, kaydırılmış ikinci bir daire çıkarılarak elde edilir.
        using var moonPath = new GraphicsPath();
        moonPath.AddEllipse(300, 40, 54, 54);
        using var cutPath = new GraphicsPath();
        cutPath.AddEllipse(316, 30, 54, 54);
        using var crescent = new Region(moonPath);
        crescent.Exclude(cutPath);

        using var moon = new SolidBrush(Color.FromArgb(250, 245, 210));
        g.FillRegion(moon, crescent);
    }

    private void DrawClouds(Graphics g, long tick, GameState state)
    {
        // Bulutlar oyundan bağımsız olarak çok yavaş kayar (paralaks etkisi).
        float drift = state == GameState.GameOver ? 0 : tick * 0.25f;
        const float loopWidth = GameEngine.WorldWidth + 200;

        using var brush = new SolidBrush(Palette.Cloud);
        foreach (var (x, y, scale) in Clouds)
        {
            float cx = ((x - drift) % loopWidth + loopWidth) % loopWidth - 100;
            float w = 90 * scale, h = 34 * scale;
            g.FillEllipse(brush, cx, y, w, h);
            g.FillEllipse(brush, cx + w * 0.2f, y - h * 0.5f, w * 0.5f, h * 1.1f);
            g.FillEllipse(brush, cx + w * 0.45f, y - h * 0.3f, w * 0.45f, h);
        }
    }

    private void DrawHills(Graphics g)
    {
        float baseY = GameEngine.PlayAreaHeight;
        using var far = new SolidBrush(Palette.FarHills);
        using var near = new SolidBrush(Palette.NearHills);

        g.FillEllipse(far, -60, baseY - 110, 260, 220);
        g.FillEllipse(far, 180, baseY - 140, 300, 280);
        g.FillEllipse(near, 60, baseY - 70, 220, 140);
        g.FillEllipse(near, 290, baseY - 60, 200, 120);
    }

    private static void DrawGround(Graphics g, float offset)
    {
        float top = GameEngine.PlayAreaHeight;
        var ground = new RectangleF(0, top, GameEngine.WorldWidth, GameEngine.GroundHeight);

        using var dirt = new SolidBrush(Color.FromArgb(222, 200, 140));
        using var grass = new SolidBrush(Color.FromArgb(120, 190, 70));
        using var stripe = new SolidBrush(Color.FromArgb(100, 165, 55));
        using var edge = new Pen(Color.FromArgb(80, 60, 30), 2f);

        g.FillRectangle(dirt, ground);
        g.FillRectangle(grass, 0, top, GameEngine.WorldWidth, 18);

        // Kayan çapraz şeritler zeminin hareket ettiği hissini verir.
        var oldClip = g.Clip;
        g.SetClip(new RectangleF(0, top, GameEngine.WorldWidth, 18));
        for (float x = -offset - 24; x < GameEngine.WorldWidth + 24; x += 24)
        {
            PointF[] p = { new(x, top + 18), new(x + 12, top), new(x + 22, top), new(x + 10, top + 18) };
            g.FillPolygon(stripe, p);
        }
        g.Clip = oldClip;

        g.DrawLine(edge, 0, top, GameEngine.WorldWidth, top);
        g.DrawLine(edge, 0, top + 18, GameEngine.WorldWidth, top + 18);
    }

    // ------------------------------------------------------------------ Bilgi ekranları

    private void DrawHud(Graphics g, GameEngine engine)
    {
        DrawOutlinedText(g, engine.Score.ToString(), 56, new PointF(GameEngine.WorldWidth / 2f, 60), Color.White);

        string info = $"Seviye {engine.Level}";
        DrawOutlinedText(g, info, 18, new PointF(GameEngine.WorldWidth / 2f, 108), Color.FromArgb(255, 230, 120));
    }

    private void DrawReadyOverlay(Graphics g, GameEngine engine)
    {
        float cx = GameEngine.WorldWidth / 2f;
        DrawOutlinedText(g, "KANATÇIK", 52, new PointF(cx, 110), Color.FromArgb(255, 210, 70));
        DrawOutlinedText(g, "Hazır mısın?", 26, new PointF(cx, 165), Color.White);

        var card = new RectangleF(50, 330, 300, 140);
        FillCard(g, card);

        DrawCenteredText(g, "SPACE / ↑ / Fare tıkı: Zıpla", infoFont, card.Y + 30);
        DrawCenteredText(g, $"Zorluk: {engine.Difficulty.DisplayName}", smallFont, card.Y + 64);
        DrawCenteredText(g, $"En yüksek skor: {engine.HighScore}", smallFont, card.Y + 90);
        DrawCenteredText(g, "P: Duraklat  M: Ses  T: Tema  F3: Skorlar", smallFont, card.Y + 116);
    }

    private void DrawPausedOverlay(Graphics g)
    {
        using var shade = new SolidBrush(Color.FromArgb(110, 0, 0, 0));
        g.FillRectangle(shade, 0, 0, GameEngine.WorldWidth, GameEngine.WorldHeight);

        float cx = GameEngine.WorldWidth / 2f;
        DrawOutlinedText(g, "DURAKLATILDI", 40, new PointF(cx, 260), Color.White);
        DrawOutlinedText(g, "Devam etmek için P", 20, new PointF(cx, 310), Color.FromArgb(255, 230, 120));
    }

    private void DrawGameOverOverlay(Graphics g, GameEngine engine)
    {
        float cx = GameEngine.WorldWidth / 2f;
        DrawOutlinedText(g, "GAME OVER", 50, new PointF(cx, 130), Color.FromArgb(255, 110, 80));

        var card = new RectangleF(60, 185, 280, 190);
        FillCard(g, card);

        DrawCenteredText(g, "SKOR", smallFont, card.Y + 25);
        DrawOutlinedText(g, engine.Score.ToString(), 40, new PointF(cx, card.Y + 60), Color.White);
        DrawCenteredText(g, $"En yüksek ({engine.Difficulty.DisplayName}): {engine.HighScore}", infoFont,
            card.Y + 105);
        DrawCenteredText(g, $"Ulaşılan seviye: {engine.Level}", smallFont, card.Y + 135);

        if (engine.IsNewHighScore)
            DrawOutlinedText(g, "YENİ REKOR!", 22, new PointF(cx, card.Y + 166), Color.FromArgb(255, 220, 60));
        else if (engine.LastRank > 0)
            DrawOutlinedText(g, $"Skor tablosunda {engine.LastRank}. sıra!", 18, new PointF(cx, card.Y + 166),
                Color.FromArgb(150, 220, 255));

        if (engine.CanRestart)
        {
            // Yanıp sönen yönlendirme yazısı
            if (engine.TickCount / 30 % 2 == 0)
                DrawOutlinedText(g, "R / Enter / Space: Yeniden Başlat", 18,
                    new PointF(cx, card.Bottom + 35), Color.White);
        }
    }

    // ------------------------------------------------------------------ Yardımcılar

    private static void FillCard(Graphics g, RectangleF rect)
    {
        using var path = RoundedRect(rect, 14);
        using var fill = new SolidBrush(Color.FromArgb(235, 255, 244, 214));
        using var border = new Pen(TextOutline, 3f);
        g.FillPath(fill, path);
        g.DrawPath(border, path);
    }

    private static void DrawCenteredText(Graphics g, string text, Font font, float centerY)
    {
        using var brush = new SolidBrush(TextOutline);
        using var format = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
        g.DrawString(text, font, brush, new PointF(GameEngine.WorldWidth / 2f, centerY), format);
    }

    /// <summary>Kenarları çizgili (okunaklı) büyük yazı çizer.</summary>
    private void DrawOutlinedText(Graphics g, string text, float size, PointF center, Color fill)
    {
        using var path = new GraphicsPath();
        using var format = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
        path.AddString(text, fontFamily, (int)FontStyle.Bold, size, center, format);

        using var pen = new Pen(TextOutline, Math.Max(3f, size / 9f)) { LineJoin = LineJoin.Round };
        using var brush = new SolidBrush(fill);
        g.DrawPath(pen, path);
        g.FillPath(brush, path);
    }

    private static GraphicsPath RoundedRect(RectangleF r, float radius)
    {
        float d = radius * 2;
        var path = new GraphicsPath();
        path.AddArc(r.X, r.Y, d, d, 180, 90);
        path.AddArc(r.Right - d, r.Y, d, d, 270, 90);
        path.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90);
        path.AddArc(r.X, r.Bottom - d, d, d, 90, 90);
        path.CloseFigure();
        return path;
    }

    public void Dispose()
    {
        fontFamily.Dispose();
        smallFont.Dispose();
        infoFont.Dispose();
    }
}
