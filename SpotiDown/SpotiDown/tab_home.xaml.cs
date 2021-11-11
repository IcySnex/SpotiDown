using System;
using SpotiDown.Helpers;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SpotiDown
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class tab_home : ContentPage
    {
        public tab_home() => InitializeComponent();

        private async void btn_search_click(object sender, EventArgs e)
        {
            try { 
                SpotifyHelper.client = await SpotifyHelper.ConnectToSpotify();
                if (SpotifyHelper.IsValidUrl(entry_url.Text)) { await Navigation.PushModalAsync(new page_loading(entry_url.Text)); } else { await DisplayAlert("Something went wrong - Error", "Input Url is not a valid Spotify Url.", "OK"); }
            }
            catch (Exception ex) { await DisplayAlert("Something went wrong - Error", ex.Message, "OK"); }
        }

        private async void btn_github_click(object sender, EventArgs e) => await Browser.OpenAsync("https://github.com/IcySnex/SpotiDown");
        private async void btn_youtube_click(object sender, EventArgs e) => await Browser.OpenAsync("https://www.youtube.com/channel/UCiaH5KyxTwmd57tbDLX5gmw");
        private async void btn_share_click(object sender, EventArgs e) => await Share.RequestAsync(new ShareTextRequest {Title = "SpotiDown - Download music from Spotify for free", Uri = "https://github.com/IcySnex/SpotiDown"});
        private async void btn_discord_click(object sender, EventArgs e) => await Browser.OpenAsync("https://discord.gg/3ycHEVMhT4");
    }
}