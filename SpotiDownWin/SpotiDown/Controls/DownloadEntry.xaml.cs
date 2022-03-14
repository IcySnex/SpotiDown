using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml;
using SpotiDown.Models;
using System;
using System.Threading;

namespace SpotiDown.Controls;

public sealed partial class DownloadEntry : UserControl
{
    public DownloadEntry(Song Song)
    {
        InitializeComponent();
        this.Song = Song;

        CancelDownload.Click += (s, e) => Cancel();

        DownloadNow.Click += (s, e) => Download();
        TrackInfo.Click += async (s, e) => 
        {
            var cd = new ContentDialog
            {
                Title = "Track Info",
                Content = new TrackInfo((Song)this.Song.Clone()),
                CloseButtonText = "Cancel",
                PrimaryButtonText = "Save",
                XamlRoot = Content.XamlRoot
            };
            cd.PrimaryButtonClick += (s, e) =>
            {
                Song Song = ((TrackInfo)cd.Content).Song;
                if (string.IsNullOrWhiteSpace(Song.Title))
                    Song.Title = this.Song.Title;
                this.Song = Song;
                Title.Text = this.Song.Title;
                Artist.Text = Helpers.Text.NaIfEmpty(this.Song.Artist);
                Album.Text = Helpers.Text.NaIfEmpty(this.Song.Album);
                Artwork.ImageSource = Helpers.Local.DownloadImage(this.Song.Artwork);
            };
            await cd.ShowAsync();
        };
        Delete.Click += async (s, e) =>
        {
            if (await Helpers.Window.Alert(Content.XamlRoot, "Delete Track", "Do you really want to delete this track from the download queue?", "No", "Yes") == ContentDialogResult.Primary)
            {
                var Container = (ItemsControl)Parent;
                Container.Items.Remove(this);
                ((TextBlock)((Grid)((Grid)((ScrollViewer)Container.Parent).Parent).Parent).Children[1]).Visibility = Container.Items.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
            }
        };

        UpdateFlyout(false);
    }

    public void Download()
    {
        UpdateFlyout(true);
    }
    public void Cancel()
    {
        cts.Cancel();
        UpdateFlyout(false);
    }

    public Song Song;
    CancellationTokenSource cts = new();

    private MenuFlyout Flyout = new();
    private MenuFlyoutItem CancelDownload = new() { Icon = new SymbolIcon(Symbol.Cancel), Text = "Cancel Download" };
    private MenuFlyoutItem DownloadNow = new() { Icon = new SymbolIcon(Symbol.Download), Text = "Download Now" };
    private MenuFlyoutItem TrackInfo = new() { Icon = new SymbolIcon(Symbol.MusicInfo), Text = "Track Info" };
    private MenuFlyoutItem Delete = new() { Icon = new SymbolIcon(Symbol.Delete), Text = "Delete" };
    private void UpdateFlyout(bool IsDownloading)
    {
        Flyout.Items.Clear();
        if (IsDownloading)
        {
            Progress.IsIndeterminate = false;
            Flyout.Items.Add(CancelDownload);
        }
        else
        {
            Progress.IsIndeterminate = true;
            Flyout.Items.Add(DownloadNow);
            Flyout.Items.Add(TrackInfo);
            Flyout.Items.Add(Delete);
        }
    }
}