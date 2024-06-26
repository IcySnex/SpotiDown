﻿using Microsoft.UI.Xaml.Controls;

namespace SpotiDown.Pages;

public sealed partial class Home : Page
{
    public Home()
    {
        NavigationCacheMode = Microsoft.UI.Xaml.Navigation.NavigationCacheMode.Required;
        InitializeComponent();
    }

    private async void Github_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e) =>
        await Helpers.Local.AskUrl(Content.XamlRoot, "View Source Code", "https://github.com/IcySnex/SpotiDown");

    private async void Share_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        if (await Helpers.Window.Alert(Content.XamlRoot, "Share Project", "Do you want to copy the project (GitHub) url to your clipboard?", "No", "Yes") == ContentDialogResult.Primary)
            Helpers.Local.SetClipboard("https://github.com/IcySnex/SpotiDown");
    }

    private async void Discord_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e) =>
        await Helpers.Local.AskUrl(Content.XamlRoot, "Join Discord Server", "https://discord.com/invite/Z8kweuryux");

    private async void Youtube_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e) =>
        await Helpers.Local.AskUrl(Content.XamlRoot, "View YouTube Channel", "https://www.youtube.com/channel/UCkAQG8OkLKgi_WAR526pd9w");


    private void YoutubeSearch_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e) =>
        Helpers.Window.Navigate("YouTube", Query.Text);

    private void SpotifySearch_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e) =>
        Helpers.Window.Navigate("Spotify", Query.Text);

    private void Search_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e) =>
        Helpers.Window.Navigate(Query.Text.Contains("Spotify", System.StringComparison.InvariantCultureIgnoreCase) ? "Spotify" : "YouTube", Query.Text);
}
