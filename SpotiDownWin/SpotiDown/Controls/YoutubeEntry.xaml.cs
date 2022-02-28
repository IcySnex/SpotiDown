using Microsoft.UI.Xaml.Controls;
using SpotiDown.Models;

namespace SpotiDown.Controls;

public sealed partial class YoutubeEntry : UserControl
{
    public YoutubeEntry(YoutubeSong YoutubeSong)
    {
        InitializeComponent();
        this.YoutubeSong = YoutubeSong;
    }
    
    public YoutubeSong YoutubeSong;
}