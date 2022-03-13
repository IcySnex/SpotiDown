using System;

namespace SpotiDown.Models;

public class YoutubeSong
{
    public YoutubeSong(string Id, string Title, string Channel, string? Playlist, TimeSpan Duration, string Thumbnail)
    {
        this.Id = Id;
        this.Title = Title;
        this.Channel = Channel;
        this.Playlist = Playlist;
        this.Duration = Duration;
        this.Thumbnail = Thumbnail;
    }

    public string Id { get; set; }
    public string Title { get; set; }
    public string Channel { get; set; }
    public string? Playlist { get; set; }
    public TimeSpan Duration { get; set; }
    public string Thumbnail { get; set; }
}