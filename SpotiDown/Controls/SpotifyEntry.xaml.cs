using Microsoft.UI.Xaml.Controls;
using SpotiDown.Models;

namespace SpotiDown.Controls;

public sealed partial class SpotifyEntry : UserControl
{
    public SpotifyEntry(SpotifySong SpotifySong)
    {
        InitializeComponent();
        this.SpotifySong = SpotifySong;
    }
    
    public SpotifySong SpotifySong;
}