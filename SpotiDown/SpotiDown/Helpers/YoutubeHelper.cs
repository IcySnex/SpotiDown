using HtmlAgilityPack;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using YoutubeExplode;

namespace SpotiDown.Helpers
{
    public class YoutubeHelper
    {
        public static YoutubeClient client = new YoutubeClient();

        public static async Task<string> GetId(string search) => Regex.Matches(await Helper.DownloadHtmlUTF8($"https://www.youtube.com/results?search_query={search}"), "v=([a-zA-Z0-9-_]{11})")[0].Groups[1].Value;

        public static async Task<string> GetLyrics(string search)
        {
            try
            {
                var url = JsonConvert.DeserializeObject<lyrics_search>(await Helper.DownloadHtmlUTF8($"https://api.genius.com/search?q={search}&access_token=mfXhXfTm2AuMSZPztd8a6eLAX-8nTdzGmMJd_2N3yjmJHeax3TGOqrTW4FLfqwq9")).response.hits[0].result.url;
                var html = new HtmlDocument(); html.LoadHtml((await Helper.DownloadHtmlUTF8(url)).Replace("https://ajax.googleapis.com/ajax/libs/jquery/2.1.4/jquery.min.js", ""));
                string lyrics;
                if (html.DocumentNode.Descendants("div").Any(d => d.Id == "lyrics-root-pin-spacer"))
                {
                    foreach (HtmlNode node in html.DocumentNode.SelectNodes("//br"))
                        node.ParentNode.ReplaceChild(html.CreateTextNode("\n"), node);
                    lyrics = string.Join("", html.GetElementbyId("lyrics-root-pin-spacer").ChildNodes[0].ChildNodes.Select(n => n.InnerText)).Replace("EmbedShare URLCopyEmbedCopy", "");
                }
                else
                {
                    lyrics = string.Join("", html.DocumentNode.SelectNodes("//div[@class='lyrics']//*").Select(n => n.InnerText)).Replace("EmbedShare URLCopyEmbedCopy", "");
                }

                return $"{url}\n\n{WebUtility.HtmlDecode(lyrics)}";
            }
            catch (Exception ex)
            {
                return $"Erorr fetching lyrics from 'genius.com'\nError message: *{ex.Message}*";
            }
        }

        public static async Task<Stream> DownloadAudioStream(string id)
        {
            var avaiavles = (await client.Videos.Streams.GetManifestAsync(id)).GetAudioOnlyStreams();
            var diffList = avaiavles.Select(x => new { n = x.Bitrate.KiloBitsPerSecond, diff = Math.Abs(x.Bitrate.KiloBitsPerSecond - GetKbpsConfig()) });
            var info = avaiavles.Where(b => b.Bitrate.KiloBitsPerSecond == diffList.Where(x => x.diff == diffList.Select(y => y.diff).Min()).First().n).First();
            return await client.Videos.Streams.GetAsync(info);
        }

        public static async Task<string> WriteTrack(SpotifyTrack trackinfo)
        {
            try {
                var filepath = Helper.config.prefernces.downloadpath + Helper.config.prefernces.filename.Replace("{title}", trackinfo.title).Replace("{artist}", trackinfo.artist.Split(',')[0]).Replace("{album}", trackinfo.album).Replace("{release}", trackinfo.release.Year.ToString());
                if (Helper.config.prefernces.format == 6)
                {
                    if (await Helper.ihs.writeStream(filepath + ".webm", await DownloadAudioStream(trackinfo.youtube)))
                    {
                        await Helper.ihs.writeMetadata(filepath + ".webm", trackinfo);
                        return $"Song sucessfully downloaded! ('{filepath}.webm')";
                    }
                }
                else
                {
                    if (await Helper.ihs.writeFfmpeg(filepath + GetFormatConfig(), await DownloadAudioStream(trackinfo.youtube)))
                    {
                        await Helper.ihs.writeMetadata(filepath + GetFormatConfig(), trackinfo);
                        return $"Song sucessfully downloaded! ('{filepath}{GetFormatConfig()}')";
                    }
                }
                return $"Song failed to download - '{trackinfo.title}'! ('Could not write file')";
            }
            catch (Exception ex) { return $"Song failed to download - '{trackinfo.title}'! ('{ex.Message}')"; }
        }

        public static int GetKbpsConfig()
        {
            switch (Helper.config.prefernces.quality)
            {
                case 0: return 32;
                case 1: return 64;
                case 2: return 128;
                case 3: return 160;
                case 4: return 256;
                case 5: return 320;
                default: return 160;
            }
        }
        public static string GetFormatConfig()
        {
            switch (Helper.config.prefernces.format)
            {
                case 0: return ".mp3";
                case 1: return ".wav";
                case 2: return ".m4a";
                case 3: return ".aac";
                case 4: return ".ogg";
                case 5: return ".flac";
                case 6: return ".webm";
                default: return ".mp3";
            }
        }
    }

    public partial class lyrics_search
    { public lyrics_search_response response { get; set; } = new lyrics_search_response(); }
    public partial class lyrics_search_response
    { public List<lyrics_search_response_hits> hits { get; set; } = new List<lyrics_search_response_hits>(); }
    public partial class lyrics_search_response_hits
    { public lyrics_search_response_hits_result result { get; set; } = new lyrics_search_response_hits_result(); }
    public partial class lyrics_search_response_hits_result
    { public string url { get; set; } }
}
