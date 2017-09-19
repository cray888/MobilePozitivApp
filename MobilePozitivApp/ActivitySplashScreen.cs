using Android.App;
using Android.Content;
using Android.OS;

namespace MobilePozitivApp
{
    [Activity(Label = "Позитив телеком", MainLauncher = true, Icon = "@drawable/MainIcon", Theme = "@style/SplashTheme", ConfigurationChanges = Android.Content.PM.ConfigChanges.Orientation | Android.Content.PM.ConfigChanges.ScreenSize)]
    public class ActivitySplashScreen : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            Intent intent = new Intent(this, typeof(ActivityLogin));
            StartActivity(intent);
            Finish();

            base.OnCreate(savedInstanceState);
        }
    }
}