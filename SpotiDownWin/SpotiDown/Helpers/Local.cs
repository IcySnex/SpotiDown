using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;
using SpotiDown.Models;
using System;
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

    public static async Task<string> DownloadString(string Url) =>
        await Client.GetStringAsync(Url);

    public static BitmapImage DownloadImage(string? Url) =>
        new(new Uri(Url is null ? "ms-appx:///Assets/NoImage.png" : Url));

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

    public Bitmap MakeSquarePhoto(Bitmap bmp, int size)
    {
        Bitmap res = new Bitmap(size, size);
        using (Graphics g = Graphics.FromImage(res))
        {
            g.FillRectangle(new SolidBrush(Color.White), 0, 0, size, size);
            int t = 0, l = 0;
            if (bmp.Height > bmp.Width)
                t = (bmp.Height - bmp.Width) / 2;
            else
                l = (bmp.Width - bmp.Height) / 2;
            g.DrawImage(bmp, new Rectangle(0, 0, size, size), new Rectangle(l, t, bmp.Width - l * 2, bmp.Height - t * 2), GraphicsUnit.Pixel);
        };
        return res;
    }
}