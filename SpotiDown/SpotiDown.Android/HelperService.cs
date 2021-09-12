using SpotiDownVB;
using System.IO;
using XamarinFirebase.Droid;
using TagLib.Id3v2;
using System;
using System.Threading.Tasks;
using System.Net;
using SpotiDown;
using Xabe.FFmpeg;
using Xabe.FFmpeg.Downloader;

[assembly: Xamarin.Forms.Dependency(typeof(HelperService))]
namespace XamarinFirebase.Droid
{
    public class HelperService : IHelperService
    {
        [Obsolete]
        async Task IHelperService.wirteStream(string path, Stream stream)
        {
            string ffmpeg = @"/storage/emulated/0/ffmpeg";
            string fullpath = $@"/storage/emulated/0{path}.mp3";
            string temppath = $@"/storage/emulated/0{path}.webm";
            if (!Directory.Exists(Path.GetDirectoryName(fullpath))) { Directory.CreateDirectory(Path.GetDirectoryName(fullpath)); }
            using (var fileStream = new FileStream(temppath, FileMode.Create, FileAccess.Write))
            {
                await stream.CopyToAsync(fileStream);
            }

            FFmpeg.SetExecutablesPath(ffmpeg);
            await FFmpegDownloader.GetLatestVersion(FFmpegVersion.Android, ffmpeg);
            await FFmpeg.Conversions.New().Start($"-i \"{temppath}\" -vn \"{fullpath}\"");

            await App.Current.MainPage.DisplayAlert("Converted!", "webm to mp3", "OK");
        }

        async Task IHelperService.writeMetadata(SpotifyTrack trackinfo, string path)
        {
            Tag.DefaultVersion = 3;
            Tag.ForceDefaultVersion = true;
            TagLib.File file = TagLib.File.Create($@"/storage/emulated/0{path}.mp3");
            file.Tag.AlbumArtists = new[] { trackinfo.artist.Split(",")[0] };
            file.Tag.Performers = trackinfo.artist.Split(",");
            file.Tag.Composers = trackinfo.artist.Split(",");
            file.Tag.Copyright = "Spotify";
            file.Tag.Lyrics = trackinfo.lyrics;
            file.Tag.Title = trackinfo.title;
            file.Tag.Genres = new[] { "Music" };
            file.Tag.Album = trackinfo.album;
            file.Tag.Track = Convert.ToUInt32(trackinfo.trackNumber);
            file.Tag.Disc = Convert.ToUInt32(trackinfo.discNumber);
            file.Tag.DiscCount = Convert.ToUInt32(trackinfo.discNumber);
            file.Tag.Year = Convert.ToUInt32(trackinfo.release.Year);
            file.Tag.Comment = $@"Song downloaded via SpotiDown.
SpotiDown is created by IcySnex (https://github.com/IcySnex/SpotiDown).
Using this tool is at your own risk!

Full Copyright for this song goes to the artist(s): {trackinfo.artist}.
Track meta data fetched from Spotify, Track downloaded from YouTube, Lyrics fetched from genius.com!

Spotify URL: {trackinfo.url}
YouTube URL: https://www.youtube.com/watch?v={trackinfo.youtube}";
            file.Tag.Pictures = new[] { new TagLib.Picture(await new WebClient().DownloadDataTaskAsync(trackinfo.artwork)) };
            file.Save();
            await App.Current.MainPage.DisplayAlert("Song sucessfully downloaded!", $@"/storage/emulated/0{path}", file.Writeable.ToString());
        }

        async Task IHelperService.downloadFile(string url, string path) => await new WebClient().DownloadFileTaskAsync(url, Helper.GetPath(path));
    }
}

