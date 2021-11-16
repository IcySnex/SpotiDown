using SpotiDown.Helpers;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SpotiDown
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class tab_bar : TabbedPage
    {

        public tab_bar() { InitializeComponent(); SetDebugmode(); }

        public tab_ffmpeg tff = new tab_ffmpeg() { Title = "FFMPEG" };
        public void SetDebugmode()
        {
            if (Helper.config.debug && !Children.Contains(tff)) Children.Add(tff);
            if (!Helper.config.debug && Children.Contains(tff)) Children.Remove(tff);
        }
    }
}