using Microsoft.UI.Xaml.Media.Imaging;
using SpotiDown.Enums;
using System;

namespace SpotiDown.Models;

public class Song : ICloneable
{
    public Song(SongType Type, string Url, string Title, string? Artist, TimeSpan Duration, string? Album, DateTime Release, string? Lyrics, string? Artwork, BitmapImage? ArtworkSource, int Track, int Total)
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
        this.ArtworkSource = ArtworkSource;
        this.Track = Track;
        this.Total = Total;
    }

    public SongType Type { get; set; }
    public string Url { get; set; }
    public string Title { get; set; }
    public string? Artist { get; set; }
    public TimeSpan Duration { get; set; }
    public string? Album { get; set; }
    public DateTimeOffset Release { get; set; }
    public string? Lyrics { get; set; }
    public string? Artwork { get; set; }
    public BitmapImage? ArtworkSource { get; set; }
    public int Track { get; set; }
    public int Total { get; set; }

    public object Clone() =>
        MemberwiseClone();
}