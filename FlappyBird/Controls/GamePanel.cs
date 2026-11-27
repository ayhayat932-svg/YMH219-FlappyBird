namespace FlappyBird.Controls;

/// <summary>
/// Oyunun çizildiği panel. Çift tamponlama (double buffering) açık olduğu için
/// her karede yeniden çizim yapılırken ekran titremez.
/// </summary>
public sealed class GamePanel : Panel
{
    public GamePanel()
    {
        SetStyle(ControlStyles.AllPaintingInWmPaint
                 | ControlStyles.UserPaint
                 | ControlStyles.OptimizedDoubleBuffer
                 | ControlStyles.ResizeRedraw, true);

        // Panel odak almasın; klavye olaylarını form yakalar.
        SetStyle(ControlStyles.Selectable, false);
        TabStop = false;
    }
}
