using SpotiDown.Helpers;
using System.IO;
using XamarinFirebase.Droid;
using TagLib.Id3v2;
using System;
using System.Threading.Tasks;
using System.Net;
using System.Linq;
using System.Diagnostics;
using System.Text;

[assembly: Xamarin.Forms.Dependency(typeof(HelperService))]
namespace XamarinFirebase.Droid
{
    public class HelperService : IHelperService
    {
        [Obsolete]
        async Task<bool> IHelperService.writeStream(string path, Stream stream)
        {
            string temppath = $@"/storage/emulated/0{path}";
            if (!Directory.Exists(Path.GetDirectoryName(temppath))) { Directory.CreateDirectory(Path.GetDirectoryName(temppath)); }
            using (var fileStream = new FileStream(temppath, FileMode.Create, FileAccess.Write))
                await stream.CopyToAsync(fileStream);
            return true;
        }

        async Task IHelperService.writeMetadata(string path, SpotifyTrack trackinfo)
        {
            Tag.DefaultVersion = 3;
            Tag.ForceDefaultVersion = true;
            TagLib.File file = TagLib.File.Create($@"/storage/emulated/0{path}", path.EndsWith("webm") ? "video/webm" : $"audio/{Path.GetExtension(path).Replace(".", "")}", TagLib.ReadStyle.None);
            file.Tag.AlbumArtists = new[] { trackinfo.artist.Split(",")[0] };
            file.Tag.Performers = trackinfo.artist.Split(",");
            file.Tag.Composers = trackinfo.artist.Split(",");
            file.Tag.Copyright = "Spotify";
            if (Helper.config.metadata.lyrics) { file.Tag.Lyrics = trackinfo.lyrics; }
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
            if (Helper.config.metadata.artwork) { file.Tag.Pictures = new[] { new TagLib.Picture(await new WebClient().DownloadDataTaskAsync(trackinfo.artwork)) }; }
            file.Save();
        }

        async Task IHelperService.downloadFile(string url, string path) => await new WebClient().DownloadFileTaskAsync(url, Helper.GetPath(path));

        string IHelperService.getArch() => Android.OS.Build.SupportedAbis[0].Substring(0, 3);

        async Task<bool> IHelperService.writeFfmpeg(string path, Stream stream)
        {
            string temppath = $@"/storage/emulated/0{path}";
            if (!Directory.Exists(Path.GetDirectoryName(temppath))) { Directory.CreateDirectory(Path.GetDirectoryName(temppath)); }
            await Helper.ihs.chmod(Helper.GetPath("ffmpeg"));
            Java.Lang.Process proc = Java.Lang.Runtime.GetRuntime().Exec(new[] { Helper.GetPath("ffmpeg") }.Concat(new[] { "-i", $"-", "-b:a", $"{YoutubeHelper.GetKbpsConfig()}k", temppath }).ToArray());
            using (var OutputStream = proc.OutputStream)
                await stream.CopyToAsync(OutputStream);
            return await proc.WaitForAsync() == 0 ? true : false;
        }

        async Task<bool> IHelperService.chmod(string path)
        {
            Java.Lang.Process chmod = Java.Lang.Runtime.GetRuntime().Exec(new[] { "chmod", "777", path });
            if (await chmod.WaitForAsync() == 0) return true; else throw new Exception(new StreamReader(chmod.ErrorStream).ReadToEnd());
        }

        async Task<string> IHelperService.runFFMPEG(string command)
        {
            await Helper.ihs.chmod(Helper.GetPath("ffmpeg"));
            Java.Lang.Process proc = Java.Lang.Runtime.GetRuntime().Exec(new[] { Helper.GetPath("ffmpeg") }.Concat(command.Split(' ')).ToArray());

            int res = await proc.WaitForAsync();
            string ou = await new StreamReader(proc.InputStream).ReadToEndAsync();
            string error = await new StreamReader(proc.ErrorStream).ReadToEndAsync();
            return string.IsNullOrWhiteSpace(ou) ? string.IsNullOrWhiteSpace(error) ? res == 0 ? "PROCESS FINISHED SUCESSFULLY: NO DATA RETURNED!" : "PROCESS FAILED: NO DATA RETURNED!" : error : ou;
        }
    }
}

