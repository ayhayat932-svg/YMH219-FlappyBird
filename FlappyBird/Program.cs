namespace FlappyBird;

internal static class Program
{
    /// <summary>
    /// Uygulamanın giriş noktası.
    /// </summary>
    [STAThread]
    private static void Main()
    {
        ApplicationConfiguration.Initialize();

        // Beklenmeyen bir hata oluşursa uygulama sessizce kapanmasın, kullanıcı bilgilendirilsin.
        Application.ThreadException += (_, e) =>
            MessageBox.Show("Beklenmeyen bir hata oluştu:\n\n" + e.Exception.Message,
                "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);

        Application.Run(new MainForm());
    }
}
