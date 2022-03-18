using Android.App;
using Android.Content;
using Android.OS;
using AndroidX.AppCompat.App;
using SpotiDown.Droid;
[Activity(Theme = "@style/MyTheme.Splash", MainLauncher = true, NoHistory = true, Icon = "@mipmap/icon", RoundIcon = "@mipmap/icon")]
public class SplashActivity : AppCompatActivity
{
    static readonly string TAG = "X:" + typeof(SplashActivity).Name;
    public override void OnCreate(Bundle savedInstanceState, PersistableBundle persistentState) => base.OnCreate(savedInstanceState, persistentState);
    protected override void OnResume()
    {
        base.OnResume();
        StartActivity(new Intent(Application.Context, typeof(MainActivity)));
    }
}