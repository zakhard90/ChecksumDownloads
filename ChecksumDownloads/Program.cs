namespace ChecksumDownloads;

using System.Windows.Forms;

internal static class Program
{
    [STAThread]
    static void Main()
    {
        bool createdNew;
        using Mutex mutex = new(true, "ChecksumDownloadsAppMutex", out createdNew);

        if (!createdNew)
        {
            MessageBox.Show("The application is already running.", "Checksum Downloads");
            return;
        }

        ApplicationConfiguration.Initialize();
        Application.Run(new ChecksumApplet());
    }
}