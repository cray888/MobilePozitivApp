using System;

using Android.App;
using Android.Views;
using Android.Widget;
using Android.Support.V7.Widget;
using Newtonsoft.Json.Linq;

namespace MobilePozitivApp
{
    public class NewsHolder : RecyclerView.ViewHolder
    {
        public TextView Title { get; private set; }
        public TextView Text { get; private set; }
        public TextView Date { get; private set; }
        public TextView Autor { get; private set; }

        public NewsHolder(View itemView, Action<int> listener)
            : base(itemView)
        {
            //Image = itemView.FindViewById<ImageView>(Resource.Id.imageView);
            Title = itemView.FindViewById<TextView>(Resource.Id.NewsTitle);
            Text = itemView.FindViewById<TextView>(Resource.Id.NewsText);
            Date = itemView.FindViewById<TextView>(Resource.Id.NewsDate);
            Autor = itemView.FindViewById<TextView>(Resource.Id.NewsAutor);

            //itemView.Click += (sender, e) => listener(base.Position);
        }
    }

    public class AdapterNews : RecyclerView.Adapter
    {
        public event EventHandler<int> ItemClick;

        Activity mContext;

        DataNews mDataNews;

        public AdapterNews(Activity context)
        {
            this.mContext = context;
            mDataNews = new DataNews();
            UpdateList();
        }

        public void UpdateList()
        {
            DataSetWS dataSetWS = new DataSetWS();
            string AllNewsResult = dataSetWS.GetNews();

            JObject jsonResult = JObject.Parse(AllNewsResult);

            foreach (JObject Elements in jsonResult["Data"])
            {
                JValue jTitle = (JValue)Elements["Title"];
                JValue jDate = (JValue)Elements["Date"];
                JValue jText = (JValue)Elements["Text"];
                JValue jAutor = (JValue)Elements["Autor"];
                JValue jFix = (JValue)Elements["Fix"];
                mDataNews.Add(new News
                {
                    Title = (string)jTitle.Value,
                    Date = (DateTime)jDate.Value,
                    Text = (string)jText.Value,
                    Autor = (string)jAutor.Value,
                    Fix = (bool)jFix.Value
                });
            }
        }

        public override RecyclerView.ViewHolder
            OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.RecycleViewNews, parent, false);

            NewsHolder vh = new NewsHolder(itemView, OnClick);
            return vh;
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            NewsHolder vh = holder as NewsHolder;

            //vh.Image.SetImageResource(DataNews[position].PhotoID);
            vh.Title.Text = mDataNews[position].Title;
            vh.Text.Text = mDataNews[position].Text;
            vh.Date.Text = mDataNews[position].Date.ToShortDateString();
            vh.Autor.Text = mDataNews[position].Autor;
        }

        // Return the number of photos available in the photo album:
        public override int ItemCount
        {
            get { return mDataNews.NumNews; }
        }

        // Raise an event when the item-click takes place:
        void OnClick(int position)
        {
            if (ItemClick != null)
                ItemClick(this, position);
        }
    }
}