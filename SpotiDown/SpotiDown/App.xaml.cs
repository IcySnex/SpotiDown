using Xamarin.Forms;

namespace SpotiDown
{
    public partial class App : Application
    {
        public static tab_bar tb = new tab_bar();
        public App()
        {
            InitializeComponent();
            MainPage = new NavigationPage(tb);
        }

        protected override void OnStart() { }

        protected override void OnSleep() { }

        protected override void OnResume() { }
    }
}
