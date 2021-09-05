using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using SpotiDownVB;
using System;
using System.Threading.Tasks;

namespace SpotiDown
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class page_download : ContentPage
    {
        public page_download(DownloadPageInfo info)
        {
            InitializeComponent();
            lb_name.Text = info.name;
            img_image.Source = ImageSource.FromUri(new Uri(info.image));
            img_background.Source = ImageSource.FromUri(new Uri(info.image));
            foreach (SpotifyTrack track in info.tracks) { sl_trackcontainer.Children.Add(new cw_track(track)); }
        }

        private async void btn_back_click(object sender, EventArgs e) { try { await Navigation.PopModalAsync(true); } catch { } }

        private void btn_download_click(object sender, EventArgs e)
        {
            DisplayAlert("d", sl_trackcontainer.Children.Count.ToString(), "OK");
        }

        private async void sl_trackcontainer_ChildRemoved(object sender, ElementEventArgs e) { if (sl_trackcontainer.Children.Count == 0) { await Task.Delay(250); btn_back_click(sender, null); } }
    }
}