using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;
using SpotiDown.Models;
using System;
using System.Collections.Generic;

namespace SpotiDown.Controls;

public sealed partial class DownloadEntry : UserControl
{
    public DownloadEntry()
    {
        InitializeComponent();
        this.Song = new(Enums.SongType.YouTube, "", "Song sex", "187", new(0,2,54), "Nothingles", DateTime.Now, "lalalalala", "https://yt3.ggpht.com/G-xQ-34A-GcfZm9ByvMnFEf1TjOatTKrJ3g0XaL1kXqCbin-7hTXhQBDe3VYtcAhx59iKG9C5jA=s900-c-k-c0x00ffffff-no-rj", 5, 1);

        CancelDownload.Click += (s, e) => UpdateFlyout(false);
        DownloadNow.Click += (s, e) => UpdateFlyout(true);

        UpdateFlyout(false);
    }

    public Song Song;

    private MenuFlyout Flyout = new();
    private MenuFlyoutItem CancelDownload = new() { Icon = new SymbolIcon(Symbol.Cancel), Text = "Cancel Download" };
    private MenuFlyoutItem DownloadNow = new() { Icon = new SymbolIcon(Symbol.Download), Text = "Download Now" };
    private MenuFlyoutItem EditInfo = new() { Icon = new SymbolIcon(Symbol.MusicInfo), Text = "Edit Info" };
    private MenuFlyoutItem Delete = new() { Icon = new SymbolIcon(Symbol.Delete), Text = "Delete" };
    
    private void UpdateFlyout(bool IsDownloading)
    {
        Flyout.Items.Clear();
        if (IsDownloading)
            Flyout.Items.Add(CancelDownload);
        else
        {
            Flyout.Items.Add(DownloadNow);
            Flyout.Items.Add(EditInfo);
            Flyout.Items.Add(Delete);
        }
    }
}