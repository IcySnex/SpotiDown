using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Linq;
using SpotiDown.Controls;
using SpotiDown.Enums;
using SpotiDown.Models;
using System;
using System.Threading;
using System.Collections.Generic;
using System.Threading.Tasks;
using SpotiDown.Helpers;
using Microsoft.UI.Xaml.Navigation;

namespace SpotiDown.Pages;

public sealed partial class Spotify : Page
{
    public Spotify()
    {
        NavigationCacheMode = NavigationCacheMode.Required;
        InitializeComponent();
        Helpers.Spotify.Initialize();
    }

    protected override async void OnNavigatedTo(NavigationEventArgs e)
    {
        if (e.Parameter is string Query)
        {
            this.Query.Text = Query;
            await Task.Delay(500);
            Search_Click(this, null);
        }

    }

    private void UpdateDownloadBar(bool Enabled)
    {
        if (Enabled)
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
    private void DownloadBar_FadeOut_Completed(object sender, object e)
    {
        if (DownloadBar.Opacity == 0)
            DownloadBar.Visibility = Visibility.Collapsed;
    }

    private void UpdateLoading(bool Enabled, string Type = "Searching on Spotify...")
    {
        if (Enabled)
        {
            Loading_Type.Text = Type;
            LoadingBox.Visibility = Visibility.Visible;
            Loading_FadeIn.Begin();
        }
        else
            Loading_FadeOut.Begin();
    }
    private void Loading_FadeOut_Completed(object sender, object e) =>
            LoadingBox.Visibility = Visibility.Collapsed;


    private void UpdateList()
    {
        if (Result.Count() == 0)
            return;

        Container.Items.Clear();
        switch ((SortingType)Sorting.SelectedIndex)
        {
            case SortingType.Default:
                foreach (SpotifyEntry Entry in Result.Select(Song => new SpotifyEntry(Song)))
                    Container.Items.Add(Entry);
                break;
            case SortingType.Default_Inv:
                foreach (SpotifyEntry Entry in Result.Reverse().Select(Song => new SpotifyEntry(Song)))
                    Container.Items.Add(Entry);
                break;
            case SortingType.Title:
                foreach (SpotifyEntry Entry in Result.OrderBy(r => r.Title).Select(Song => new SpotifyEntry(Song)))
                    Container.Items.Add(Entry);
                break;
            case SortingType.Artist:
                foreach (SpotifyEntry Entry in Result.OrderBy(r => r.Artist).Select(Song => new SpotifyEntry(Song)))
                    Container.Items.Add(Entry);
                break;
            case SortingType.Duration:
                foreach (SpotifyEntry Entry in Result.OrderBy(r => r.Duration).Select(Song => new SpotifyEntry(Song)))
                    Container.Items.Add(Entry);
                break;
        }
    }


    private void Container_SelectionChanged(object sender, SelectionChangedEventArgs? e) =>
        UpdateDownloadBar(Container.SelectedItems.Count > 0 ? true : false);

    private void Sorting_SelectionChanged(object sender, SelectionChangedEventArgs? e) =>
        UpdateList();


    CancellationTokenSource cts = new();
    IEnumerable<SpotifySong> Result = Enumerable.Empty<SpotifySong>();

    private async void Search_Click(object sender, RoutedEventArgs? e)
    {
        if (string.IsNullOrWhiteSpace(Query.Text))
        {
            await Helpers.Window.Alert(Content.XamlRoot, "Search failed!", "Please type in a query or paste an URL in to start downloading.");
            return;
        }
        cts.Dispose();
        cts = new CancellationTokenSource();

        try
        {
            UpdateLoading(true);

            SpotifySearchType Type = Helpers.Spotify.GetSearchType(Query.Text);
            Result = await Helpers.Spotify.Search(Query.Text, Type, cts.Token);

            Nothing.Visibility = Visibility.Visible;

            if (Result.Count() == 0)
            {
                LoadingBox.Opacity = 0;
                LoadingBox.Visibility = Visibility.Collapsed;
                Container.Items.Clear();
                await Helpers.Window.Alert(Content.XamlRoot, "Search failed!", "Could not find any songs based on the given query.");
                return;
            }

            UpdateList();

            if (Type != SpotifySearchType.Query)
                Container.SelectAll();

            Nothing.Visibility = Visibility.Collapsed;
            UpdateLoading(false);
        }
        catch (Exception ex)
        {
            UpdateLoading(false);

            if (!(ex is OperationCanceledException))
                await Helpers.Window.Alert(Content.XamlRoot, "Search failed!", ex.Message);
        }
    }
    private void Query_KeyDown(object sender, Microsoft.UI.Xaml.Input.KeyRoutedEventArgs e)
    {
        if (e.Key == Windows.System.VirtualKey.Enter)
            Search_Click(sender, null);
    }


    CancellationTokenSource ctsdl = new();

    private async void Download_Click(object sender, RoutedEventArgs e)
    {
        ctsdl.Dispose();
        ctsdl = new CancellationTokenSource();

        UpdateLoading(true, "Preparing Download...");

        try
        {
            var Parameter = await Task.WhenAll(
             Container.SelectedItems
             .WithCancellation(ctsdl.Token)
             .Select(async Entry => await Helpers.Spotify.Convert(((SpotifyEntry)Entry).SpotifySong, Text.GetBool(SaveLyrics.IsChecked), Text.GetBool(SaveArtwork.IsChecked), ctsdl.Token)));

            UpdateLoading(false);

            if (!ctsdl.IsCancellationRequested)
                Helpers.Window.Navigate("Downloads", Parameter);
        }
        catch (Exception ex)
        {
            UpdateLoading(false);

            if (!(ex is OperationCanceledException))
                await Helpers.Window.Alert(Content.XamlRoot, "Preparing Downloads failed!", $"{ex.Message}{(ex.InnerException != null ? $"\nInner: {ex.InnerException.Message}" : "")}");
        }
    }


    private void Cancel_Click(object sender, RoutedEventArgs e)
    {
        if (Loading_Type.Text == "Searching on YouTube...")
            cts.Cancel();
        else
            ctsdl.Cancel();
    }
}
