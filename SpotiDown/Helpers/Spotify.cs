using SpotiDown.Models;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Linq;
using System.Threading.Tasks;
using YoutubeExplode;
using YoutubeExplode.Common;
using YoutubeExplode.Videos;
using SpotiDown.Enums;
using YoutubeExplode.Playlists;
using YoutubeExplode.Channels;
using YoutubeExplode.Converter;
using SpotiDown.Controls;
using System.IO;
using YoutubeExplode.Videos.Streams;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using SpotifyAPI.Web;
using System.Text.RegularExpressions;

namespace SpotiDown.Helpers;

public class Spotify
{
    public Spotify() =>
        Initialize();

    public async void Initialize()
    {
        var Token = (await new OAuthClient().RequestToken(new ClientCredentialsRequest(Local.Config.Advanced.SpotifyAuth.Id, Local.Config.Advanced.SpotifyAuth.Secret))).AccessToken;
        Client = new(Token);
    }

    private static SpotifyClient? Client;

    public static SpotifySearchType GetSearchType(string Query)
    {
        if (Query.Contains("track", StringComparison.InvariantCultureIgnoreCase) && IsValidUrl(Query))
            return SpotifySearchType.Track;
        if (Query.Contains("playlist", StringComparison.InvariantCultureIgnoreCase) && IsValidUrl(Query))
            return SpotifySearchType.Playlist;
        if (Query.Contains("album", StringComparison.InvariantCultureIgnoreCase) && IsValidUrl(Query))
            return SpotifySearchType.Album;
        return SpotifySearchType.Query;
    }

    public static bool IsValidUrl(string Url) =>
        Regex.IsMatch(Url, @"http(s)?\:\/\/open\.spotify\.com\/(track|playlist|album)\/.+$");

    public static string GetIdByUrl(string Url) =>
        Url.Split('?')[0].Split('/').Last();

    public static async Task<IEnumerable<SpotifySong>> Search(string Query, SpotifySearchType Type, CancellationToken CancellationToken = default)
    {
        switch (Type)
        {
            case SpotifySearchType.Track:
                var Track = await Client!.Tracks.Get(Query);
                return new List<SpotifySong> { new(Track.Name, string.Join(", ", Track.Artists), Track.Album.Name, TimeSpan.FromMilliseconds(Track.DurationMs), Track.Album.Images.Count > 0 ? Track.Album.Images[0].Url : null) };
            case SpotifySearchType.Playlist:
                var Playlist = await Client!.Playlists.Get(Query);
                if (Playlist.Tracks is null || Playlist.Tracks.Total < 1)
                    return new List<SpotifySong>();
                return (await Client.PaginateAll(Playlist.Tracks)).Where(Track => Track.Track.Type == ItemType.Track).Select(Item => 
                {
                    var Track = (FullTrack)Item.Track;
                    return new SpotifySong(Track.Name, string.Join(", ", Track.Artists), Local.Config.Advanced.SavePlaylistAsAlbum ? Playlist.Name : Track.Album.Name, TimeSpan.FromMilliseconds(Track.DurationMs), Track.Album.Images.Count > 0 ? Track.Album.Images[0].Url : null);
                });
            case SpotifySearchType.Album:
                var Album = await Client!.Albums.Get(Query);
                return (await Client.PaginateAll(Album.Tracks)).Select(Track => new SpotifySong(Track.Name, string.Join(", ", Track.Artists), Album.Name, TimeSpan.FromMilliseconds(Track.DurationMs), Album.Images.Count > 0 ? Album.Images[0].Url : null));
            default:
                var SearchResult = (await Client!.Search.Item(new(SearchRequest.Types.Track, Query))).Tracks.Items;
                if (SearchResult is null || SearchResult.Count < 1)
                    return new List<SpotifySong>();
                return SearchResult.Select(Track => new SpotifySong(Track.Name, string.Join(", ", Track.Artists), Track.Album.Name, TimeSpan.FromMilliseconds(Track.DurationMs), Track.Album.Images.Count > 0 ? Track.Album.Images[0].Url : null));
        }
    }

    public static async Task<DownloadEntry> Convert(YoutubeSong Song, bool Lyrics, bool Artwork, CancellationToken CancellationToken = default)
    {
        Video Video = await Client.Videos.GetAsync(Song.Id, CancellationToken);

        return new(new(
            SongType.YouTube,
            $"https://www.youtube.com/watch?v={Song.Id}",
            Song.Title,
            Song.Channel,
            Song.Duration,
            Song.Playlist,
            Video.UploadDate.UtcDateTime,
            Lyrics ? Video.Description : null,
            Artwork ? Song.Thumbnail : null,
            null,
            0,
            0));
    }

    public static async Task<Stream> GetStream(string Input, double Quality, CancellationToken CancellationToken = default)
    {
        var Avaiable = (await Client.Videos.Streams.GetManifestAsync(Input, CancellationToken)).GetAudioOnlyStreams();
        var Info = Avaiable.First(n => Math.Abs(Quality - n.Bitrate.KiloBitsPerSecond) == Avaiable.Min(n => Math.Abs(Quality - n.Bitrate.KiloBitsPerSecond)));
        return await Client.Videos.Streams.GetAsync(Info, CancellationToken);
    }

    public static async Task Download(Models.Song Song, string Filepath, Progress<double> Progress, CancellationToken CancellationToken = default)
    {
        double Quality = Helpers.Song.GetQuality(Local.Config.YoutubePreferences.Quality);
        
        if (Path.GetDirectoryName(Filepath) is string Director)
            Directory.CreateDirectory(Director);

        var Avaiable = (await Client.Videos.Streams.GetManifestAsync(Song.Url, CancellationToken)).GetAudioOnlyStreams();
        var Info = new IStreamInfo[] { Avaiable.First(n => Math.Abs(Quality - n.Bitrate.KiloBitsPerSecond) == Avaiable.Min(n => Math.Abs(Quality - n.Bitrate.KiloBitsPerSecond))) };

        await Client.Videos.DownloadAsync(Info, new ConversionRequestBuilder(Filepath).SetFFmpegPath(Local.Config.Paths.FFMPEG).Build(), Progress, CancellationToken);
    }
}