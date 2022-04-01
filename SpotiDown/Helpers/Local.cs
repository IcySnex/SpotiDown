using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;
using SpotiDown.Models;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.System;

namespace SpotiDown.Helpers;

public class Local
{
    public static Config Config = Config.Load();
    public static HttpClient Client = new();

    public static void SetClipboard(string Text)
    {
        var dp = new DataPackage();
        dp.SetText(Text);
        Clipboard.SetContent(dp);
    }

    public static async Task<bool> AskUrl(XamlRoot Root, string Title, string Url)
    {
        if (await Window.Alert(Root, Title, $"Do you want to open the Url ({Url}) in the default browser?", "No", "Yes") == ContentDialogResult.Primary)
            return await Launcher.LaunchUriAsync(new Uri(Url));
        return false;
    }
    public static async Task<bool> OpenUrl(string Url) =>
        await Launcher.LaunchUriAsync(new Uri(Url));

    public static async Task<string> DownloadString(string Url) =>
        await Client.GetStringAsync(Url);

    public static async Task<Stream> DownloadStream(string Url) =>
        await Client.GetStreamAsync(Url);

    public static BitmapImage DownloadImage(string? Url) =>
        new(new Uri(string.IsNullOrWhiteSpace(Url) ? "ms-appx:///Assets/NoImage.png" : Url.StartsWith(":::") ? Url.Substring(3) : Url));

    public static async Task DownloadFile(string Url, string FileName, IProgress<double> Progress, CancellationToken CancellationToken)
    {
        using (HttpResponseMessage response = Client.GetAsync(Url, HttpCompletionOption.ResponseHeadersRead, CancellationToken).Result)
        using (Stream contentStream = await response.Content.ReadAsStreamAsync(CancellationToken), fileStream = new FileStream(FileName, FileMode.Create, FileAccess.Write, FileShare.None, 8192, true))
        {
            response.EnsureSuccessStatusCode();
            var totalRead = 0L;
            var buffer = new byte[8192];
            var isMoreToRead = true;

            do
            {
                var read = await contentStream.ReadAsync(buffer, 0, buffer.Length, CancellationToken);
                if (read == 0)
                    isMoreToRead = false;
                else
                {
                    await fileStream.WriteAsync(buffer, 0, read, CancellationToken);
                    totalRead += read;

                    Progress.Report(totalRead / (response.Content.Headers.ContentLength is long Lenght ? Lenght : 1.0));
                }
            }
            while (isMoreToRead);
        }
    }

    public static Bitmap MakeSquareImage(Bitmap Image, int Size)
    {
        Bitmap Result = new Bitmap(Size, Size);
        using (Graphics g = Graphics.FromImage(Result))
        {
            g.FillRectangle(new SolidBrush(Color.White), 0, 0, Size, Size);
            int t = 0, l = 0;
            if (Image.Height > Image.Width)
                t = (Image.Height - Image.Width) / 2;
            else
                l = (Image.Width - Image.Height) / 2;
            g.DrawImage(Image, new Rectangle(0, 0, Size, Size), new Rectangle(l, t, Image.Width - l * 2, Image.Height - t * 2), GraphicsUnit.Pixel);
        };
        return Result;
    }
    public static byte[] MakeSquareImage(Stream Image, int Size) =>
        new ImageConverter().ConvertTo(MakeSquareImage(new Bitmap(Image), Size), typeof(byte[])) is byte[] Result ? Result : Array.Empty<byte>();

    public static bool IsFileLocked(string Filepath)
    {
        try
        {
            FileStream fs = new(Filepath, FileMode.Open, FileAccess.Write);
            fs.Close();
            return false;
        }
        catch (IOException)
        {
            return true;
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async static Task DeleteFile(string Filepath)
    {
        if (IsFileLocked(Filepath))
            throw new("Could not delete file", new("File is locked"));

        using (var fw = new FileSystemWatcher(Path.GetDirectoryName(Filepath)!))
        {
            bool done = false;
            int timeout = 0;

            fw.EnableRaisingEvents = true;
            fw.Deleted += (object sender, FileSystemEventArgs e) => done = e.Name == Path.GetFileName(Filepath);

            File.Delete(Filepath);
            
            while (!done)
            {
                if (timeout >= 12)
                    throw new("Could not delete file", new("Timeout has been exceeded"));
                timeout += 1;
                await Task.Delay(1);
            }
        }
    }
}