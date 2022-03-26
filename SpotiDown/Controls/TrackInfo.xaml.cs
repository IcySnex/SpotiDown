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

    private void Image_PointerEntered(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e) =>
        Image_FadeIn.Begin();
    private void Image_PointerExited(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e) =>
        Image_FadeOut.Begin();
}
