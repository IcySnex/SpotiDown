using Microsoft.AspNetCore.Mvc;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using YoutubeExplode;

namespace SpotiDown.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DownloadController : ControllerBase
    {
        [HttpGet]
        public async Task<object> Get(string id, int kbps)
        {
            if (string.IsNullOrWhiteSpace(id)) return "Failed: [ No Youtube Video ID is given! ]"; if (kbps < 1) kbps = 160;
            Stream s = ConvertToMp3(await DownloadAudioStream(id, kbps), kbps);
            return new FileStreamResult(s, "audio/mpeg") { FileDownloadName = $"{id}.mp3" };
        }

        public static YoutubeClient client = new YoutubeClient();
        public static async Task<Stream> DownloadAudioStream(string id, int kbps)
        {
            var info = (await client.Videos.Streams.GetManifestAsync(id)).GetAudioOnlyStreams().First();
            return await client.Videos.Streams.GetAsync(info);
        }
        public static Stream ConvertToMp3(Stream webm, int kbps)
        {
            Stream opt = new MemoryStream();
            Process ffmpeg = new Process()
            {
                StartInfo = new ProcessStartInfo()
                {
                    FileName = @$"{System.Reflection.Assembly.GetExecutingAssembly().Location.Replace("SpotiDown.API.dll", "")}ffmpeg",
                    Arguments = $"-i - -f mp3 -b:a {kbps}k -",
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardError = true,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true
                },
                EnableRaisingEvents = true
            };
            ffmpeg.Start();
            ffmpeg.BeginErrorReadLine();
            var inputTask = Task.Run(() =>
            { webm.CopyTo(ffmpeg.StandardInput.BaseStream); ffmpeg.StandardInput.Close(); });
            var outputTask = Task.Run(() =>
            { ffmpeg.StandardOutput.BaseStream.CopyTo(opt); });
            Task.WaitAll(inputTask, outputTask);
            ffmpeg.WaitForExit();
            opt.Position = 0;
            return opt;
        }
    }
}
