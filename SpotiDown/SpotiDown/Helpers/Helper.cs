using System;
using Newtonsoft.Json;
using System.IO;
using Xamarin.Forms;
using System.Text;
using System.Text.RegularExpressions;
using System.Net;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Diagnostics;

namespace SpotiDown.Helpers
{
    public class Helper
    {
        public static config config { get; set; } = File.Exists(GetPath("config.json")) ? JsonConvert.DeserializeObject<config>(File.ReadAllText(GetPath("config.json"))) : new config();
        public static IHelperService ihs = DependencyService.Get<IHelperService>();

        public static void SaveConfig() => File.WriteAllText(GetPath("config.json"), JsonConvert.SerializeObject(config));

        public static string GetPath(string file) => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), file);

        public static string RandomString(int min, int max, string charset = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789")
        {
            Random r = new Random(); StringBuilder sb = new StringBuilder(); int l = r.Next(min, max);
            for (int i = 1, loopTo = l; i <= loopTo; i++) sb.Append(charset.Substring(r.Next(0, charset.Length), 1));
            return sb.ToString();
        }

        public static string MakeSafe(string input) => Regex.Replace(input, @"[\/\\\?\*\<\>\|\:\""]", "#");
        public static string MakeUriSafe(string input) => Regex.Replace(input, @"[\s+\&]", "%20");
        public static string MakeFileSafe(string input) => Regex.Replace(input.Trim().EndsWith("/") ? input.Trim().Remove(input.Trim().Length - 1) : input.Trim(), @"[\\\?\*\<\>\|\:\""]", "#");
        public static string MakePathSafe(string input)
        {
            var s = MakeFileSafe(input).Trim();
            if (s.StartsWith("/") == false)
                s = $"/{s}";
            if (s.EndsWith("/"))
                s = s.Remove(s.Length - 1);
            return s;
        }

        public static string FormatTimespan(TimeSpan timespan)
        {
            if (timespan.Hours > 0) { return $"{timespan.Hours}h, {timespan.Minutes}m, {timespan.Seconds}s"; }
            else if (timespan.Minutes > 0) { return $"{timespan.Minutes}m, {timespan.Seconds}s"; }
            else { return $"{timespan.Seconds}s"; }
        }

        public static SpotifyTrack CreateSpotifyTrack(string title_, string artist_, string artwork_, TimeSpan lenght_) => new SpotifyTrack() { title = title_, artist = artist_, artwork = artwork_, lenght = lenght_ };
        public static async Task<string> DownloadHtmlUTF8(string url) => await new WebClient() { Encoding = Encoding.UTF8 }.DownloadStringTaskAsync(url);

        public static async Task<bool> IsValidWebAPI(string url) => await new WebClient().DownloadStringTaskAsync(url) == "{SpotiDown Valid Web API}" ? true : false;

       
    }

    public partial interface IHelperService
    {
        Task<bool> writeStream(string path, Stream stream);
        Task writeMetadata(string path, SpotifyTrack trackinfo);
        Task downloadFile(string url, string path);
        string getArch();
        Task<bool> writeFfmpeg(string path, Stream stream);
        Task<bool> chmod(string path);
        Task<string> runFFMPEG(string command);
    }

    public partial class DownloadPageInfo
    {
        public string name { get; set; }
        public string image { get; set; }
        public List<SpotifyTrack> tracks { get; set; } = new List<SpotifyTrack>();
    }
}
