using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using SpotiDown.Controls;
using SpotiDown.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SpotiDown.Pages;

public sealed partial class Downloads : Page
{
    public static Downloads? Instance { get; private set; }
    public Downloads()
    {
        InitializeComponent();
        NavigationCacheMode = NavigationCacheMode.Required;
        Instance = this;
    }

    private void UpdateLoading(bool Enabled, string Type = "Downloading FFMPEG...")
    {
        if (Enabled)
        {
            Progress.IsIndeterminate = true;
            Progress.Value = 0;
            Loading_Type.Text = Type;
            LoadingBox.Visibility = Visibility.Visible;
            Loading_FadeIn.Begin();
        }
        else
            Loading_FadeOut.Begin();
    }
    private void Loading_FadeOut_Completed(object sender, object e) =>
        LoadingBox.Visibility = Visibility.Collapsed;

    private void UpdateList(bool Performance = true)
    {
        if (Queue.Count == 0)
            return;

        IEnumerable<DownloadEntry> Filtered;

        switch (SearchBy.SelectedIndex)
        {
            default:
                Filtered = Queue.Where(Entry => Entry.Song.Title.Contains(Search.Text, StringComparison.InvariantCultureIgnoreCase));
                break;
            case 1:
                Filtered = Queue.Where(Entry => Helpers.Text.NaIfEmpty(Entry.Song.Artist).Contains(Search.Text, StringComparison.InvariantCultureIgnoreCase));
                break;
            case 2:
                Filtered = Queue.Where(Entry => Helpers.Text.NaIfEmpty(Entry.Song.Album).Contains(Search.Text, StringComparison.InvariantCultureIgnoreCase));
                break;
        }

        if (!Helpers.Text.GetBool(ShowSpotify.IsChecked))
            Filtered = Filtered.Where(Entry => Entry.Song.Type != Enums.SongType.Spotify);
        if (!Helpers.Text.GetBool(ShowYoutube.IsChecked))
            Filtered = Filtered.Where(Entry => Entry.Song.Type != Enums.SongType.YouTube);
        if (!Helpers.Text.GetBool(ShowSoundcloud.IsChecked))
            Filtered = Filtered.Where(Entry => Entry.Song.Type != Enums.SongType.SoundCloud);

        if (Performance && Container.Items.SequenceEqual(Filtered))
            return;

        Container.Items.Clear();
        foreach (DownloadEntry Entry in Filtered)
            Container.Items.Add(Entry);

        Nothing.Visibility = Container.Items.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        if (e.Parameter is DownloadEntry[] Songs)
        {
            Queue = Songs.ToList();
            UpdateList(false);
        }
    }


    private void Search_TextChanged(object sender, TextChangedEventArgs e) =>
        UpdateList();

    private void Show_Changed(object sender, RoutedEventArgs e) =>
        UpdateList();


    public List<DownloadEntry> Queue = new();
    CancellationTokenSource cts = new();


    private async void Clear_Click(object sender, RoutedEventArgs e)
    {
        if (await Helpers.Window.Alert(Content.XamlRoot, "Delete All Track", "Do you really want to delete all tracks from the download queue?", "No", "Yes") == ContentDialogResult.Primary)
        {
            Queue.Clear();
            Container.Items.Clear();
            Nothing.Visibility = Visibility.Visible;

        }
    }

    public async Task<bool> DownloadFFMPEG()
    {
        cts.Dispose();
        cts = new CancellationTokenSource();

        try
        {
            UpdateLoading(true);
            Progress.IsIndeterminate = false;

            var Progres = new Progress<double>(value => Progress.Value = value * 100);
            await Helpers.Local.DownloadFile("https://github.com/IcySnex/SpotiDown/raw/main/other/ffmpeg.exe", Helpers.Local.Config.Paths.FFMPEG, Progres, cts.Token);

            UpdateLoading(false);
            return true;
        }
        catch (Exception ex)
        {
            UpdateLoading(false);

            if (!(ex is OperationCanceledException))
                await Helpers.Window.Alert(Content.XamlRoot, "Preparing Downloads failed!", $"{ex.Message}{(ex.InnerException != null ? $"\nInner: {ex.InnerException.Message}" : "")}");

            return false;
        }
    }

    private async void Cancel_Click(object sender, RoutedEventArgs e)
    {
        cts.Cancel();
        int timeout = 0;
        while (Helpers.Local.IsFileLocked(Helpers.Local.Config.Paths.FFMPEG))
        {
            if (timeout >= 12)
                return;
            timeout += 1;
            await Task.Delay(5000);
        }
        File.Delete(Helpers.Local.Config.Paths.FFMPEG);
    }
}