using System;

namespace SpotiDown.Models;

public class YoutubeSongModel
{
    public YoutubeSongModel(string Url, string Title, string Artist, TimeSpan Duration, DateTime AddedOn, string Artwork)
    {
        this.Url = Url;
        this.Title = Title;
        this.Artist = Artist;
        this.Duration = Duration;
        this.AddedOn = AddedOn;
        this.Artwork = Artwork;
    }

    public string Url { get; set; }
    public string Title { get; set; }
    public string Artist { get; set; }
    public TimeSpan Duration { get; set; }
    public DateTime AddedOn { get; set; }
    public string Artwork { get; set; }
}