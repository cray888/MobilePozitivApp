using System;
using Android.Content;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Views;
using Android.Widget;
using Java.Lang;
using System.Collections.Generic;

namespace MobilePozitivApp
{
    class AdapterCustomPager : FragmentPagerAdapter
    {
        //new
        private List<Fragment> mFragmentList = new List<Fragment>();
        private List<string> mFragmentTitleList = new List<string>();
        readonly Context context;

        public AdapterCustomPager(Context context, FragmentManager fm) : base(fm)
        {
            this.context = context;
        }

        public override int Count
        {
            get { return mFragmentList.Count; }
        }

        public override Fragment GetItem(int position)
        {
            return mFragmentList[position];
        }

        public override ICharSequence GetPageTitleFormatted(int position)
        {
            return new Java.Lang.String(mFragmentTitleList[position].ToLower());
        }

        public View GetTabView(int position)
        {
            var tv = (TextView)LayoutInflater.From(context).Inflate(Resource.Layout.CustomTab, null);
            tv.Text = mFragmentTitleList[position];
            return tv;
        }

        public void addFragment(Fragment fragment, string title)
        {
            mFragmentList.Add(fragment);
            mFragmentTitleList.Add(title);
        }
    }
}