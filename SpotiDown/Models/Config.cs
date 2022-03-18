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
    public ConfigModel_Preferences SoundCloudPrefernces { get; set; } = new();
    public ConfigModel_DownloadSettings DownloadSettings { get; set; } = new();
    public ConfigModel_Paths Paths { get; set; } = new();
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
    public bool ShowSpotify { get; set; } = true;
    public bool ShowYoutube { get; set; } = true;
    public bool ShowSoundcloud { get; set; } = true;
    public int MaxDownloads { get; set; } = 1;
}

public class ConfigModel_Paths
{
    public string Download { get; set; } = "SpotiDown";
    public string FileName { get; set; } = "{title} ({artist})";
    public string FFMPEG { get; set; } = "FFMPEG.exe";
}