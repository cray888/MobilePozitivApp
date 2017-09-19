using System;
using Android.OS;
using Android.Support.V4.App;
using Android.Views;
using Android.Widget;
using Java.Lang;

namespace MobilePozitivApp
{
    class FragmentTest : Fragment
    {
        private int page;

        public static FragmentTest newInstance(int page)
        {
            FragmentTest fragmentTest = new FragmentTest();
            Bundle args = new Bundle();
            args.PutInt("page", page);
            fragmentTest.Arguments = args;
            return fragmentTest;
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            page = Arguments.GetInt("page", 0);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.FragmentTest, container, false);
            var textView = (TextView)view;
            textView.Text = "Fragment #" + page;
            return view;
        }
    }
}