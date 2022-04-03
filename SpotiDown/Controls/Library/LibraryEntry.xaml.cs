using Microsoft.UI.Xaml.Controls;
using SpotiDown.Enums;
using SpotiDown.Interfaces;
using SpotiDown.Models;

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
        Song.Title.Contains(Input, System.StringComparison.InvariantCultureIgnoreCase);
}