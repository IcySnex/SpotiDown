using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Linq;
using SpotiDown.Controls;
using SpotiDown.Enums;
using SpotiDown.Models;
using System;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.UI.Xaml.Data;

namespace SpotiDown.Pages;

public sealed partial class Youtube : Page
{
    public Youtube()
    {
        NavigationCacheMode = Microsoft.UI.Xaml.Navigation.NavigationCacheMode.Required;
        InitializeComponent();
    }

    #region Update
    #region DownloadBar
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
    private void DownloadBar_FadeOut_Completed(object sender, object e) =>
            DownloadBar.Visibility = Visibility.Collapsed;
    #endregion
    #region Loading
    private void UpdateLoading(bool Enabled)
    {
        if (Enabled)
        {
            Loading.Visibility = Visibility.Visible;
            Loading_FadeIn.Begin();
        }
        else
            Loading_FadeOut.Begin();
    }
    private void Loading_FadeOut_Completed(object sender, object e) =>
            Loading.Visibility = Visibility.Collapsed;
    #endregion
    #endregion

    private void Container_SelectionChanged(object sender, SelectionChangedEventArgs e) =>
        UpdateDownloadBar(Container.SelectedItems.Count > 0 ? true : false);

    private void Sorting_SelectionChanged(object sender, SelectionChangedEventArgs? e)
    {
        Container.Items.Clear();
        switch (Sorting.SelectedIndex)
        {
            case 0:
                Container.Items.Clear();
                foreach (YoutubeEntry Entry in Result.Select(Song => new YoutubeEntry(Song)))
                    Container.Items.Add(Entry);
                break;
            case 1:
                Container.Items.Clear();
                foreach (YoutubeEntry Entry in Result.OrderBy(r => r.Title).Select(Song => new YoutubeEntry(Song)))
                    Container.Items.Add(Entry);
                break;
            case 2:
                Container.Items.Clear();
                foreach (YoutubeEntry Entry in Result.OrderBy(r => r.Artist).Select(Song => new YoutubeEntry(Song)))
                    Container.Items.Add(Entry);
                break;
            case 3:
                Container.Items.Clear();
                foreach (YoutubeEntry Entry in Result.OrderBy(r => r.Duration).Select(Song => new YoutubeEntry(Song)))
                    Container.Items.Add(Entry);
                break;
        }
    }

    CancellationTokenSource cts = new();
    IEnumerable<YoutubeSong> Result = Enumerable.Empty<YoutubeSong>();

    private async void Search_Click(object sender, RoutedEventArgs e)
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
                Loading.Opacity = 0;
                Loading.Visibility = Visibility.Collapsed;
                await Helpers.Window.Alert(Content.XamlRoot, "Search failed!", "Could not find any songs based on the given query.");
                return;
            }

            Sorting_SelectionChanged(this, null);

            if (Type != YoutubeSearchType.Query)
            {
                Container.SelectAll();
                DownloadBar.Visibility = Visibility.Visible;
                DownloadBar.Opacity = 1;
                ScrollContainer.Margin = new(0, 110, 0, 110);
            }

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

    private void Cancel_Click(object sender, RoutedEventArgs e) =>
        cts.Cancel();

    private void Download_Click(object sender, RoutedEventArgs e)
    {
        Query.Text = Container.Items.Count.ToString();
    }
}
