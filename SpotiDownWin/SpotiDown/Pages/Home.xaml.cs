using Microsoft.UI.Xaml.Controls;
using System;

namespace SpotiDown.Pages;

public sealed partial class Home : Page
{
    public Home()
    {
        NavigationCacheMode = Microsoft.UI.Xaml.Navigation.NavigationCacheMode.Required;
        InitializeComponent();
    }

    private async void Github_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        => await Helpers.Local.AskUrl(this, "View Source Code", "https://github.com/IcySnex/SpotiDown");

    private async void Share_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        if (await Helpers.Window.Alert(this, "Share Project", "Do you want to copy the project (GitHub) url to your clipboard?", "No", "Yes") == ContentDialogResult.Primary)
            Helpers.Local.SetClipboard("https://github.com/IcySnex/SpotiDown");
    }

    private async void Discord_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        => await Helpers.Local.AskUrl(this, "Join Discord Server", "https://discord.com/invite/Z8kweuryux");

    private async void Youtube_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        => await Helpers.Local.AskUrl(this, "View YouTube Channel", "https://www.youtube.com/channel/UCkAQG8OkLKgi_WAR526pd9w");
}
