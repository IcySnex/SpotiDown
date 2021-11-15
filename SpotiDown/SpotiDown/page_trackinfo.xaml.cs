using SpotiDown.Helpers;
using System;
using System.IO;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SpotiDown
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class page_trackinfo : ContentPage
    {
        public cw_track track_sender;
        public page_trackinfo(SpotifyTrack trackinfo, cw_track sender)
        {
            InitializeComponent();
            track_sender = sender;
            if (Helper.config.metadata.artwork) { img_image.Source = ImageSource.FromUri(new Uri(trackinfo.artwork)); }
            lb_name.Text = trackinfo.title;
            lb_title.Text = trackinfo.title;
            lb_artist.Text = trackinfo.artist;
            lb_lenght.Text = Helper.FormatTimespan(trackinfo.lenght);
            lb_album.Text = trackinfo.album;
            lb_release.Text = trackinfo.release.ToString("MM/dd/yyyy");
            lb_url.Text = trackinfo.url;
            lb_youtube.Text = $"https://www.youtube.com/watch?v={trackinfo.youtube}";
            edi_lyrics.Text = trackinfo.lyrics;
            edi_lyrics.TextChanged += edi_lyrics_TextChanged;
        }


        private async void btn_back_click(object sender, EventArgs e) { try { await Navigation.PopModalAsync(true); } catch { } }
        private async void btn_download_click(object sender, EventArgs e)
        {
            if (await Permissions.CheckStatusAsync<Permissions.StorageWrite>() == PermissionStatus.Granted)
            {
                try
                {
                    Button btn = (Button)((StackLayout)((Frame)((StackLayout)((StackLayout)track_sender.Parent).Parent).Children[1]).Content).Children[0];
                    if (Helper.config.prefernces.format != 6 && !File.Exists(Helper.GetPath("ffmpeg")))
                    {
                        string arch = Helper.ihs.getArch();
                        if (!await DisplayAlert("Something went wrong - Error", $"FFMPEG is not installed. Do you want to download the binary ({arch})?", "YES", "NO")) return;
                        await Helper.ihs.downloadFile($"https://github.com/IcySnex/SpotiDown/raw/main/other/ffmpeg-{arch}", "ffmpeg");
                        await DisplayAlert("Finished FFMPEG Download", $"Successfully downloaded ffmpeg binary ({arch}). Processing command now...", "OK");
                    }
                    btn_download.IsEnabled = false;
                    track_sender.btn_download.IsEnabled = false;
                    btn.IsEnabled = false;
                    await Downloadtrack(track_sender.trackinfo);
                    btn.IsEnabled = true;
                    btn_remove_click(sender, e);
                }
                catch (Exception ex) { await DisplayAlert("Something went wrong - Error", ex.Message, "OK"); }
            }
            else
            {
                if (await DisplayAlert("Something went wrong - Error", "Missing permissions for downloading track to external storage. Please grant SpotiDown permissions for 'Storage'!", "OK", "CANCEL"))
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
        private async void btn_remove_click(object sender, EventArgs e) { ((StackLayout)track_sender.Parent).Children.Remove(track_sender); btn_back_click(sender,e); }
        private void edi_lyrics_TextChanged(object sender, TextChangedEventArgs e) => track_sender.trackinfo.lyrics = edi_lyrics.Text;
    }
}