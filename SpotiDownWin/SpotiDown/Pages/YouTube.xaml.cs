using Microsoft.UI.Xaml.Controls;
using SpotiDown.Controls;
using SpotiDown.Enums;
using SpotiDown.Models;
using System;

namespace SpotiDown.Pages;

public sealed partial class YouTube : Page
{
    public YouTube()
    {
        NavigationCacheMode = Microsoft.UI.Xaml.Navigation.NavigationCacheMode.Required;
        InitializeComponent();
    }

    private void Search(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        SongModel Song = new(
            SongType.YouTube,
            "https://www.youtube.com/watch?v=-Mdkbg0nyWA&ab_channel=1nonly",
            "HeyHo song",
            "Fortniter187",
            new(1, 32, 41),
            "SexSounds",
            DateTime.UtcNow,
            "Hey,\nThis\nis some random ass lyrics\nFUCKKKK\nHelp me\nIM being kidnapped LOOOL\n\n Anyways\nHIIIIII",
            "https://img.youtube.com/vi/-Mdkbg0nyWA/maxresdefault.jpg",
            1,
            34);
        Container.Items.Add(new YoutubeEntry(Song));
    }
}
