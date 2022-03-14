using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using SpotiDown.Controls;
using SpotiDown.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SpotiDown.Pages;

public sealed partial class Downloads : Page
{
    public Downloads()
    {
        InitializeComponent();
        NavigationCacheMode = NavigationCacheMode.Required;
    }

    private void UpdateList(bool Performance = true)
    {
        if (Queue.Length == 0)
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
            Queue = Songs;
            UpdateList(false);
        }
    }


    private void Search_TextChanged(object sender, TextChangedEventArgs e) =>
        UpdateList();

    private void Show_Changed(object sender, RoutedEventArgs e) =>
        UpdateList();



    DownloadEntry[] Queue = Array.Empty<DownloadEntry>();


    private async void Clear_Click(object sender, RoutedEventArgs e)
    {
        if (await Helpers.Window.Alert(Content.XamlRoot, "Delete All Track", "Do you really want to delete all tracks from the download queue?", "No", "Yes") == ContentDialogResult.Primary)
        {
            Queue = Array.Empty<DownloadEntry>();
            Container.Items.Clear();
            Nothing.Visibility = Visibility.Visible;

        }
    }
}