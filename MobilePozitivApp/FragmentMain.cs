using System;
using System.Threading.Tasks;
using System.Threading;

using Android.App;
using Android.Content;
using Android.Views;
using Android.OS;
using Android.Support.V4.Widget;
using Android.Support.V7.Widget;

namespace MobilePozitivApp
{
	class FragmentMain: Android.Support.V4.App.Fragment
	{        
        private ViewGroup root;
        private Activity mContext;

        AdapterNews mAdapternews;
        RecyclerView.LayoutManager mLayoutManager;
        RecyclerView mRecyclerView;
        SwipeRefreshLayout mSwipeRefreshLayout;

        public FragmentMain()
		{

		}

		public static Android.Support.V4.App.Fragment newInstance(Context context)
		{
            FragmentMain busrouteFragment = new FragmentMain();
			return busrouteFragment;
		}

		public override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
		}

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			root = (ViewGroup)inflater.Inflate(Resource.Layout.FragmentMain, null);
            mContext = (Activity)container.Context;

            mLayoutManager = new LinearLayoutManager(mContext);
            mRecyclerView = root.FindViewById<RecyclerView>(Resource.Id.recyclerViewNews);            
            mRecyclerView.SetLayoutManager(mLayoutManager);

            mSwipeRefreshLayout = root.FindViewById<SwipeRefreshLayout>(Resource.Id.swipeLayoutNews);
            mSwipeRefreshLayout.SetColorSchemeResources(Android.Resource.Color.HoloBlueBright, Android.Resource.Color.HoloBlueDark, Android.Resource.Color.HoloGreenLight, Android.Resource.Color.HoloRedLight);
            mSwipeRefreshLayout.Refresh += mSwipeRefreshLayout_Refresh;

            return root;
		}

        public void UpdateList()
        {
            if (AppVariable.Variable.isOnline != true) return;
            Task taskUpdate = new Task(() => {
                Thread.Sleep(500);
                mContext.RunOnUiThread(() => mSwipeRefreshLayout.Refreshing = true);
                mAdapternews = new AdapterNews(mContext);
                mContext.RunOnUiThread(() =>
                {
                    mRecyclerView.SetAdapter(mAdapternews);
                    mSwipeRefreshLayout.Refreshing = false;
                });
            });
            taskUpdate.Start();
        }

        void mSwipeRefreshLayout_Refresh(object sender, EventArgs e)
        {
            UpdateList();
        }

        public override void OnResume()
        {
            base.OnResume();
            UpdateList();
        }
    }

}


