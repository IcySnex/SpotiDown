using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using SpotiDown.Helpers;
using Xamarin.Essentials;
using System.IO;

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
                Button btn = (Button)((StackLayout)((Frame)((StackLayout)((StackLayout)Parent).Parent).Children[1]).Content).Children[0];
                try
                {
                    btn_download.IsEnabled = false;
                    btn.IsEnabled = false;
                    await App.Current.MainPage.DisplayAlert("Done!", $"Song sucessfully downloaded! ('{await YoutubeHelper.WriteTrack(trackinfo)}')", "OK");
                    btn.IsEnabled = true;
                    StackLayout container = (StackLayout)Parent;
                    container.Children.Remove(this);
                }
                catch (Exception ex)
                {
                    await App.Current.MainPage.DisplayAlert("Something went wrong - Error", ex.Message, "OK");
                }
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
    }
}