using System.Media;

namespace FlappyBird.Services;

/// <summary>
/// Oyunun ses efektleri. Harici ses dosyasına ihtiyaç duymamak için kısa WAV sesleri
/// bellekte üretilir. Ses aygıtı yoksa hata yutulur ve oyun sessiz devam eder.
/// </summary>
public sealed class SoundEffects : IDisposable
{
    private const int SampleRate = 22050;

    private readonly SoundPlayer flapSound;
    private readonly SoundPlayer pointSound;
    private readonly SoundPlayer levelUpSound;
    private readonly SoundPlayer hitSound;

    public bool Enabled { get; set; } = true;

    public SoundEffects()
    {
        flapSound = CreatePlayer(Tone(520, 760, 0.07));
        pointSound = CreatePlayer(Concat(Tone(880, 880, 0.06), Tone(1320, 1320, 0.09)));
        levelUpSound = CreatePlayer(Concat(Tone(660, 660, 0.07), Tone(880, 880, 0.07), Tone(1175, 1175, 0.12)));
        hitSound = CreatePlayer(Tone(260, 70, 0.30, noise: true));
    }

    public void PlayFlap() => Play(flapSound);
    public void PlayPoint() => Play(pointSound);
    public void PlayLevelUp() => Play(levelUpSound);
    public void PlayHit() => Play(hitSound);

    private void Play(SoundPlayer player)
    {
        if (!Enabled) return;

        try
        {
            player.Play();
        }
        catch (Exception ex) when (ex is InvalidOperationException or TimeoutException or IOException)
        {
            // Ses aygıtı bulunamadı vb. — oyunun çalışmasını engellememeli.
            Enabled = false;
        }
    }

    private static SoundPlayer CreatePlayer(short[] samples)
    {
        var player = new SoundPlayer(ToWav(samples));
        player.Load();
        return player;
    }

    /// <summary>Başlangıç frekansından bitiş frekansına kayan kısa bir ses üretir.</summary>
    private static short[] Tone(double startHz, double endHz, double seconds, bool noise = false)
    {
        int count = (int)(SampleRate * seconds);
        var samples = new short[count];
        var rnd = new Random(42);
        double phase = 0;

        for (int i = 0; i < count; i++)
        {
            double t = (double)i / count;
            double freq = startHz + (endHz - startHz) * t;
            phase += 2 * Math.PI * freq / SampleRate;

            double value = Math.Sin(phase) > 0 ? 1 : -1;      // kare dalga (retro ses)
            if (noise) value = value * 0.6 + (rnd.NextDouble() * 2 - 1) * 0.4;

            double envelope = Math.Min(1, i / 200.0) * (1 - t);  // yumuşak giriş ve sönümlenme
            samples[i] = (short)(value * envelope * 6000);
        }

        return samples;
    }

    private static short[] Concat(params short[][] parts) => parts.SelectMany(p => p).ToArray();

    /// <summary>16 bit, mono PCM WAV verisi oluşturur.</summary>
    private static MemoryStream ToWav(short[] samples)
    {
        var stream = new MemoryStream();
        using (var writer = new BinaryWriter(stream, System.Text.Encoding.ASCII, leaveOpen: true))
        {
            int dataSize = samples.Length * 2;
            writer.Write("RIFF"u8.ToArray());
            writer.Write(36 + dataSize);
            writer.Write("WAVE"u8.ToArray());
            writer.Write("fmt "u8.ToArray());
            writer.Write(16);                 // fmt bloğu boyutu
            writer.Write((short)1);           // PCM
            writer.Write((short)1);           // mono
            writer.Write(SampleRate);
            writer.Write(SampleRate * 2);     // byte rate
            writer.Write((short)2);           // block align
            writer.Write((short)16);          // bit derinliği
            writer.Write("data"u8.ToArray());
            writer.Write(dataSize);
            foreach (short s in samples) writer.Write(s);
        }

        stream.Position = 0;
        return stream;
    }

    public void Dispose()
    {
        flapSound.Dispose();
        pointSound.Dispose();
        levelUpSound.Dispose();
        hitSound.Dispose();
    }
}
