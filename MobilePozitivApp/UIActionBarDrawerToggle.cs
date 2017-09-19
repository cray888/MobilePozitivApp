using System;
using SupportActionBarDrawerToggle = Android.Support.V7.App.ActionBarDrawerToggle;
using Android.Support.V7.App;
using Android.Support.V4.Widget;

namespace MobilePozitivApp
{
	public class UIActionBarDrawerToggle : SupportActionBarDrawerToggle
	{
		private AppCompatActivity mHostActivity;

		public UIActionBarDrawerToggle (AppCompatActivity host, DrawerLayout drawerLayout) : base(host, drawerLayout, 0, 0)
		{
			mHostActivity = host;
		}

		public override void OnDrawerOpened (Android.Views.View drawerView)
		{	
			if ((int)drawerView.Tag == 0) base.OnDrawerOpened (drawerView);
		}

		public override void OnDrawerClosed (Android.Views.View drawerView)
		{
			if ((int)drawerView.Tag == 0) base.OnDrawerClosed (drawerView);
		}

		public override void OnDrawerSlide (Android.Views.View drawerView, float slideOffset)
		{
			if ((int)drawerView.Tag == 0) base.OnDrawerSlide (drawerView, slideOffset);
		}
	}
}

