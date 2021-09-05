using SpotiDownVB;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SpotiDown
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class page_loading : ContentPage
    {
        CancellationTokenSource cancel = new CancellationTokenSource();
        public page_loading(string url)
        {
            InitializeComponent();
            LoadDownloadPage(url);
        }

        private async void btn_back_click(object sender, EventArgs e) { try { await Navigation.PopModalAsync(true); cancel.Cancel(); } catch { } }

        private async void LoadDownloadPage(string url)
        {
            try
            {
                var progress = new Progress<int>(percent => { lb_text.Text = $"Fetching songs from Spotify... ({percent}%)"; pb_percentage.ProgressTo((Double)percent/100, 300, Easing.Linear); });

                Task<DownloadPageInfo> task_GetDownloadPageInfo = Task.Run(() => SpotifyHelper.GetDownloadPageInfo(url, progress, cancel.Token), cancel.Token);
                page_download dlp = new page_download(await task_GetDownloadPageInfo);
                await Navigation.PopModalAsync(true);
                await App.Current.MainPage.Navigation.PushModalAsync(dlp);
            }
            catch (Exception ex)
            {
                await DisplayAlert("Something went wrong - Error", ex.Message, "OK");
                await Navigation.PopModalAsync(true);
            }
        }

    }
}