using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;
using SpotiDown.Controls;
using SpotiDown.Enums;
using SpotiDown.Models;
using System;
using System.Threading.Tasks;

namespace SpotiDown.Pages;

public sealed partial class YouTube : Page
{
    public YouTube()
    {
        NavigationCacheMode = Microsoft.UI.Xaml.Navigation.NavigationCacheMode.Required;
        InitializeComponent();
    }

    private void UpdateUI(bool WithDownloadBar)
    {
        if (WithDownloadBar)
        {
            DownloadBar.Visibility = Visibility.Visible;
            DownloadBar_FadeIn.Begin();
            ScrollContainer.Margin = new(0, 110, 0, 110);
        }
        else
        {
            DownloadBar_FadeOut.Begin();
            ScrollContainer.Margin = new(0, 110, 0, 0);
        }
    }

    private void DownloadBar_FadeOut_Completed(object sender, object e) =>
            DownloadBar.Visibility = Visibility.Collapsed;

    private void Container_SelectionChanged(object sender, SelectionChangedEventArgs e) =>
        UpdateUI(Container.SelectedItems.Count > 0 ? true : false);

    private void Search_Click(object sender, RoutedEventArgs e)
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
        SongModel Song1 = new(
            SongType.YouTube,
            "https://www.youtube.com/watch?v=-Mdkbg0nyWA&ab_channel=1nonly",
            "TheseHoes",
            "SexyMexy",
            new(0, 2, 21),
            "HIIII",
            DateTime.UtcNow,
            "Hey,\nThis\nis some random ass lyrics\nFUCKKKK\nHelp me\nIM being kidnapped LOOOL\n\n Anyways\nHIIIIII",
            "https://img.youtube.com/vi/-Mdkbg0nyWA/maxresdefault.jpg",
            1,
            34);
        Container.Items.Add(new YoutubeEntry(Song));
        Container.Items.Add(new YoutubeEntry(Song1));
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
    }
}
