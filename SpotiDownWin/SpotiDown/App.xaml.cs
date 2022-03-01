using Microsoft.UI.Xaml;

namespace SpotiDown
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
        }

        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            m_window = new MainWindow();
            m_window.Activate();
        }

        private static Window? m_window;
        public static Window Window { get { return m_window!; } }
    }
}
