using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;
using SpotiDown.Models;
using System;
using System.Drawing;
using System.Net.Http;
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
