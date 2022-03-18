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
    public async Task<List<Genius_Response_Hit>> SearchLyrics(string Title, string Artist) =>
        Text.Deserialize<Genius>(await Local.DownloadString($"https://api.genius.com/search?q={Title} {Artist}&access_token=7lBNcWe_5ljK10TnPh7q3j31clowVoGLN9IEYFbO9AcKsngZ49Iz7aeZ0D9W4H1j")) is Genius Search ? Search.response.hits : new();
    
    public async Task<string> GetLyrics(Genius_Response_Hit_Result Song)
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
        return WebUtility.HtmlDecode(string.Join("", Nodes.Select(Node => Node.InnerHtml)).Replace("<br>", "\n"));
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
        Enum.GetName(Format) is string Result ? "." + Result : ".mp3";
    public static string GetFormat(int Format) =>
        GetFormat((FormatType)Format);

    public static async Task Write(Models.Song Song, IProgress<long> Progress, CancellationToken CancellationToken)
    {
        string FilePath = "";
        double Quality = 160;
        long Size = 1;
        Stream Stream = new MemoryStream();

        switch (Song.Type)
        {
            case SongType.Spotify:
                Quality = GetQuality(Local.Config.SpotifyPrefernces.Quality);
                throw new NotImplementedException();
            case SongType.YouTube:
                FilePath = Text.MakeSafe(Path.Combine(Local.Config.Paths.Download, Local.Config.Paths.FileName.Replace("{title}", Song.Title).Replace("{artist}", Song.Artist).Replace("{album}", Song.Album).Replace("{release}", Song.Release.Year.ToString())) + GetFormat(Local.Config.YoutubePreferences.Format));
                Quality = GetQuality(Local.Config.YoutubePreferences.Quality);
                Stream = await Youtube.GetStream(Song.Url, Quality, CancellationToken);
                break;
            case SongType.SoundCloud:
                Quality = GetQuality(Local.Config.SoundCloudPrefernces.Quality);
                throw new NotImplementedException();
        }
        Size = Stream.Length / 10000;
        if (Path.GetDirectoryName(FilePath) is string Director)
            Directory.CreateDirectory(Director);
        Progress.Report(5);

        Process FFMPEG = new Process()
        {
            StartInfo = new ProcessStartInfo()
            {
                FileName = "FFMPEG.exe",
                Arguments = $"-i - -v quiet -hide_banner -stats -y -b:a {Quality}k \"{FilePath}\"",
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardInput = true,
                RedirectStandardError = true
            }
        };
        FFMPEG.ErrorDataReceived += (s, e) =>
        {
            if (e.Data is string Data && Data.Contains("size="))
            {
                int Cur = Convert.ToInt32(Data.Split('t')[0].Replace("size=", "").Replace("kB", " ").Trim());
                Progress.Report(Cur / Size);
            }
        };

        FFMPEG.Start();

        FFMPEG.BeginErrorReadLine();
        using (var Input = FFMPEG.StandardInput.BaseStream)
            await Stream.CopyToAsync(Input, CancellationToken);

        await FFMPEG.WaitForExitAsync(CancellationToken);
    }
}