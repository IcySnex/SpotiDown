﻿using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using SpotiDown.Helpers;
using System;
using System.Threading.Tasks;
using Xamarin.Essentials;
using System.IO;

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

        private async void btn_download_click(object sender, EventArgs e)
        {

            if (await Permissions.CheckStatusAsync<Permissions.StorageWrite>() == PermissionStatus.Granted)
            {
                if (Helper.config.prefernces.format != 6 && !File.Exists(Helper.GetPath("ffmpeg")))
                {
                    string arch = Helper.ihs.getArch();
                    if (!await DisplayAlert("Something went wrong - Error", $"FFMPEG is not installed. Do you want to download the binary ({arch})?", "YES", "NO")) return;
                    await Helper.ihs.downloadFile($"https://github.com/IcySnex/SpotiDown/raw/main/other/ffmpeg-{arch}", "ffmpeg");
                    await DisplayAlert("Finished FFMPEG Download", $"Successfully downloaded ffmpeg binary ({arch}). Processing command now...", "OK");
                }
                btn_download.IsEnabled = false;
                for (int i = 0, loopTo = sl_trackcontainer.Children.Count - 1; i <= loopTo; i++)
                {
                    cw_track c = (cw_track)sl_trackcontainer.Children[0];
                    c.btn_download.IsEnabled = false;
                    await Downloadtrack(c.trackinfo);
                    sl_trackcontainer.Children.Remove(c);
                }
                btn_download.IsEnabled = true;
                await App.Current.MainPage.DisplayAlert("Done!", "All songs sucessfully downloaded!", "OK");
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
            try {
                string res = await YoutubeHelper.WriteTrack(trackinfo);
                if (res.Contains("Song failed to download"))
                    if (await App.Current.MainPage.DisplayAlert("Something went wrong - Error", res + " Do you want to retry?", "YES", "NO")) return await Downloadtrack(trackinfo); else return false;
                return true;
            } catch (Exception ex) { if (await App.Current.MainPage.DisplayAlert("Something went wrong - Error", $"Song failed to download! ('{ex.Message}') Do you want to retry?", "YES", "NO")) return await Downloadtrack(trackinfo); else return false; }
        }

        private async void sl_trackcontainer_ChildRemoved(object sender, ElementEventArgs e) { if (sl_trackcontainer.Children.Count == 0) { await Task.Delay(250); btn_back_click(sender, null); } }
    }
}