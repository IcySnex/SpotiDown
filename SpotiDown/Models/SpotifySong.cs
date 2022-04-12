using System;

namespace SpotiDown.Models;

public class SpotifySong
{
    public SpotifySong(string Title, string Artist, string? Album, TimeSpan Duration, string? Artowrk)
    {
        this.Title = Title;
        this.Artist = Artist;
        this.Album = Album;
        this.Duration = Duration;
        this.Artowrk = Artowrk;
    }

    public string Title { get; set; }
    public string Artist { get; set; }
    public string? Album { get; set; }
    public TimeSpan Duration { get; set; }
    public string? Artowrk { get; set; }
}