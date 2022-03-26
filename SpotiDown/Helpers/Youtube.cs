﻿using SpotiDown.Models;
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

namespace SpotiDown.Helpers;

public class Youtube
{
    private static YoutubeClient Client = new();

    public static YoutubeSearchType GetSearchType(string Query)
    {
        if (VideoId.TryParse(Query) is VideoId)
            return YoutubeSearchType.Video;
        if (PlaylistId.TryParse(Query) is PlaylistId)
            return YoutubeSearchType.Playlist;
        if (ChannelId.TryParse(Query) is ChannelId)
            return YoutubeSearchType.Channel;
        return YoutubeSearchType.Query;
    }

    public static async Task<IEnumerable<YoutubeSong>> Search(string Query, YoutubeSearchType Type, int ResultCount = 50, CancellationToken CancellationToken = default)
    {
        switch (Type)
        {
            case YoutubeSearchType.Video:
                var Result = await Client.Videos.GetAsync(Query, CancellationToken);
                return new List<YoutubeSong> { new(Result.Id, Result.Title, Result.Author.Title, null, Result.Duration is TimeSpan d ? d : new(0), Result.Thumbnails.IndexIfExists(1)) };
            case YoutubeSearchType.Playlist:
                string PlaylistName = (await Client.Playlists.GetAsync(Query, CancellationToken)).Title;
                return (await Client.Playlists.GetVideosAsync(Query, CancellationToken).CollectAsync(ResultCount)).Select(Video => new YoutubeSong(Video.Id, Video.Title, Video.Author.Title, PlaylistName, Video.Duration is TimeSpan d ? d : new(0), Video.Thumbnails.IndexIfExists(1)));
            case YoutubeSearchType.Channel:
                return (await Client.Channels.GetUploadsAsync(Query, CancellationToken).CollectAsync(ResultCount)).Select(Video => new YoutubeSong(Video.Id, Video.Title, Video.Author.Title, Video.Author.Title, Video.Duration is TimeSpan d ? d : new(0), Video.Thumbnails.IndexIfExists(1)));
            default:
                return (await Client.Search.GetVideosAsync(Query, CancellationToken).CollectAsync(ResultCount)).Select(Video => new YoutubeSong(Video.Id, Video.Title, Video.Author.Title, null, Video.Duration is TimeSpan d ? d : new(0), Video.Thumbnails.IndexIfExists(1)));
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

        //META TAGS
    }
}