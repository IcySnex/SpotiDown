using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using SpotiDown.Controls;
using SpotiDown.Enums;
using SpotiDown.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SpotiDown.Pages;

public sealed partial class Library : Page
{
    public static Library? Instance { get; private set; }
    public Library()
    {
        InitializeComponent();
        NavigationCacheMode = NavigationCacheMode.Required;
        Instance = this;

        Watcher.Changed += async (s, e) => await UpdateFileSystem(e.FullPath);
        Watcher.Created += async (s, e) => await UpdateFileSystem(e.FullPath);
        Watcher.Deleted += async (s, e) => await UpdateFileSystem(e.FullPath);
        Watcher.Renamed += async (s, e) => await UpdateFileSystem(e.FullPath);

        try
        {
            GetContent(Helpers.Local.Config.Paths.Download, ref Entries);
            UpdateList(false);
        } catch (Exception ex)
        {
            Helpers.Window.Notify("Updating Library failed!", ex.Message, "ms-appx:///Assets/Error.png");
        }
    }


    private void UpdateList(bool Performance = true)
    {
        if (Entries.Count == 0)
        {
            Container.Items.Clear();
            Nothing.Visibility = Visibility.Visible;
            return;
        }

        IEnumerable<ILibrary> Filtered = Entries.Where(Library => Library.NameContains(Search.Text));

        if (Performance && Container.Items.SequenceEqual(Filtered))
            return;

        Container.Items.Clear();
        switch ((SortingType)Sorting.SelectedIndex)
        {
            case SortingType.Default:
                foreach (ILibrary Entry in Filtered)
                    Container.Items.Add(Entry);
                break;
            case SortingType.Default_Inv:
                foreach (ILibrary Entry in Filtered.Reverse())
                    Container.Items.Add(Entry);
                break;
            case SortingType.Title:
                foreach (ILibrary Entry in Filtered.OrderBy(r => r.ContainerName))
                    Container.Items.Add(Entry);
                break;
        }

        foreach (LibraryContainer Entry in Entries.Where(c => c.IsContainer))
            Entry.UpdateList(Search.Text, (SortingType)Sorting.SelectedIndex, Performance);

        Nothing.Visibility = Container.Items.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
    }

    public async Task UpdateFileSystem(string Reason)
    {
        try
        {
            if (!File.Exists(Reason) || await Helpers.Local.WaitFileLock(Reason, 10000))
                Sync.Post(async val => { 
                    await Task.Delay(1000);
                    Entries.Clear();
                    GetContent(Helpers.Local.Config.Paths.Download, ref Entries);
                    UpdateList();
                }, null);
        } catch (Exception ex)
        {
            Helpers.Window.Notify("Updating Library failed!", ex.Message, "ms-appx:///Assets/Error.png");
        }
    }


    private void Search_TextChanged(object sender, TextChangedEventArgs e) =>
        UpdateList();

    private void Sorting_SelectionChanged(object sender, SelectionChangedEventArgs? e) =>
        UpdateList(false);


    FileSystemWatcher Watcher = new(Helpers.Local.Config.Paths.Download) { EnableRaisingEvents = true, IncludeSubdirectories = true };
    SynchronizationContext Sync = SynchronizationContext.Current!;
    private List<ILibrary> Entries = new();


    private void GetContent(string Filepath, ref List<ILibrary> Result)
    {
        foreach (string Directory in Directory.GetDirectories(Filepath))
        {
            List<ILibrary> InnerResult = new();
            GetContent(Directory, ref InnerResult);
            if (InnerResult.Count > 0)
                Result.Add(new LibraryContainer(new(Path.GetFileName(Directory) is string Name ? Name : "N/A", "", InnerResult)));
        }

        foreach (string File in Directory.GetFiles(Filepath).Where(Path => Helpers.Text.EndsWithAny(Path, ".mp3", ".wav", ".m4a", ".aac", ".ogg", ".pcm", ".flac")))
            Result.Add(new LibraryEntry(Helpers.Song.LoadFile(File)));
    }
}