using SpotifyAPI.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace SpotiDown.Helpers
{
    public class SpotifyHelper
    {
        public static SpotifyClient client;
        public static Exception NotValidSpotifyUrl = new Exception("Input Url is not a valid Spotify Url.");


        public static async Task<SpotifyClient> ConnectToSpotify() => new SpotifyClient((await new OAuthClient().RequestToken(new ClientCredentialsRequest(Helper.config.oauth.id, Helper.config.oauth.secret))).AccessToken);
        
        public static bool IsValidUrl(string url)
        {
            if (string.IsNullOrWhiteSpace(url)) return false;
            return Regex.IsMatch(url, @"http(s)?\:\/\/open\.spotify\.com\/(track|playlist|album)\/.+$");
        }

        public static SpotifyUrlType GetSpotifyUrlType(string url)
        {
            if (IsValidUrl(url) == false) { throw NotValidSpotifyUrl; }
            else if (url.ToLower().Contains("track")) { return SpotifyUrlType.Track; }
            else if (url.ToLower().Contains("playlist")) { return SpotifyUrlType.Playlist; }
            else if (url.ToLower().Contains("album")) { return SpotifyUrlType.Album; }
            else { throw NotValidSpotifyUrl; }
        }

        public static string GetIdByUrl(string url)
        {
            if (IsValidUrl(url) == false)
                throw NotValidSpotifyUrl;
            return url.Split('?')[0].Split('/').Last();
        }

        public static async Task<SpotifyTrack> GetSpotifytrackByFulltrack(FullTrack track)
        {
            return new SpotifyTrack()
            {
                title = Helper.MakeSafe(track.Name),
                artist = string.Join(", ", track.Artists.Select(a => Helper.MakeSafe(a.Name))),
                artwork = Helper.config.metadata.artwork ? track.Album.Images[0].Url : "",
                lenght = TimeSpan.FromMilliseconds(track.DurationMs),
                lyrics = Helper.config.metadata.lyrics ? await YoutubeHelper.GetLyrics($"{Helper.MakeUriSafe(track.Artists[0].Name)} {Helper.MakeUriSafe(track.Name)}") : "N/A",
                album = track.Album.Name,
                url = track.ExternalUrls["spotify"],
                youtube = await YoutubeHelper.GetId($"{track.Artists[0].Name} {track.Name}"),
                release = DateTime.Parse(track.Album.ReleaseDate),
                trackNumber = track.TrackNumber,
                discNumber = track.DiscNumber
            };
        }

        public static async Task<DownloadPageInfo> GetDownloadPageInfo(string url, IProgress<int> progress, CancellationToken cancel)
        {
            var type = GetSpotifyUrlType(url);
            var tracks = new List<SpotifyTrack>();
            if (type == SpotifyUrlType.Track)
            {
                var track = await client.Tracks.Get(GetIdByUrl(url));
                tracks.Add(await GetSpotifytrackByFulltrack(track));
                cancel.ThrowIfCancellationRequested();
                return new DownloadPageInfo() { name = track.Name, image = track.Album.Images[0].Url, tracks = tracks };
            }
            else if (type == SpotifyUrlType.Album)
            {
                var album = await client.Albums.Get(GetIdByUrl(url));
                var total = await client.PaginateAll(album.Tracks);
                foreach (var simpletrack in total)
                {
                    try
                    {
                        var track = await client.Tracks.Get(simpletrack.Id);
                        tracks.Add(await GetSpotifytrackByFulltrack(track));
                        progress.Report(total.IndexOf(simpletrack) / total.Count * 100);
                        cancel.ThrowIfCancellationRequested();
                    }
                    catch { }
                }

                return new DownloadPageInfo() { name = album.Name, image = album.Images[0].Url, tracks = tracks };
            }
            else if (type == SpotifyUrlType.Playlist)
            {
                var playlist = await client.Playlists.Get(GetIdByUrl(url));
                var total = await client.PaginateAll(playlist.Tracks);
                foreach (var playableitem in total)
                {
                    try
                    {
                        if (playableitem.Track.Type == ItemType.Track)
                        {
                            var track = await client.Tracks.Get(((FullTrack)playableitem.Track).Id);
                            tracks.Add(await GetSpotifytrackByFulltrack(track));
                            progress.Report(total.IndexOf(playableitem) / total.Count * 100);
                            cancel.ThrowIfCancellationRequested();
                        }
                    }
                    catch { }
                }

                return new DownloadPageInfo() { name = playlist.Name, image = playlist.Images[0].Url, tracks = tracks };
            }
            else
            {
                throw new Exception("Error", new Exception("Failed generating download page."));
            }
        }
    }

    public enum SpotifyUrlType { Track, Playlist, Album }

    public partial class SpotifyTrack
    {
        public string title { get; set; }
        public string artist { get; set; }
        public string artwork { get; set; }
        public TimeSpan lenght { get; set; }
        public string lyrics { get; set; }
        public string album { get; set; }
        public string url { get; set; }
        public string youtube { get; set; }
        public DateTime release { get; set; }
        public int trackNumber { get; set; }
        public int discNumber { get; set; }
    }
}
