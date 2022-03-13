using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using SpotiDown.Controls;
using SpotiDown.Models;
using System.Collections.Generic;

namespace SpotiDown.Pages;

public sealed partial class Downloads : Page
{
    public Downloads()
    {
        InitializeComponent();
        NavigationCacheMode = NavigationCacheMode.Required;
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        if (e.Parameter is Song[] Songs)
            foreach (Song Song in Songs)
                Container.Items.Add(new DownloadEntry(Song));
    }
}