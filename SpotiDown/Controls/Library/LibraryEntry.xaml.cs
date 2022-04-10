using Microsoft.UI.Xaml.Controls;
using SpotiDown.Interfaces;
using SpotiDown.Models;
using System;
using System.Threading.Tasks;
using Windows.UI.Popups;
using WinRT.Interop;

namespace SpotiDown.Controls;

public sealed partial class LibraryEntry : UserControl, ILibrary
{
    public LibraryEntry(Song Song)
    {
        InitializeComponent();
        this.Song = Song;
    }

    public Song Song;


    bool ILibrary.IsContainer => false;

    string ILibrary.ContainerName { get => Song.Title; set => Song.Title = value; }

    public bool NameContains(string Input) =>
        Song.Title.Contains(Input, StringComparison.InvariantCultureIgnoreCase);


    private async void Play_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e) =>
        await Helpers.Local.OpenUrl(Song.Url);
    private async void TrackInfo_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        var cd = new ContentDialog
        {
            Title = "Track Info",
            Content = new TrackInfo((Song)Song.Clone()),
            CloseButtonText = "Cancel",
            PrimaryButtonText = "Save",
            XamlRoot = Content.XamlRoot
        };
        cd.PrimaryButtonClick += async (s, e) =>
        {
            try
            {
                await Helpers.Song.WriteMeta(((TrackInfo)cd.Content).Song, Song.Url, false);
            }
            catch (Exception ex)
            {
                await Task.Delay(500);
                await Helpers.Window.Alert(Content.XamlRoot, "Editing Track Info failed!", ex.Message);
            }
        };
        await cd.ShowAsync();
    }
    private async void Delete_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        if (await Helpers.Window.Alert(Content.XamlRoot, "Delete Song", "Do you really want to delete this song from your drive and libary? This cannot be undone!", "No", "Yes") == ContentDialogResult.Primary)
            try
            {
                await Helpers.Local.DeleteFile(Song.Url);
            } catch (Exception ex)
            {
                await Task.Delay(500);
                await Helpers.Window.Alert(Content.XamlRoot, "Deleting Song failed", ex.Message);
            }
    }
}