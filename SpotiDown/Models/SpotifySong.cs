using System;

namespace SpotiDown.Models;

public class SpotifySong
{
    public SpotifySong(string Title, string Artist, string? Album, TimeSpan Duration, DateTime Release, string? Artwork)
    {
        this.Title = Title;
        this.Artist = Artist;
        this.Album = Album;
        this.Duration = Duration;
        this.Release = Release;
        this.Artwork = Artwork;
    }

    public string Title { get; set; }
    public string Artist { get; set; }
    public string? Album { get; set; }
    public TimeSpan Duration { get; set; }
    public DateTime Release { get; set; }
    public string? Artwork { get; set; }
}