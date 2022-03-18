using Microsoft.UI.Xaml.Controls;
using SpotiDown.Models;

namespace SpotiDown.Controls;

public sealed partial class TrackInfo : UserControl
{
    public TrackInfo(Song Song)
    {
        InitializeComponent();
        this.Song = Song;
    }

    public Song Song;
}
