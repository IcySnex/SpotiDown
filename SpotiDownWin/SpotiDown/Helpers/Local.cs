using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using SpotiDown.Models;
using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.System;

namespace SpotiDown.Helpers;

public class Local
{
    public static Config Config = Config.Load();

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
}
