using Android.App;
using Android.Widget;
using Android.OS;
using ZXing.Mobile;

namespace BarcodeScaner
{
    [Activity(Label = "BarcodeScaner", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        private Button mGetCode;
        private TextView mCode;
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Main);

            mGetCode = FindViewById<Button>(Resource.Id.btnGetCode);
            mCode = FindViewById<TextView>(Resource.Id.txtCode);
            mCode.Text = "Не считано";

            mGetCode.Click += async (sender, args) =>
            {
                MobileBarcodeScanner.Initialize(Application);
                var scanner = new ZXing.Mobile.MobileBarcodeScanner();
                var result = await scanner.Scan();
                mCode.Text = result != null ? result.Text : "Отменено";
            };
        }
    }
}

