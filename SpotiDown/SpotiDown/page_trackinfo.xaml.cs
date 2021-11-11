using SpotiDown.Helpers;
using System;
using System.IO;
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
                    btn_download.IsEnabled = false;
                    await DisplayAlert("Done!", $"Song sucessfully downloaded! ('{await YoutubeHelper.WriteTrack(track_sender.trackinfo)}')", "OK");
                    btn_remove_click(sender, e);
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Something went wrong - Error", ex.Message, "OK");
                }
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
        private async void btn_remove_click(object sender, EventArgs e) { ((StackLayout)track_sender.Parent).Children.Remove(track_sender); btn_back_click(sender,e); }
        private void edi_lyrics_TextChanged(object sender, TextChangedEventArgs e) => track_sender.trackinfo.lyrics = edi_lyrics.Text;
    }
}