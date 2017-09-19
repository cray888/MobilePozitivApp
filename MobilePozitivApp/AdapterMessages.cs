using System;

using Android.App;
using Android.Views;
using Android.Widget;
using Android.Support.V7.Widget;
using Newtonsoft.Json.Linq;

namespace MobilePozitivApp
{
    

    public class AdapterMessages : RecyclerView.Adapter
    {
        public event EventHandler<int> ItemClick;

        Activity mContext;

        DataMessages mDataMessages;

        public class MessagesHolder : RecyclerView.ViewHolder
        {
            public ImageView ReciveRead { get; private set; }
            public ImageView Direction { get; private set; }
            public TextView Title { get; private set; }
            public TextView Text { get; private set; }
            public TextView Date { get; private set; }
            public TextView User { get; private set; }
            public CardView CardView { get; private set; }

            public MessagesHolder(View itemView, Action<int> listener)
                : base(itemView)
            {
                ReciveRead = itemView.FindViewById<ImageView>(Resource.Id.Recive);
                Direction = itemView.FindViewById<ImageView>(Resource.Id.Direction);
                Title = itemView.FindViewById<TextView>(Resource.Id.Title);
                Text = itemView.FindViewById<TextView>(Resource.Id.Text);
                Date = itemView.FindViewById<TextView>(Resource.Id.Date);
                User = itemView.FindViewById<TextView>(Resource.Id.User);

                CardView = itemView.FindViewById<CardView>(Resource.Id.CardView);

                itemView.LongClick += (sender, e) => listener(base.Position);
            }
        }

        public AdapterMessages(Activity context)
        {
            this.mContext = context;
            UpdateList();
        }

        public void UpdateList()
        {
            mDataMessages = new DataMessages();
            DataSetWS dataSetWS = new DataSetWS();
            string AllMessagesResult = string.Empty;
            try
            {
                AllMessagesResult = dataSetWS.GetMessage("");
            }
            catch (Exception e)
            {
                mContext.RunOnUiThread(() => {
                    Toast.MakeText(mContext, e.Message, ToastLength.Long).Show();
                });
                return;
            }

            JObject jsonResult = JObject.Parse(AllMessagesResult);

            foreach (JObject Elements in jsonResult["Data"])
            {
                JValue jDirectIn = (JValue)Elements["DirectIn"];
                JValue jMessageID = (JValue)Elements["MessageID"];
                JValue jTitle = (JValue)Elements["Title"];
                JValue jDate = (JValue)Elements["Date"];
                JValue jText = (JValue)Elements["Text"];
                JValue jUserRef = (JValue)Elements["UserRef"];
                JValue jUserStr = (JValue)Elements["UserStr"];
                JValue jRecive = (JValue)Elements["Recive"];
                JValue jRead = (JValue)Elements["Read"];
                JValue jFix = (JValue)Elements["Fix"];
                mDataMessages.Add(new Message
                {
                    DirectIn = (bool)jDirectIn.Value,
                    MessageID = (long)jMessageID.Value,
                    Title = (string)jTitle.Value,
                    Date = (DateTime)jDate.Value,
                    Text = (string)jText.Value,
                    UserRef = (string)jUserRef.Value,
                    UserStr = (string)jUserStr.Value,
                    Recive = (bool)jRecive.Value,
                    Read = (bool)jRead.Value,
                    Fix = (bool)jFix.Value
                });
            }
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.RecycleViewMessages, parent, false);

            MessagesHolder vh = new MessagesHolder(itemView, OnClick);
            return vh;
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            MessagesHolder vh = holder as MessagesHolder;

            CardView.LayoutParams layoutParams = new CardView.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent);

            if (mDataMessages[position].UserRef == string.Empty)
            {
                vh.ReciveRead.Visibility = ViewStates.Invisible;
                vh.Direction.Visibility = ViewStates.Invisible;
            }
            else
            {
                if (mDataMessages[position].DirectIn)
                {
                    layoutParams.SetMargins(0, 0, 50, 0);
                }
                else
                {
                    layoutParams.SetMargins(50, 0, 0, 0);
                }
                vh.CardView.LayoutParameters = layoutParams;

                if (mDataMessages[position].Read)
                {
                    vh.ReciveRead.SetImageResource(Resource.Drawable.ic_action_read);
                }
                else if (mDataMessages[position].Recive)
                {
                    vh.ReciveRead.SetImageResource(Resource.Drawable.ic_action_accept);
                }
                else
                {
                    vh.ReciveRead.Visibility = ViewStates.Invisible;
                }

                vh.Direction.SetImageResource(mDataMessages[position].DirectIn
                    ? Resource.Drawable.ic_action_expand
                    : Resource.Drawable.ic_action_collapse);
            }

            vh.Title.Text = mDataMessages[position].Title;
            vh.Text.Text = mDataMessages[position].Text;
            vh.Date.Text = mDataMessages[position].Date.ToShortDateString() + " " + mDataMessages[position].Date.ToShortTimeString();
            vh.User.Text = mDataMessages[position].UserStr;
        }

        public override int ItemCount
        {
            get { return mDataMessages.NumMessages; }
        }

        void OnClick(int position)
        {
            if (ItemClick != null)
                ItemClick(this, position);
        }

        public Message GetItemAtPosition(int position)
        {
            return mDataMessages[position];
        }
    }
}