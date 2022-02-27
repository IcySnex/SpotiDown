using System;

namespace SpotiDown.Models;

public class YoutubeSong
{
    public YoutubeSong(string Id, string Title, string Artist, TimeSpan Duration, string? Artwork)
    {
        this.Id = Id;
        this.Title = Title;
        this.Artist = Artist;
        this.Duration = Duration;
        this.Artwork = Artwork;
    }

    public string Id { get; set; }
    public string Title { get; set; }
    public string Artist { get; set; }
    public TimeSpan Duration { get; set; }
    public string? Artwork { get; set; }
}