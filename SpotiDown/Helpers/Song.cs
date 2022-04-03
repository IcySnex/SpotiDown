using HtmlAgilityPack;
using SpotiDown.Enums;
using SpotiDown.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace SpotiDown.Helpers;

public class Song
{
    public static async Task<List<Genius_Response_Hit>> SearchLyrics(string Title, string? Artist) =>
        Text.Deserialize<Genius>(await Local.DownloadString($"https://api.genius.com/search?q={Title} {Artist}&access_token=7lBNcWe_5ljK10TnPh7q3j31clowVoGLN9IEYFbO9AcKsngZ49Iz7aeZ0D9W4H1j"), false) is Genius Search ? Search.response.hits : new();

    public static async Task<string> GetLyrics(Genius_Response_Hit_Result Song)
    {
        var Html = new HtmlDocument();
        Html.LoadHtml((await Local.DownloadString(Song.Url)).Replace("https://ajax.googleapis.com/ajax/libs/jquery/2.1.4/jquery.min.js", ""));

        var Clickables = Html.DocumentNode.SelectNodes("//a[contains(@class, 'ReferentFragmentVariantdesktop')]");
        if (Clickables != null)
            foreach (HtmlNode Node in Clickables)
                Node.ParentNode.ReplaceChild(Html.CreateTextNode(Node.ChildNodes[0].InnerHtml), Node);
        var Spans = Html.DocumentNode.SelectNodes("//span");
        if (Spans != null)
            foreach (HtmlNode Node in Spans)
                Node.Remove();

        var Nodes = Html.DocumentNode.SelectNodes("//div[@data-lyrics-container]");
        return $"[{Song.Title} - {Song.Artist} | {Song.Url} | SpotiDown]\n\n{WebUtility.HtmlDecode(string.Join("", Nodes.Select(Node => Node.InnerHtml)).Replace("<br>", "\n").Replace("<i>", "").Replace("</i>", "").Replace("<b>", "").Replace("</b>", ""))}";
    }

    public static double GetQuality(QualityType Quality)
    {
        switch (Quality)
        {
            default:
                return 160;
            case QualityType._32kbps:
                return 32;
            case QualityType._64kbps:
                return 64;
            case QualityType._128kbps:
                return 128;
            case QualityType._256kbps:
                return 256;
            case QualityType._320kbps:
                return 320;
        }
    }
    public static double GetQuality(int Quality) =>
        GetQuality((QualityType)Quality);

    public static string GetFormat(FormatType Format) =>
        Enum.GetName(Format) is string Result ? Result : "mp3";
    public static string GetFormat(int Format) =>
        GetFormat((FormatType)Format);
    public static string GetFormat(string FilePath) =>
        Path.GetExtension(FilePath) is string Result ? Result.Replace(".", "") : "mp3";

    public static string GetFilepath(Models.Song Song) =>
        Text.MakeSafe(Path.Combine(Local.Config.Paths.Download, Local.Config.Paths.FileName.Replace("{title}", Song.Title).Replace("{artist}", Song.Artist).Replace("{album}", Song.Album).Replace("{release}", Song.Release.Year.ToString())) + $".{GetFormat(Local.Config.YoutubePreferences.Format)}");

    public static async Task WriteStream(Models.Song Song, string Filepath, IProgress<double> Progress, CancellationToken CancellationToken)
    {
        Stream Stream = new MemoryStream();
        double Quality = 160;
        switch (Song.Type)
        {
            case SongType.Spotify:
                Quality = GetQuality(Local.Config.SpotifyPrefernces.Quality);
                throw new NotImplementedException();
            case SongType.YouTube:
                Quality = GetQuality(Local.Config.YoutubePreferences.Quality);
                Stream = await Youtube.GetStream(Song.Url, Quality, CancellationToken);
                break;
            case SongType.SoundCloud:
                Quality = GetQuality(Local.Config.SoundCloudPrefernces.Quality);
                throw new NotImplementedException();
        }

        if (Path.GetDirectoryName(Filepath) is string Director)
            Directory.CreateDirectory(Director);

        Progress.Report(0.05);

        Process FFMPEG = new Process()
        {
            StartInfo = new ProcessStartInfo()
            {
                FileName = "FFMPEG.exe",
                Arguments = $"-i - -v quiet -hide_banner -stats -y -b:a {Quality}k \"{Filepath}\"",
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardInput = true,
                RedirectStandardError = true
            }
        };
        FFMPEG.ErrorDataReceived += (s, e) =>
        {
            if (e.Data is string Data && Data.Contains("time="))
            {
                var Cur = TimeSpan.Parse(Data.Split("time=")[1].Split("bitrate=")[0].Trim());
                Progress.Report(Cur.TotalSeconds / Song.Duration.TotalSeconds);
            }
        };

        FFMPEG.Start();

        FFMPEG.BeginErrorReadLine();
        using (var Input = FFMPEG.StandardInput.BaseStream)
            await Stream.CopyToAsync(Input, CancellationToken);

        await FFMPEG.WaitForExitAsync(CancellationToken);
    }

    public static async Task WriteMeta(Models.Song Song, string Filepath)
    {
        string Format = GetFormat(Filepath);
        TagLib.File Meta = TagLib.File.Create(Filepath, $"audio/{Format}", TagLib.ReadStyle.None);

        Meta.Tag.Album = Song.Album;
        Meta.Tag.Comment = $"Song downloaded via SpotiDoen.\nSpotiDown is created by IcySnex (https://github.com/IcySnex/SpotiDown).\nUSING THIS TOOL IS AT YOUR OWN RISK!\n\n Full copyright for this song goes to the artist(s): '{Song.Artist}'.\nTrack meta data fetched from {Song.Type}, Lyrics fetched from genius.com.\nTrack Url: {Song.Url}";
        Meta.Tag.Copyright = Song.Type.ToString();
        Meta.Tag.TrackCount = (uint)Song.Total;
        Meta.Tag.Track = (uint)Song.Track;
        Meta.Tag.Lyrics = Song.Lyrics;
        Meta.Tag.Publisher = Song.Type.ToString();
        Meta.Tag.Title = Song.Title;
        Meta.Tag.Year = (uint)Song.Release.Year;

        if (Song.Artwork is string)
            if (Song.Artwork.StartsWith(":::"))
                Meta.Tag.Pictures = new[] { new TagLib.Picture(Song.Artwork.Substring(3)) };
            else
                Meta.Tag.Pictures = new[] { new TagLib.Picture(Local.MakeSquareImage(await Local.DownloadStream(Song.Artwork), 512)) };

        if (Song.Artist is string)
        {
            string[] Artists = Song.Artist.Split(',');

            Meta.Tag.AlbumArtists = new[] { Artists[0] };
            Meta.Tag.Composers = Artists;
            Meta.Tag.Performers = Artists;
        }

        Meta.Save();
    }

    public static Models.Song LoadFile(string Filepath)
    {
        
        FileInfo File = new(Filepath);
        string Format = string.IsNullOrWhiteSpace(File.Extension) ? "mp3" : File.Extension.Replace(".", "");
        TagLib.File Meta = TagLib.File.Create(Filepath, $"audio/{Format}", TagLib.ReadStyle.None);
        TimeSpan Duration;
        try{ Duration = TimeSpan.FromMilliseconds((double)Microsoft.WindowsAPICodePack.Shell.ShellFile.FromFilePath(Filepath).Properties.System.Media.Duration.Value! / 10000); } 
        catch { Duration = TimeSpan.Zero; }

        return new(
            SongType.Local,
            File.FullName,
            string.IsNullOrWhiteSpace(Meta.Tag.Title) ? File.Name : Meta.Tag.Title,
            string.Join(", ", Meta.Tag.Performers),
            Duration,
            Meta.Tag.Album,
            Meta.Tag.Year == 0 ? File.CreationTime : new((int)Meta.Tag.Year, 1, 1),
            Meta.Tag.Lyrics,
            Meta.Tag.Pictures.Length > 0 ? "?" : null,
            Meta.Tag.Pictures.Length > 0 ? Local.BytesToImage(Meta.Tag.Pictures[0].Data.Data) : null,
            (int)Meta.Tag.Track,
            (int)Meta.Tag.TrackCount);
    }
}