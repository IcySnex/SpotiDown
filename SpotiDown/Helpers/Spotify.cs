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
using System.Diagnostics;

namespace SpotiDown.Helpers;

public class Spotify
{
    public static async void Initialize()
    {
        var Token = (await new OAuthClient().RequestToken(new ClientCredentialsRequest(Local.Config.Advanced.Spotify.Id, Local.Config.Advanced.Spotify.Secret))).AccessToken;
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

    public static DateTime GetDate(string Input)
    {
        if (Input.Length == 4)
            return new(int.Parse(Input), 1, 1);
        return DateTime.Parse(Input);
    }

    public static async Task<IEnumerable<SpotifySong>> Search(string Query, SpotifySearchType Type, CancellationToken CancellationToken = default)
    {
        switch (Type)
        {
            case SpotifySearchType.Track:
                var Track = await Client!.Tracks.Get(GetIdByUrl(Query));
                return new List<SpotifySong> { new(Track.Name, string.Join(", ", Track.Artists.Select(Artist => Artist.Name)), Track.Album.Name, TimeSpan.FromMilliseconds(Track.DurationMs), GetDate(Track.Album.ReleaseDate), Track.Album.Images.Count > 0 ? Track.Album.Images[0].Url : null) };
            case SpotifySearchType.Playlist:
                var Playlist = await Client!.Playlists.Get(GetIdByUrl(Query));
                if (Playlist.Tracks is null || Playlist.Tracks.Total < 1)
                    return new List<SpotifySong>();
                return (await Client.PaginateAll(Playlist.Tracks)).Where(Track => Track.Track.Type == ItemType.Track).Select(Item => 
                {
                    var Track = (FullTrack)Item.Track;
                    return new SpotifySong(Track.Name, string.Join(", ", Track.Artists.Select(Artist => Artist.Name)), Local.Config.Advanced.Spotify.SavePlaylistAsAlbum ? Playlist.Name : Track.Album.Name, TimeSpan.FromMilliseconds(Track.DurationMs), GetDate(Track.Album.ReleaseDate), Track.Album.Images.Count > 0 ? Track.Album.Images[0].Url : null);
                });
            case SpotifySearchType.Album:
                var Album = await Client!.Albums.Get(GetIdByUrl(Query));
                return (await Client.PaginateAll(Album.Tracks)).Select(Track => new SpotifySong(Track.Name, string.Join(", ", Track.Artists.Select(Artist => Artist.Name)), Album.Name, TimeSpan.FromMilliseconds(Track.DurationMs), GetDate(Album.ReleaseDate), Album.Images.Count > 0 ? Album.Images[0].Url : null));
            default:
                var SearchResult = (await Client!.Search.Item(new(SearchRequest.Types.Track, Query) { Market = Local.Config.Advanced.Spotify.SearchMarketCode })).Tracks.Items;
                if (SearchResult is null || SearchResult.Count < 1)
                    return new List<SpotifySong>();
                return SearchResult.Select(Track => new SpotifySong(Track.Name, string.Join(", ", Track.Artists.Select(Artist => Artist.Name)), Track.Album.Name, TimeSpan.FromMilliseconds(Track.DurationMs), GetDate(Track.Album.ReleaseDate), Track.Album.Images.Count > 0 ? Track.Album.Images[0].Url : null));
        }
    }

    public static async Task<DownloadEntry> Convert(SpotifySong Song, bool Lyrics, bool Artwork, CancellationToken CancellationToken = default)
    {
        string Query = Local.Config.Advanced.Spotify.YoutubeSearchAlgorithm.Replace("{title}", Song.Title).Replace("{artist}", Song.Artist).Replace("{album}", Song.Album);
        var Video = (await Youtube.Client.Search.GetVideosAsync(Query, CancellationToken).CollectAsync(1)).First();

        return new(new(
            SongType.Spotify,
            $"https://www.youtube.com/watch?v={Video.Id}",
            Song.Title,
            Song.Artist,
            Video.Duration!.Value,
            Song.Album,
            Song.Release,
            Lyrics ? await Helpers.Song.SearchLyrics(Song.Title, Song.Artist) is List<Genius_Response_Hit> res && res.Count > 0 ? await Helpers.Song.GetLyrics(res.First().result) : null : null,
            Artwork ? Song.Artwork : null,
            null,
            0,
            0));
    }
}