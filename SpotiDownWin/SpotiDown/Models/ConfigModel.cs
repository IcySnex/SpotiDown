using SpotiDown.Enums;
using SpotiDown.Helpers;
using System;
using System.IO;

namespace SpotiDown.Models;

public class ConfigModel
{
    public static ConfigModel Load() =>
        File.Exists("config.json") ? Text.Deserialize<ConfigModel>("config") : new();

    public void Save() =>
       File.WriteAllText("config.json", Text.Serialize(this));

    public ConfigModel_MetaData MetaData { get; set; } = new();
    public ConfigModel_Preferences Preferences { get; set; } = new();
    public ConfigModel_Paths Paths { get; set; } = new();
}

public class ConfigModel_MetaData
{
    public bool Lyrics { get; set; } = true;
    public bool Artwork { get; set; } = true;
}

public class ConfigModel_Preferences
{
    public QualityType Quality { get; set; } = QualityType.Medium;
    public FormatType Format { get; set; } = FormatType.mp3;
}

public class ConfigModel_Paths
{
    public string Download { get; set; } = "\\SpotiDown";
    public string FileName { get; set; } = "{title} ({artist})";
    public string FFMPEG { get; set; } = "\\FFMPEG.exe";
}