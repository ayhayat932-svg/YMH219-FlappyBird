namespace FlappyBird.Game;

/// <summary>
/// Ekranda konumu ve boyutu olan tüm oyun nesneleri için ortak temel sınıf.
/// </summary>
public abstract class GameObject
{
    public float X { get; protected set; }
    public float Y { get; protected set; }
    public float Width { get; }
    public float Height { get; }

    protected GameObject(float x, float y, float width, float height)
    {
        if (width <= 0 || height <= 0)
            throw new ArgumentException("Nesne boyutları sıfırdan büyük olmalıdır.");

        X = x;
        Y = y;
        Width = width;
        Height = height;
    }

    /// <summary>Nesnenin ekrandaki dikdörtgen sınırı.</summary>
    public virtual RectangleF Bounds => new(X, Y, Width, Height);

    /// <summary>Nesneyi verilen grafik yüzeyine çizer.</summary>
    public abstract void Draw(Graphics g);
}
