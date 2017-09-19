using System;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using Android.Support.V7.App;
using SupportToolbar = Android.Support.V7.Widget.Toolbar;
using Android.Preferences;

namespace MobilePozitivApp
{
    [Activity(Label = "ActivityAbout", Theme = "@style/MyTheme")]
    public class ActivityAbout : AppCompatActivity
    {
        private SupportToolbar mToolbar;
        private ImageView mImageView;
        private int ClickToDebug;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.ActivityAbout);

            mToolbar = FindViewById<SupportToolbar>(Resource.Id.Toolbar);
            mImageView = FindViewById<ImageView>(Resource.Id.ImageView);
            mImageView.Click += MImageView_Click;
            
            mToolbar.Title = "О программе";
            SetSupportActionBar(mToolbar);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetDisplayShowHomeEnabled(true);
        }

        private void MImageView_Click(object sender, EventArgs e)
        {
            ClickToDebug++;
            if (ClickToDebug == 10)
            {
                ISharedPreferences mPrefs = PreferenceManager.GetDefaultSharedPreferences(this);
                ISharedPreferencesEditor mPerfsEditor = mPrefs.Edit();
                mPerfsEditor.PutBoolean("debug", true);
                mPerfsEditor.Apply();
                AppVariable.Variable.DebugMode = true;
                Toast.MakeText(this, "Debug mode ON", ToastLength.Long).Show();
            }
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    Finish();
                    return true;
                default:
                    return base.OnOptionsItemSelected(item);
            }
        }
    }
}