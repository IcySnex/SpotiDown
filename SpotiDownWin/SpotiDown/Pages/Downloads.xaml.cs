using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using SpotiDown.Controls;

namespace SpotiDown.Pages;

public sealed partial class Downloads : Page
{
    public Downloads()
    {
        InitializeComponent();
        NavigationCacheMode = Microsoft.UI.Xaml.Navigation.NavigationCacheMode.Required;
        RootDownloadsPage = this;
        RootDownloadsPage.Container.Items.Add(new DownloadEntry(Helpers.Download.Song));
    }

    public static Downloads? RootDownloadsPage { get; private set; }
}