using System;
using System.Diagnostics;
using System.IO;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SpotiDown.Helpers
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class tab_ffmpeg : ContentPage
    {
        public tab_ffmpeg() => InitializeComponent();

        private async void btn_cmdrun(object sender, EventArgs e)
        {
            if (await Permissions.CheckStatusAsync<Permissions.StorageWrite>() != PermissionStatus.Granted)
            {
                if (!await DisplayAlert("Something went wrong - Error", "Missing permissions for downloading track to external storage. Please grant SpotiDown permissions for 'Storage'!", "OK", "CANCEL")) return;
                await Permissions.RequestAsync<Permissions.StorageWrite>();
                btn_cmdrun(sender, e);
            }
            if (!File.Exists(Helper.GetPath("ffmpeg")))
            {
                string arch = Helper.ihs.getArch();
                if (!await DisplayAlert("Something went wrong - Error", $"FFMPEG is not installed. Do you want to download the binary ({arch})?", "YES", "NO")) return;
                await Helper.ihs.downloadFile($"https://github.com/IcySnex/SpotiDown/raw/main/other/ffmpeg-{arch}", "ffmpeg");
                await DisplayAlert("Finished FFMPEG Download", $"Successfully downloaded ffmpeg binary ({arch}). Processing command now...", "OK");
            }
            if (string.IsNullOrWhiteSpace(edi_cmd.Text)) { await DisplayAlert("Something went wrong - Error", "Command cannot be empty!", "OK"); return; }

            edi_result.Text = "Startet processing...";
            edi_result.Text = await Helper.ihs.runFFMPEG(edi_cmd.Text);
            edi_result.Focus();
        }
    }
}