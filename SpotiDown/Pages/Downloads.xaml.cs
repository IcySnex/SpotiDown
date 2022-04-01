using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using SpotiDown.Controls;
using System;
using System.Collections.Generic;
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

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        if (e.Parameter is DownloadEntry[] Songs)
        {
            Queue.AddRange(Songs);
            UpdateList(false);
        }
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


    private void Search_TextChanged(object sender, TextChangedEventArgs e) =>
        UpdateList();

    private void Show_Changed(object sender, RoutedEventArgs e) =>
        UpdateList();


    public List<DownloadEntry> Queue = new();
    CancellationTokenSource cts = new();
    CancellationTokenSource ctsff = new();


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
        ctsff.Dispose();
        ctsff = new CancellationTokenSource();

        try
        {
            UpdateLoading(true);
            Progress.IsIndeterminate = false;

            var Progres = new Progress<double>(value => Progress.Value = value * 100);
            await Helpers.Local.DownloadFile("https://github.com/IcySnex/SpotiDown/raw/main/other/ffmpeg.exe", Helpers.Local.Config.Paths.FFMPEG, Progres, ctsff.Token);

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
        ctsff.Cancel();
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


    private async void Download_Click(object sender, RoutedEventArgs e)
    {
        if (Container.Items.Count < 1)
        {
            await Helpers.Window.Alert(Content.XamlRoot, "Downloads failed!", "There are no downloads in the queue. You can search for songs with diefferent sources.");
            return;
        }
        cts.Dispose();
        cts = new CancellationTokenSource();

        Cancel.Visibility = Visibility.Visible;
        Download.Visibility = Visibility.Collapsed;
        Clear.IsEnabled = false;

        try
        {
            while (Container.Items.Count > 0)
            {
                cts.Token.ThrowIfCancellationRequested();
                await ((DownloadEntry)Container.Items.First(Entry => {
                    DownloadEntry Entr = (DownloadEntry)Entry;
                    return Entr.Progress.IsIndeterminate && !Entr.Skip; 
                } )).Download(cts.Token);
            }

            Cancel.Visibility = Visibility.Collapsed;
            Download.Visibility = Visibility.Visible;
            Clear.IsEnabled = true;

            if (await Helpers.Window.Alert(Content.XamlRoot, "Downloads finished!", $"All songs in the queue have been downloaded successfully! Do you want to view them in the Libary?", "No", "Yes") == ContentDialogResult.Primary)
                Helpers.Window.Navigate("Libary");
        }
        catch (Exception ex)
        {
            Cancel.Visibility = Visibility.Collapsed;
            Download.Visibility = Visibility.Visible;
            Clear.IsEnabled = true;

            if (ex is InvalidOperationException)
            {
                if (await Helpers.Window.Alert(Content.XamlRoot, "Downloads finished!", $"All songs in the queue have been downloaded successfully! Do you want to view them in the Libary?", "No", "Yes") == ContentDialogResult.Primary)
                    Helpers.Window.Navigate("Libary");
                return;
            }
            if (!(ex is OperationCanceledException))
                await Helpers.Window.Alert(Content.XamlRoot, "Downloads failed!", $"{ex.Message}{(ex.InnerException != null ? $"\nInner: {ex.InnerException.Message}" : "")}");
        }
    }

    private void CancelDownload_Click(object sender, RoutedEventArgs e) =>
        cts.Cancel();
}