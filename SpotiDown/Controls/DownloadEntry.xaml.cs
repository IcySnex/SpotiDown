using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml;
using SpotiDown.Models;
using System;
using System.Threading;
using System.IO;
using SpotiDown.Pages;

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
                Remove();
        };

        UpdateFlyout(false);
    }

    public async void Download()
    {
        if (!File.Exists(Helpers.Local.Config.Paths.FFMPEG))
        {
            if (await Helpers.Window.Alert(Content.XamlRoot, "Download failed!", $"It looks like FFMPEG is not installed in the config path ({Helpers.Local.Config.Paths.FFMPEG}). Do you want to download it?", "No", "Yes") != ContentDialogResult.Primary)
                return;
;
            if (!await Downloads.Instance!.DownloadFFMPEG())
                return;
        }

        string Filepath = Helpers.Song.GetFilepath(Song);
        if (File.Exists(Filepath) && await Helpers.Window.Alert(Content.XamlRoot, "Song already downloaded!", $"It looks like a song with the same filename already exists in the download path ({Helpers.Local.Config.Paths.Download}). Do you want to overwrite it or cancel the download?", "Cancel", "Overwrite") == ContentDialogResult.None)
            return;

        cts.Dispose();
        cts = new CancellationTokenSource();

        try
        {
            UpdateFlyout(true);

            var ProgressReport = new Progress<double>(value => Progress.Value = value * 100);
            if (Helpers.Local.Config.Advanced.CustomDownload)
                await Helpers.Song.WriteStream(Song, Filepath, ProgressReport, cts.Token);
            else
                await Helpers.Youtube.Download(Song, Filepath, ProgressReport, cts.Token);
            await Helpers.Song.WriteMeta(Song, Filepath);

            Remove();
            await Helpers.Window.Notify("Song download finished!", $"{Song.Title}, by {Song.Artist}", Song.Artwork);
        }
        catch (Exception ex)
        {
            UpdateFlyout(false);

            if (!(ex is OperationCanceledException))
                await Helpers.Window.Alert(Content.XamlRoot, "Download failed!", ex.Message);
        }
    }
    public void Cancel()
    {
        cts.Cancel();
        UpdateFlyout(false);
    }
    public void Remove()
    {
        try
        {
            Downloads.Instance!.Container.Items.Remove(this);
        } catch { }
        Downloads.Instance!.Nothing.Visibility = Downloads.Instance!.Container.Items.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
        Downloads.Instance!.Queue.Remove(this);
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
            Progress.Value = 0;
        }
    }
}