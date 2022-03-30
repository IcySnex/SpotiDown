using Microsoft.UI.Xaml.Controls;
using SpotiDown.Models;
using System;
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

    private void Lyrics_Loaded(object sender, Microsoft.UI.Xaml.RoutedEventArgs e) =>
        Lyrics.Text = Song.Lyrics;

    private void Lyrics_Unloaded(object sender, Microsoft.UI.Xaml.RoutedEventArgs e) =>
        Song.Lyrics = Lyrics.Text;
}
