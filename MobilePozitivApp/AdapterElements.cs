using System;
using System.Collections.Generic;

using Android.App;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json.Linq;

namespace MobilePozitivApp
{
    public class AdapterElements : BaseAdapter
    {
        Activity mContext;
        public List<DataElements> items;
        public string mRef;

        public AdapterElements(Activity context, string Ref) //We need a context to inflate our row view from
          : base()
        {
            this.mContext = context;
            this.mRef = Ref;

            UpdateList();
        }
        public override int Count
        {
            get { return items.Count; }
        }

        public override Java.Lang.Object GetItem(int position)
        {
            return position;
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var item = items[position];
            var view = (convertView ?? mContext.LayoutInflater.Inflate(Resource.Layout.ListViewElemets, parent, false)) as LinearLayout;
            var mImage = view.FindViewById(Resource.Id.Image) as ImageView;
            mImage.Visibility = ViewStates.Gone;
            var mName = view.FindViewById(Resource.Id.Name) as TextView;
            var mDescriptione = view.FindViewById(Resource.Id.Description) as TextView;
            mName.SetText(item.Name, TextView.BufferType.Normal);
            if (item.Description == string.Empty) mDescriptione.Visibility = ViewStates.Gone;
                else mDescriptione.SetText(item.Description, TextView.BufferType.Normal);

            return view;
        }

        public DataElements GetItemAtPosition(int position)
        {
            return items[position];
        }

        public void UpdateList()
        {
            items = new List<DataElements>();

            if (mRef == null) { return; }

            DataSetWS dataSetWS = new DataSetWS();

            string AllowGroupsResult = string.Empty;
            try
            { 
                AllowGroupsResult = dataSetWS.GetElements(mRef);
            }
            catch (Exception e)
            {
                mContext.RunOnUiThread(() => {
                    Toast.MakeText(mContext, e.Message, ToastLength.Long).Show();
                    return;
                });
                return;
            }

            JObject jsonResult = JObject.Parse(AllowGroupsResult);
            
            foreach (JObject Elements in jsonResult["Data"])
            {
                JValue jName = (JValue)Elements["Name"];
                JValue jDescription = (JValue)Elements["Description"];
                JValue jRef = (JValue)Elements["Ref"];
                JValue jRead = (JValue)Elements["Read"];
                JValue jEdit = (JValue)Elements["Edit"];
                JValue jDelete = (JValue)Elements["Delete"];
                items.Add(new DataElements() {
                    Name = (string)jName.Value,
                    Description = (string)jDescription.Value,
                    Ref = (string)jRef.Value,
                    Read = (bool)jRead.Value,
                    Edit = (bool)jEdit.Value,
                    Delete = (bool)jDelete.Value
                });
            }
        }
    }
}