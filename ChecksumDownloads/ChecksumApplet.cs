using Microsoft.Win32;
using System.Security.Cryptography;
using System.Text;

namespace ChecksumDownloads;

public partial class ChecksumApplet : Form
{
    private const int maxEntries = 3;
    private const int delay = 3000;
    private const int duration = 5000;
    private readonly string[] allowedExtensions = [".exe", ".msi", ".zip", ".rar", ".7z"];
    private readonly string[] ignoedExtensions = [".tmp", ".crdownload", ".part"];
    private readonly Queue<(DateTime Time, string FileName, string Checksum)> latestChecksums = new();

    public ChecksumApplet()
    {
        this.WindowState = FormWindowState.Minimized;
        this.ShowInTaskbar = false;
        this.Visible = false;

        InitializeComponent();
    }

    private void AppletOnLoad(object sender, EventArgs e)
    {
        watcher.Path = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\Downloads";

        var exePath = Application.ExecutablePath;
        var key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", true);
        if (key == null)
            return;

        key.SetValue("ChecksumDownloads", exePath);
        this.Hide();
    }

    private async void OnFileCreatedAsync(object sender, FileSystemEventArgs e)
    {
        await HandleChangeAsync(sender, e);
    }

    private async void OnFileRenamedAsync(object sender, FileSystemEventArgs e)
    {
        await HandleChangeAsync(sender, e);
    }

    private async Task HandleChangeAsync(object sender, FileSystemEventArgs e)
    {
        try
        {
            var ext = Path.GetExtension(e.FullPath).ToLowerInvariant();

            if (ignoedExtensions.Contains(ext))
                return;

            if (!allowedExtensions.Contains(ext))
                return;

            await Task.Delay(delay);
            var hash = ComputeSHA256(e.FullPath);
            var fileName = Path.GetFileName(e.FullPath);

            if (AlreadyNotified(fileName, hash))
                return;

            AddToLatest(fileName, hash);
            ShowNotification(fileName, hash);
        }
        catch (Exception ex)
        {
            MessageBox.Show("Error: " + ex.Message);
        }
    }

    private static string ComputeSHA256(string filePath)
    {
        using var stream = File.OpenRead(filePath);
        using var sha = SHA256.Create();
        var hashBytes = sha.ComputeHash(stream);
        return BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
    }

    private void ShowNotification(string fileName, string hash)
    {
        var hintLength = 8;
        notifyIcon.BalloonTipTitle = $"Downloaded: {fileName}";
        notifyIcon.BalloonTipText = $"SHA256: {hash[..hintLength]}...{hash[^hintLength..]}";
        notifyIcon.ShowBalloonTip(duration);
    }

    private void AddToLatest(string fileName, string hash)
    {
        var now = DateTime.Now;
        latestChecksums.Enqueue((now, fileName, hash));
        if (latestChecksums.Count <= maxEntries)
            return;

        latestChecksums.Dequeue();
    }
    private bool AlreadyNotified(string fileName, string hash)
    {
        var now = DateTime.Now;
        foreach (var (t, f, h) in latestChecksums)
        {
            if (f == fileName && h == hash && (now - t).TotalMilliseconds < delay)
                return true;
        }
        return false;
    }

    private void ShowLatestList(object sender, EventArgs e)
    {
        var sb = new StringBuilder();
        var hintLength = 12;
        foreach (var (_, fileName, hash) in latestChecksums)
        {
            sb.AppendLine($"File: {fileName[..hintLength]}...{fileName[^hintLength..]}");
            sb.AppendLine($"SHA256: {hash}");
            sb.AppendLine("");
        }

        sb.AppendLine("Supported file extensions:");
        var i = 0;
        foreach (var ext in allowedExtensions)
        {
            if (i++ > 0)
            {
                sb.Append(", ");
            }
            sb.Append(ext);
        }

        MessageBox.Show(
            latestChecksums.Count > 0 ? sb.ToString() : "No downloads yet.",
            "Checksum Downloads",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information);
    }

    private void OnExitClicked(object? sender, EventArgs e)
    {
        notifyIcon.Visible = false;
        Application.Exit();
    }
}
