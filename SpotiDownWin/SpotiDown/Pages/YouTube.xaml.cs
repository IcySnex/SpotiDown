using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Linq;
using SpotiDown.Controls;
using SpotiDown.Enums;
using SpotiDown.Models;
using System;
using System.Threading;
using System.Collections.Generic;

namespace SpotiDown.Pages;

public sealed partial class Youtube : Page
{
    public Youtube()
    {
        NavigationCacheMode = Microsoft.UI.Xaml.Navigation.NavigationCacheMode.Required;
        InitializeComponent();
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

    private void UpdateLoading(bool Enabled)
    {
        if (Enabled)
        {
            LoadingBox.Visibility = Visibility.Visible;
            Loading_FadeIn.Begin();
        }
        else
            Loading_FadeOut.Begin();
    }
    private void Loading_FadeOut_Completed(object sender, object e) =>
            LoadingBox.Visibility = Visibility.Collapsed;


    private void Container_SelectionChanged(object sender, SelectionChangedEventArgs? e) =>
        UpdateDownloadBar(Container.SelectedItems.Count > 0 ? true : false);

    private void Sorting_SelectionChanged(object sender, SelectionChangedEventArgs? e)
    {
        if (Result.Count() == 0)
            return;

        Container.Items.Clear();
        switch ((SortingType)Sorting.SelectedIndex)
        {
            case SortingType.Default:
                foreach (YoutubeEntry Entry in Result.Select(Song => new YoutubeEntry(Song)))
                    Container.Items.Add(Entry);
                break;
            case SortingType.Default_Rev:
                foreach (YoutubeEntry Entry in Result.Reverse().Select(Song => new YoutubeEntry(Song)))
                    Container.Items.Add(Entry);
                break;
            case SortingType.Title:
                foreach (YoutubeEntry Entry in Result.OrderBy(r => r.Title).Select(Song => new YoutubeEntry(Song)))
                    Container.Items.Add(Entry);
                break;
            case SortingType.Artist:
                foreach (YoutubeEntry Entry in Result.OrderBy(r => r.Artist).Select(Song => new YoutubeEntry(Song)))
                    Container.Items.Add(Entry);
                break;
            case SortingType.Duration:
                foreach (YoutubeEntry Entry in Result.OrderBy(r => r.Duration).Select(Song => new YoutubeEntry(Song)))
                    Container.Items.Add(Entry);
                break;
        }
    }


    CancellationTokenSource cts = new();
    IEnumerable<YoutubeSong> Result = Enumerable.Empty<YoutubeSong>();


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

            YoutubeSearchType Type = Helpers.Youtube.GetSearchType(Query.Text);
            Result = await Helpers.Youtube.Search(Query.Text, Type, (int)ResultCount.Value, cts.Token);

            Nothing.Visibility = Visibility.Visible;

            if (Result.Count() == 0)
            {
                LoadingBox.Opacity = 0;
                LoadingBox.Visibility = Visibility.Collapsed;
                await Helpers.Window.Alert(Content.XamlRoot, "Search failed!", "Could not find any songs based on the given query.");
                return;
            }

            Sorting_SelectionChanged(this, null);

            if (Type != YoutubeSearchType.Query)
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


    private void Cancel_Click(object sender, RoutedEventArgs e) =>
        cts.Cancel();


    private void Download_Click(object sender, RoutedEventArgs e)
    {
        Query.Text = Container.Items.Count.ToString();
    }
}
