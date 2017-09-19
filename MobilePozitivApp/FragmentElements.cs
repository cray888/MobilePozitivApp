using System;
using System.Threading.Tasks;
using System.Threading;

using Android.App;
using Android.Content;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Support.V4.Widget;


namespace MobilePozitivApp
{
	class FragmentElements : Android.Support.V4.App.Fragment
	{
        private ViewGroup root;
        private Activity mContext;

        private ListView mlistVivewDirectories;
        private AdapterElements adapterElements;
        private SwipeRefreshLayout mSwipeRefreshLayout;

        public bool isReport { get; set; }
        private string mRef;

        public FragmentElements()
		{

        }

		public static Android.Support.V4.App.Fragment newInstance(Context context)
		{
			FragmentElements busrouteFragment = new FragmentElements();
			return busrouteFragment;
		}

		public override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
		}

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			root = (ViewGroup)inflater.Inflate(Resource.Layout.FragmentElements, null);

            mContext = (Activity)container.Context;

            mlistVivewDirectories = root.FindViewById<ListView>(Resource.Id.listViewElements);
            mlistVivewDirectories.ItemClick += MlistVivewDirectories_ItemClick;

            mSwipeRefreshLayout = root.FindViewById<SwipeRefreshLayout>(Resource.Id.swipeLayoutElements);
            mSwipeRefreshLayout.SetColorSchemeResources(Android.Resource.Color.HoloBlueBright, Android.Resource.Color.HoloBlueDark, Android.Resource.Color.HoloGreenLight, Android.Resource.Color.HoloRedLight);
            mSwipeRefreshLayout.Refresh += mSwipeRefreshLayout_Refresh;

            return root;
		}

        private void MlistVivewDirectories_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            var item = this.adapterElements.GetItemAtPosition(e.Position);
            
            Intent intent;
            if (isReport)
            {
                intent = new Intent(mContext, typeof(ActivityDataView));
                intent.PutExtra("reflistmod", item.Ref); 
                intent.PutExtra("ref", "");
                intent.PutExtra("name", item.Name);
            }
            else
            {
                intent = new Intent(mContext, typeof(ActivityDataList));
                intent.PutExtra("ref", item.Ref);
                intent.PutExtra("name", item.Name);
                intent.PutExtra("read", item.Read);
                intent.PutExtra("edit", item.Edit);
                intent.PutExtra("delete", item.Delete);
            }
            StartActivity(intent);
        }

        public void UpdateList(string Ref)
        {
            if (AppVariable.Variable.isOnline != true) return;
            this.mRef = Ref;
            Task taskUpdate = new Task(() => {
                Thread.Sleep(100);
                mContext.RunOnUiThread(() => mSwipeRefreshLayout.Refreshing = true);
                adapterElements = new AdapterElements(mContext, Ref);
                mContext.RunOnUiThread(() =>
                {
                    mlistVivewDirectories.Adapter = adapterElements;
                    mSwipeRefreshLayout.Refreshing = false;
                });
            });
            taskUpdate.Start();
        }

        void mSwipeRefreshLayout_Refresh(object sender, EventArgs e)
        {
            UpdateList(mRef);
        }

        public override void OnResume()
        {
            base.OnResume();
            UpdateList(mRef);
        }
    }
}