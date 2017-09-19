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

using SupportToolbar = Android.Support.V7.Widget.Toolbar;
using Android.Support.Design.Widget;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace MobilePozitivApp
{
    [Activity(Label = "ActivityMessage", ConfigurationChanges = Android.Content.PM.ConfigChanges.Orientation | Android.Content.PM.ConfigChanges.ScreenSize)]
    public class ActivityMessage : AppCompatActivity
    {
        private SupportToolbar mToolbar;
        private ProgressDialog mProgressDialog;
        private FloatingActionButton mFloatingActionButton;

        private EditText mReciver;
        private EditText mTitle;
        private EditText mMessage;
        private ImageButton mReciverButton;
        private LinearLayout mRecivers;

        private EditText mCurentEditText;

        private int curentReciverID = 0;
        private int lastReciverID = 0;
        private Dictionary<int, string> Recivers;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.ActivityMessage);

            //Получатели
            mReciver = FindViewById<EditText>(Resource.Id.reciver);
            mRecivers = FindViewById<LinearLayout>(Resource.Id.recivers);
            mReciverButton = FindViewById<ImageButton>(Resource.Id.btnReciver);
            mReciverButton.Click += (s, o) =>
            {
                GetUser(0, mReciver);
            };
            Recivers = new Dictionary<int, string>();
            Recivers.Add(0, "");

            //Заголовок
            mTitle = FindViewById<EditText>(Resource.Id.title);

            //Сообщение
            mMessage = FindViewById<EditText>(Resource.Id.message);

            //Toolbar
            mToolbar = FindViewById<SupportToolbar>(Resource.Id.Toolbar);
            mToolbar.Title = "Новое сообщение";
            SetSupportActionBar(mToolbar);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetDisplayShowHomeEnabled(true);

            //FUB
            mFloatingActionButton = FindViewById<FloatingActionButton>(Resource.Id.fabSend);
            mFloatingActionButton.Click += MFloatingActionButton_Click;

            //Progress dialog
            mProgressDialog = new ProgressDialog(this);
            mProgressDialog.Indeterminate = true;
            mProgressDialog.SetProgressStyle(ProgressDialogStyle.Spinner);
            mProgressDialog.Max = 100;
            mProgressDialog.SetCancelable(false);

            if (Intent.GetStringExtra("UserStr") != null)
            {
                Recivers[curentReciverID] = Intent.GetStringExtra("UserRef");
                mReciver.Text = Intent.GetStringExtra("UserStr");
            }
            if (Intent.GetStringExtra("Title") != null) mTitle.Text = Intent.GetStringExtra("Title");
            if (Intent.GetStringExtra("Text") != null) mMessage.Text = Intent.GetStringExtra("Text");
            if (mTitle.Text == string.Empty) mTitle.Text = "Сообщение";
        }

        private void GetUser(int Id, EditText nTextDataEdit)
        {
            curentReciverID = (int)Id;
            Intent intentData = new Intent(this, typeof(ActivityDataList));
            mCurentEditText = nTextDataEdit;
            intentData.PutExtra("ref", "Справочник.Пользователи");
            intentData.PutExtra("name", "Пользователи");
            intentData.PutExtra("selected", true);
            StartActivityForResult(intentData, 1);
        }

        protected override void OnActivityResult(int requestCode, [Android.Runtime.GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            
            if (resultCode == Result.Ok)
            {
                Recivers[curentReciverID] = data.GetStringExtra("ref");
                mCurentEditText.Text = data.GetStringExtra("name");

            }
        }

        private void MFloatingActionButton_Click(object sender, EventArgs e)
        {
            SendMessage();
        }

        private void SendMessage()
        {
            if (CheckField())
            {
                mProgressDialog.SetMessage("Отправка...");
                mProgressDialog.Show();
                DataSetWS dataSetWs = new DataSetWS();
                dataSetWs.SetMessageCompleted += DataSetWs_SetMessageCompleted;
                dataSetWs.SetMessageAsync(new MessageSender(){ recivers = Recivers, title = mTitle.Text, message = mMessage.Text }.getJsonString());
            }
        }

        private void DataSetWs_SetMessageCompleted(object sender, MobileAppPozitiv.SetMessageCompletedEventArgs e)
        {
            mProgressDialog.Dismiss();
            if (e.Error == null)
            {
                if (e.Cancelled)
                {
                    Toast.MakeText(this, "Отменено", ToastLength.Short).Show();
                }
                else
                {
                    Toast.MakeText(this, "Отправлено", ToastLength.Short).Show();
                    Finish();
                }
            }
            else
            {
                Toast.MakeText(this, e.Error.Message, ToastLength.Short);
            }
        }

        private bool CheckField()
        {
            string errString = string.Empty;

            if (mReciver.Text == string.Empty) errString += "\"Получатель\", ";
            if (mTitle.Text == string.Empty) errString += "\"Заголовок\", ";
            if (mMessage.Text == string.Empty) errString += "\"Сообщение\", ";

            if (errString == string.Empty) return true;

            Snackbar.Make(FindViewById<LinearLayout>(Resource.Id.RootView), "Необходимо заполнить поле(я):\n" + errString.Trim().Trim(','), Snackbar.LengthLong).Show();
            return false;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    Finish();
                    return true;
                case Resource.Id.action_add_reciver:
                    Android.Support.V7.App.AlertDialog.Builder ab = new Android.Support.V7.App.AlertDialog.Builder(this);
                    
                    //ab.SetAdapter()
                    return true;
                default:
                    return base.OnOptionsItemSelected(item);
            }
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            //MenuInflater.Inflate(Resource.Menu.action_menu_message, menu);
            return base.OnCreateOptionsMenu(menu);
        }
    }

    class MessageSender
    {
        public string action = "send";
        public Dictionary<int, string> recivers { get; set; }
        public string title { get; set; }
        public string message { get; set; }

        public string getJsonString()
        {
            return JsonConvert.SerializeObject(this); ;
        }
    }
}