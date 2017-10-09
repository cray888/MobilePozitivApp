using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Support.V7.App;
using Android.Preferences;
using SupportToolbar = Android.Support.V7.Widget.Toolbar;

namespace MobilePozitivApp
{
    [Activity(Label = "ActivitySettings", Theme = "@style/MyTheme")]
    public class ActivitySettings : AppCompatActivity
    {
        SupportToolbar toolbar;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.ActivitySettings);

            string settings = Intent.GetStringExtra("settings");

            toolbar = FindViewById<SupportToolbar>(Resource.Id.my_toolbar);
            SetSupportActionBar(toolbar);
            SupportActionBar.Title = "Настройки";
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetDisplayShowHomeEnabled(true);

            FragmentManager.BeginTransaction().Add(Resource.Id.prefs_content, new SettingsFragment() { settings = settings }).Commit(); 
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

    public class SettingsFragment : PreferenceFragment
    {
        public string settings { get; set; }
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            if (settings == "user")
                AddPreferencesFromResource(Resource.Xml.settings_user);
            else if (settings == "admin")
                AddPreferencesFromResource(Resource.Xml.settings_admin);
            else if (settings == "app")
                AddPreferencesFromResource(Resource.Xml.settings_app);
        }
    }
}