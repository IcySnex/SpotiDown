using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using SpotiDown.Helpers;
using Xamarin.Essentials;
using System.IO;
using System.Threading.Tasks;

namespace SpotiDown
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class cw_track : ContentView
    {
        public cw_track(SpotifyTrack track)
        {
            InitializeComponent();
            trackinfo = track;
            updatedui();
        }

        public SpotifyTrack trackinfo;
        public void updatedui()
        {
            lb_title.Text = trackinfo.title;
            lb_artist.Text = trackinfo.artist;
            if (Helper.config.metadata.artwork) { img_artwork.Source = ImageSource.FromUri(new Uri(trackinfo.artwork)); }
            lb_lenght.Text = Helper.FormatTimespan(trackinfo.lenght);
        }

        private async void btn_info_click(object sender, EventArgs e) => await Navigation.PushModalAsync(new page_trackinfo(trackinfo, this));
        private async void btn_download_click(object sender, EventArgs e)
        {
            if (await Permissions.CheckStatusAsync<Permissions.StorageWrite>() == PermissionStatus.Granted)
            {
                try
                {
                    Button btn = (Button)((StackLayout)((Frame)((StackLayout)((StackLayout)Parent).Parent).Children[1]).Content).Children[0];
                    if (Helper.config.prefernces.format != 6 && !File.Exists(Helper.GetPath("ffmpeg")))
                    {
                        string arch = Helper.ihs.getArch();
                        if (!await App.Current.MainPage.DisplayAlert("Something went wrong - Error", $"FFMPEG is not installed. Do you want to download the binary ({arch})?", "YES", "NO")) return;
                        await Helper.ihs.downloadFile($"https://github.com/IcySnex/SpotiDown/raw/main/other/ffmpeg-{arch}", "ffmpeg");
                        await App.Current.MainPage.DisplayAlert("Finished FFMPEG Download", $"Successfully downloaded ffmpeg binary ({arch}). Processing command now...", "OK");
                    }
                    btn_download.IsEnabled = false;
                    btn.IsEnabled = false;
                    await Downloadtrack(trackinfo);
                    btn.IsEnabled = true;
                    StackLayout container = (StackLayout)Parent;
                    container.Children.Remove(this);
                }
                catch (Exception ex) { await App.Current.MainPage.DisplayAlert("Something went wrong - Error", ex.Message, "OK"); }
            }
            else
            {
                if (await App.Current.MainPage.DisplayAlert("Something went wrong - Error", "Missing permissions for downloading track to external storage. Please grant SpotiDown permissions for 'Storage'!", "OK", "CANCEL"))
                {
                    await Permissions.RequestAsync<Permissions.StorageWrite>();
                    btn_download_click(sender, e);
                }
            }
        }

        private async Task<bool> Downloadtrack(SpotifyTrack trackinfo)
        {
            try
            {
                string res = await YoutubeHelper.WriteTrack(trackinfo);
                if (res.Contains("Song failed to download"))
                    if (await App.Current.MainPage.DisplayAlert("Something went wrong - Error", res + " Do you want to retry?", "YES", "NO")) return await Downloadtrack(trackinfo); else return false;
                await App.Current.MainPage.DisplayAlert("Done!", res, "OK");
                return true;
            }
            catch (Exception ex) { if (await App.Current.MainPage.DisplayAlert("Something went wrong - Error", $"Song failed to download! ('{ex.Message}') Do you want to retry?", "YES", "NO")) return await Downloadtrack(trackinfo); else return false; }
        }
    }
}