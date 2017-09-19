using System;
using System.Threading.Tasks;
using System.Threading;

using Android.App;
using Android.Content;
using Android.Views;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V4.Widget;
using Android.Support.V7.Widget;
using AlertDialog = Android.Support.V7.App.AlertDialog;
using System.Collections.Generic;
using Android.Widget;

namespace MobilePozitivApp
{
	class FragmentMessages: Android.Support.V4.App.Fragment
	{        
        private Activity mContext;
	    private AdapterMessages mAdapterMessages;
	    private RecyclerView mRecyclerView;
	    private SwipeRefreshLayout mSwipeRefreshLayout;
	    private TextView mEemptyList;
        private Dialog mDialog;
	    private List<string> mListDataSet;
        private long mCurentMessageID;
	    private int mCurentMessageListID;

        public FragmentMessages()
		{

		}

        public override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
        }

		public static Android.Support.V4.App.Fragment newInstance(Context context)
		{
            FragmentMessages busrouteFragment = new FragmentMessages();
			return busrouteFragment;
		}

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
		    ViewGroup root = (ViewGroup)inflater.Inflate(Resource.Layout.FragmentMessages, null);
            mContext = (Activity)container.Context;

		    RecyclerView.LayoutManager mLayoutManager = new LinearLayoutManager(mContext);
            mRecyclerView = root.FindViewById<RecyclerView>(Resource.Id.recyclerView);            
            mRecyclerView.SetLayoutManager(mLayoutManager);

            mSwipeRefreshLayout = root.FindViewById<SwipeRefreshLayout>(Resource.Id.swipeLayout);
            mSwipeRefreshLayout.SetColorSchemeResources(Android.Resource.Color.HoloBlueBright, Android.Resource.Color.HoloBlueDark, Android.Resource.Color.HoloGreenLight, Android.Resource.Color.HoloRedLight);
            mSwipeRefreshLayout.Refresh += (s, e) => UpdateList();

		    FloatingActionButton mFloatingActionButton = root.FindViewById<FloatingActionButton>(Resource.Id.Fab);
            mFloatingActionButton.Click += MFloatingActionButton_Click;

		    mEemptyList = root.FindViewById<TextView>(Resource.Id.ViewEmpty);

		    mListDataSet = new List<string>();
		    mListDataSet.Add("Ответить");
		    mListDataSet.Add("Переслать");
            mListDataSet.Add("Удалить");
		    ArrayAdapter<string> mListAdapter = new ArrayAdapter<string>(mContext, Android.Resource.Layout.SimpleListItem1, mListDataSet);

		    AlertDialog.Builder mAlertDialog = new AlertDialog.Builder(mContext)
		        .SetTitle("Выберите действие:")
		        .SetAdapter(mListAdapter, OnSelectAction);
		    mDialog = mAlertDialog.Create();

            return root;
		}

        private void OnSelectAction(object sender, DialogClickEventArgs e)
        {
            switch (mListDataSet[e.Which])
            {
                case "Ответить":
                    ReplyMessage();
                    break;
                case "Переслать":
                    ForwardMessage();
                    break;
                case "Удалить":
                    DeleteMessage();
                    return;
            }
        }

        private void ReplyMessage()
        {
            Intent intent = new Intent(mContext, typeof(ActivityMessage));
            intent.PutExtra("UserStr", mAdapterMessages.GetItemAtPosition(mCurentMessageListID).UserStr);
            intent.PutExtra("UserRef", mAdapterMessages.GetItemAtPosition(mCurentMessageListID).UserRef);
            intent.PutExtra("Title", "RE: " + mAdapterMessages.GetItemAtPosition(mCurentMessageListID).Title);
            StartActivity(intent);
        }

	    private void ForwardMessage()
	    {
	        Intent intent = new Intent(mContext, typeof(ActivityMessage));
	        intent.PutExtra("Title", "FVD: " + mAdapterMessages.GetItemAtPosition(mCurentMessageListID).Title);
	        intent.PutExtra("Text", mAdapterMessages.GetItemAtPosition(mCurentMessageListID).Date + " " + mAdapterMessages.GetItemAtPosition(mCurentMessageListID).UserStr + " писал(а): " + mAdapterMessages.GetItemAtPosition(mCurentMessageListID).Text);
            StartActivity(intent);
	    }

        private void DeleteMessage()
        {
            DataSetWS dataSetWs = new DataSetWS();
            dataSetWs.SetMessageCompleted += DataSetWs_SetMessageCompleted;
            dataSetWs.SetMessageAsync("{\"action\" : \"setstate\", \"id\" : " + mCurentMessageID.ToString() + ", \"status\" : \"delete\"}");
        }

        private void DataSetWs_SetMessageCompleted(object sender, MobileAppPozitiv.SetMessageCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                if (e.Cancelled) Toast.MakeText(mContext, "Отменено", ToastLength.Short).Show();
                else
                {
                    Toast.MakeText(mContext, "Cообщение удалено", ToastLength.Long).Show();
                    UpdateList();
                }
            }
            else Toast.MakeText(mContext, e.Error.Message, ToastLength.Long).Show();
        }

        private void MFloatingActionButton_Click(object sender, EventArgs e)
        {
            Intent intent = new Intent(mContext, typeof(ActivityMessage));
            StartActivity(intent);
        }

        public void UpdateList()
        {
            if (AppVariable.Variable.isOnline != true) return;
            Task taskUpdate = new Task(() => {
                Thread.Sleep(500);
                mContext.RunOnUiThread(() => mSwipeRefreshLayout.Refreshing = true);
                mAdapterMessages = new AdapterMessages(mContext);
                mAdapterMessages.ItemClick += MAdapterMessages_ItemClick;
                mContext.RunOnUiThread(() =>
                {
                    mRecyclerView.SetAdapter(mAdapterMessages);
                    mSwipeRefreshLayout.Refreshing = false;
                    if (mAdapterMessages.ItemCount == 0)
                    {
                        mRecyclerView.Visibility = ViewStates.Gone;
                        mEemptyList.Visibility = ViewStates.Visible;
                    }
                    else
                    {
                        mRecyclerView.Visibility = ViewStates.Visible;
                        mEemptyList.Visibility = ViewStates.Gone;
                    }
                });
            });
            taskUpdate.Start();
        }

        private void MAdapterMessages_ItemClick(object sender, int e)
        {
            if (mAdapterMessages.ItemCount < e) return; 
            if (mAdapterMessages.GetItemAtPosition(e).UserRef == string.Empty) return;

            mCurentMessageListID = e;
            mCurentMessageID = mAdapterMessages.GetItemAtPosition(e).MessageID;
            mDialog.Show();
        }

        public override void OnResume()
        {
            base.OnResume();
            if (this.IsVisible) UpdateList();
        }
    }

}


