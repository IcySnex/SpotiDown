using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;
using SpotiDown.Models;
using System;

namespace SpotiDown.Controls;

public sealed partial class YoutubeEntry : UserControl
{
    public YoutubeEntry(YoutubeSong Song)
    {
        InitializeComponent();
        this.Song = Song;
        UpdateUI();
    }
    
    public YoutubeSong Song;

    public void UpdateUI()
    {
        Title.Text = Song.Title;
        Artist.Text = Song.Artist;
        Duration.Text = Helpers.Text.FormatTime(Song.Duration);
        if (Song.Artwork != null)
            Artwork.ImageSource = new BitmapImage(new Uri(Song.Artwork));
    }

    private void Info(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {

    }
}