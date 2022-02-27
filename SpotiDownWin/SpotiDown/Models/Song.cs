using SpotiDown.Enums;
using System;

namespace SpotiDown.Models;

public class Song
{
    public Song(SongType Type, string Url, string Title, string Artist, TimeSpan Duration, string Album, DateTime Release, string Lyrics, string Artwork, int Track, int Disc)
    {
        this.Type = Type;
        this.Url = Url;
        this.Title = Title;
        this.Artist = Artist;
        this.Duration = Duration;
        this.Album = Album;
        this.Release = Release;
        this.Lyrics = Lyrics;
        this.Artwork = Artwork;
        this.Track = Track;
        this.Disc = Disc;
    }

    public SongType Type { get; set; }
    public string Url { get; set; }
    public string Title { get; set; }
    public string Artist { get; set; }
    public TimeSpan Duration { get; set; }
    public string Album { get; set; }
    public DateTime Release { get; set; }
    public string Lyrics { get; set; }
    public string Artwork { get; set; }
    public int Track { get; set; }
    public int Disc { get; set; }
}