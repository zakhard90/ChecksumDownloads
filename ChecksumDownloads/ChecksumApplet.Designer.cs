namespace ChecksumDownloads;

using System.Security.Cryptography;
using Microsoft.Win32;

partial class ChecksumApplet
{
    private FileSystemWatcher watcher;
    private NotifyIcon notifyIcon;
    private System.ComponentModel.IContainer components = null;
    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();
        System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ChecksumApplet));
        watcher = new FileSystemWatcher();
        notifyIcon = new NotifyIcon(components);
        ((System.ComponentModel.ISupportInitialize)watcher).BeginInit();
        SuspendLayout();
        // 
        // watcher
        // 
        watcher.EnableRaisingEvents = true;
        watcher.NotifyFilter = NotifyFilters.FileName | NotifyFilters.Size;
        watcher.SynchronizingObject = this;
        watcher.Created += OnFileCreatedAsync;
        watcher.Renamed += OnFileRenamedAsync;
        // 
        // notifyIcon
        // 
        notifyIcon.Icon = (Icon)resources.GetObject("notifyIcon.Icon");
        notifyIcon.Text = "Checksum Download";
        notifyIcon.Visible = true;
        notifyIcon.MouseDoubleClick += NotifyIconOnMouseDoubleClick;
        // 
        // ChecksumApplet
        // 
        AutoScaleDimensions = new SizeF(8F, 19F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(800, 428);
        Name = "ChecksumApplet";
        Text = "Checksum Applet";
        Load += AppletOnLoad;
        ((System.ComponentModel.ISupportInitialize)watcher).EndInit();
        ResumeLayout(false);
    }

    #endregion
}
