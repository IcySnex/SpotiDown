using SpotiDown.Enums;
using SpotiDown.Helpers;
using System;
using System.IO;

namespace SpotiDown.Models;

public class Config
{
    public static Config Load() =>
        File.Exists("Config.json") ? Text.Deserialize<Config>("Config") : new();

    public void Save() =>
       File.WriteAllText("Config.json", Text.Serialize(this));

    public ConfigModel_Preferences SpotifyPrefernces { get; set; } = new();
    public ConfigModel_Preferences YoutubePreferences { get; set; } = new();
    public ConfigModel_DownloadSettings DownloadSettings { get; set; } = new();
    public ConfigModel_LibrarySettings LibrarySettings { get; set; } = new();
    public ConfigModel_Paths Paths { get; set; } = new();
    public ConfigModel_Advanced Advanced { get; set; } = new();
}

public class ConfigModel_Preferences
{
    public int Quality { get; set; } = (int)QualityType._160kbps;
    public int Format { get; set; } = (int)FormatType.mp3;
    public bool Lyrics { get; set; } = true;
    public bool Artwork { get; set; } = true;
    public int Sorting { get; set; } = (int)SortingType.Default;
    public int ResultCount { get; set; } = 50;
}

public class ConfigModel_DownloadSettings
{
    public int DefaultIfExists { get; set; } = (int)DefaultIfExistsType.Ask;
    public int ArtworkQuality = 512;
    public bool ShowSpotify { get; set; } = true;
    public bool ShowYoutube { get; set; } = true;
}

public class ConfigModel_LibrarySettings
{
    public int Sorting { get; set; } = (int)SortingType.Default;
}

public class ConfigModel_Paths
{
    public string Download { get; set; } = "SpotiDown\\";
    public string FileName { get; set; } = "{title} ({artist})";
    public string FFMPEG { get; set; } = "FFMPEG.exe";
}

public class ConfigModel_Advanced
{
    public bool CustomDownload { get; set; } = true;
    public ConfigModel_Advanced_Spotify Spotify { get; set; } = new();
}

public class ConfigModel_Advanced_Spotify
{
    public bool SavePlaylistAsAlbum { get; set; } = true;
    public string YoutubeSearchAlgorithm { get; set; } = "{title} {artist}";
    public string SearchMarketCode { get; set; } = "US";
    public string Id { get; set; } = "bd6035158be648b695451636f2e311f2";
    public string Secret { get; set; } = "0dd522b08dc646559e5fc7ef092fa9d7";
}