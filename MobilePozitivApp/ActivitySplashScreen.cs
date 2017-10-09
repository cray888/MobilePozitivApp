using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;

namespace MobilePozitivApp
{
    [Activity(Label = "Позитив телеком", MainLauncher = true, Icon = "@drawable/MainIcon", Theme = "@style/SplashTheme", ConfigurationChanges = Android.Content.PM.ConfigChanges.Orientation | Android.Content.PM.ConfigChanges.ScreenSize)]
    public class ActivitySplashScreen : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            string mRef = Intent.GetStringExtra("ref");

            Intent intent = new Intent(this, typeof(ActivityLogin));

            intent.PutExtra("isMessage",    Intent.GetStringExtra("isMessage"));
            intent.PutExtra("ref",          Intent.GetStringExtra("ref"));
            intent.PutExtra("reflistmod",   Intent.GetStringExtra("reflistmod"));
            intent.PutExtra("name",         Intent.GetStringExtra("name"));

            StartActivity(intent);

            Finish();

            base.OnCreate(savedInstanceState);
        }
    }
}