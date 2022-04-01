using Microsoft.UI.Xaml.Controls;
using SpotiDown.Models;
using System;
using System.Collections.Generic;
using Windows.Storage;
using Windows.Storage.Pickers;
using WinRT.Interop;

namespace SpotiDown.Controls;

public sealed partial class TrackInfo : UserControl
{
    public TrackInfo(Song Song)
    {
        InitializeComponent();
        this.Song = Song;

        ArtworkSelector.FileTypeFilter.Add(".png");
        ArtworkSelector.FileTypeFilter.Add(".jpeg");
        ArtworkSelector.FileTypeFilter.Add(".jpg");
        InitializeWithWindow.Initialize(ArtworkSelector, Helpers.Window.GetHandle(App.Window));
    }

    public Song Song;
    FileOpenPicker ArtworkSelector = new() { SuggestedStartLocation = PickerLocationId.Desktop, CommitButtonText = "Select" };
    List<Genius_Response_Hit> LyricsResults = new();
    int LyricsSelected = 0;


    private void Image_PointerEntered(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e) =>
        Image_FadeIn.Begin();
    private void Image_PointerExited(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e) =>
        Image_FadeOut.Begin();

    private async void Edit_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        StorageFile File = await ArtworkSelector.PickSingleFileAsync();
        if (File is StorageFile && !string.IsNullOrWhiteSpace(File.Path))
        {
            Artwork.ImageSource = Helpers.Local.DownloadImage(File.Path);
            Song.Artwork = $":::{File.Path}";
        }
    }


    private async void OpenUrl_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e) =>
        await Helpers.Local.OpenUrl(Song.Url);

    private async void OpenSource_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        switch (Song.Type)
        {
            case Enums.SongType.Spotify:
                await Helpers.Local.OpenUrl("https://open.spotify.com/");
                break;
            case Enums.SongType.YouTube:
                await Helpers.Local.OpenUrl("https://www.youtube.com/");
                break;
            case Enums.SongType.SoundCloud:
                await Helpers.Local.OpenUrl("https://soundcloud.com/");
                break;
        }
    }


    private void TitleArtist_TextChanged(object sender, TextChangedEventArgs e) =>
        ToolTipService.SetToolTip(LyricsSearch, $"Search on genius.com with the query \"{SongTitle.Text} {SongArtist.Text}\"");


    private void Lyrics_Loaded(object sender, Microsoft.UI.Xaml.RoutedEventArgs e) =>
        Lyrics.Text = Song.Lyrics;

    private void Lyrics_Unloaded(object sender, Microsoft.UI.Xaml.RoutedEventArgs e) =>
        Song.Lyrics = Lyrics.Text;

    private async void LyricsSearch_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        try
        {
            Lyrics.Text = "Searching Lyrics...";
            LyricsResults = await Helpers.Song.SearchLyrics(Song.Title, Song.Artist);
            
            switch (LyricsResults.Count)
            {
                case 0:
                    Lyrics.Text = "Could not find any lyrics with the given title/artist!";
                    LyricsBack.IsEnabled = false;
                    LyricsNext.IsEnabled = false;
                    LyricsSelected = 0;
                    LyricsCount.Text = $"0/0";
                    return;
                case 1:
                    Lyrics.Text = await Helpers.Song.GetLyrics(LyricsResults[0].result);
                    LyricsNext.IsEnabled = false;
                    break;
                default:
                    Lyrics.Text = await Helpers.Song.GetLyrics(LyricsResults[0].result);
                    LyricsNext.IsEnabled = true;
                    break;
            }

            LyricsBack.IsEnabled = false;
            LyricsSelected = 1;
            LyricsCount.Text = $"1/{LyricsResults.Count}";

        }
        catch (Exception ex)
        {
            Lyrics.Text = $"Failed searching for lyrics! (Ex: {ex.Message})";
            LyricsBack.IsEnabled = false;
            LyricsNext.IsEnabled = false;
            LyricsSelected = 0;
            LyricsCount.Text = $"0/0";
        }
    }

    private async void LyricsBack_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        try
        {
            if (LyricsSelected < 2)
                return;

            Lyrics.Text = "Loading Lyrics...";
            Lyrics.Text = await Helpers.Song.GetLyrics(LyricsResults[LyricsSelected - 2].result);

            LyricsSelected -= 1;
            LyricsCount.Text = $"{LyricsSelected}/{LyricsResults.Count}";

            LyricsBack.IsEnabled = LyricsSelected > 1;
            LyricsNext.IsEnabled = LyricsSelected < LyricsResults.Count;
        }
        catch (Exception ex)
        {
            Lyrics.Text = $"Failed loading lyrics! (Ex: {ex.Message})";
            LyricsBack.IsEnabled = false;
            LyricsNext.IsEnabled = false;
            LyricsSelected = 0;
            LyricsCount.Text = $"0/0";
        }
    }

    private async void LyricsNext_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        try
        {
            if (LyricsSelected > LyricsResults.Count - 1)
                return;

            Lyrics.Text = "Loading Lyrics...";
            Lyrics.Text = await Helpers.Song.GetLyrics(LyricsResults[LyricsSelected].result);

            LyricsSelected += 1;
            LyricsCount.Text = $"{LyricsSelected}/{LyricsResults.Count}";

            LyricsBack.IsEnabled = LyricsSelected > 1;
            LyricsNext.IsEnabled = LyricsSelected < LyricsResults.Count;
        }
        catch (Exception ex)
        {
            Lyrics.Text = $"Failed loading lyrics! (Ex: {ex.Message})";
            LyricsBack.IsEnabled = false;
            LyricsNext.IsEnabled = false;
            LyricsSelected = 0;
            LyricsCount.Text = $"0/0";
        }
    }
}
