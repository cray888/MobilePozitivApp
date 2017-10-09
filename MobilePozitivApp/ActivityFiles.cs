using System;
using System.Collections.Generic;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using Android.Support.V7.App;

using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

using SupportToolbar = Android.Support.V7.Widget.Toolbar;
using System.Net;
using Android.Webkit;

namespace MobilePozitivApp
{
    [Activity(Label = "ActivityElementsList", Theme = "@style/MyTheme", ConfigurationChanges = Android.Content.PM.ConfigChanges.Orientation | Android.Content.PM.ConfigChanges.ScreenSize)]
    class ActivityFiles : AppCompatActivity
    {
        private bool Downloaded;
        private string mRef;
        private string mName;
        private LinearLayout mFileInfo;
        private TextView mFileName;
        private TextView mFileAutor;
        private TextView mFileVersion;
        private TextView mFileSize;
        private TextView mFileExtension;
        private string mFileHref;
        private TextView mFileLref;
        private TextView mFileModDate;
        private CheckBox mFileDownloaded;
        private Button mFileDelete;
        private Button mFileOpen;

        private ProgressDialog mProgressDialog;
        private SupportToolbar mToolbar;
        Android.App.AlertDialog.Builder mAlertDialog;

        private IMenu mMenu;
        private WebClient mWebClient;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.ActivityFiles);

            mRef = Intent.GetStringExtra("ref");
            mName = Intent.GetStringExtra("name");

            mFileInfo = FindViewById<LinearLayout>(Resource.Id.FileInfo);
            mFileInfo.Visibility = ViewStates.Invisible;

            mFileName = FindViewById<TextView>(Resource.Id.FileName);
            mFileExtension = FindViewById<TextView>(Resource.Id.FileExtension);
            mFileLref = FindViewById<TextView>(Resource.Id.FilePath);            
            mFileVersion = FindViewById<TextView>(Resource.Id.FileVersion);
            mFileModDate = FindViewById<TextView>(Resource.Id.FileModDate);
            mFileAutor = FindViewById<TextView>(Resource.Id.FileAutor);
            mFileSize = FindViewById<TextView>(Resource.Id.FileSize);
            mFileDownloaded = FindViewById<CheckBox>(Resource.Id.FileDownloaded);

            mFileOpen = FindViewById<Button>(Resource.Id.FileOpen);
            mFileOpen.Click += MFileOpen_Click;

            mFileDelete = FindViewById<Button>(Resource.Id.FileDelete);
            mFileDelete.Click += MFileDelete_Click; ;

            mToolbar = FindViewById<SupportToolbar>(Resource.Id.Toolbar);
            mToolbar.Title = mName;
            SetSupportActionBar(mToolbar);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetDisplayShowHomeEnabled(true);

            mProgressDialog = new ProgressDialog(this);
            mProgressDialog.Indeterminate = true;
            mProgressDialog.SetProgressStyle(ProgressDialogStyle.Spinner);
            mProgressDialog.Max = 100;
            mProgressDialog.SetCancelable(false);

            mAlertDialog = new Android.App.AlertDialog.Builder(this);
            mAlertDialog.SetNeutralButton("OK", (sender, args) => { });
            mAlertDialog.SetCancelable(true);

            UpdateData();
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    Finish();
                    return true;
                case Resource.Id.action_save_dataView:
                    //SetDataByRef("Сохранить", "save");
                    return true;
                case Resource.Id.action_refresh_dataView:
                    Recreate();
                    return true;
                case Resource.Id.action_help_dataView:
                    return true;
                default:
                    return base.OnOptionsItemSelected(item);
            }
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            mMenu = menu;
            MenuInflater.Inflate(Resource.Menu.action_menu_view_element, menu);
            IMenuItem mMenuItem;
            mMenuItem = menu.FindItem(Resource.Id.action_save_dataView);
            mMenuItem.SetVisible(false);
            return base.OnCreateOptionsMenu(menu);
        }

        private void MFileDelete_Click(object sender, EventArgs e)
        {
            if (Downloaded)
            {
                Android.Support.V7.App.AlertDialog.Builder quitDialog = new Android.Support.V7.App.AlertDialog.Builder(this);
                quitDialog.SetTitle("Удалить файл?");
                quitDialog.SetPositiveButton("Да", (senderAlert, args) =>
                {
                    Java.IO.File mFile = new Java.IO.File(mFileLref.Text);
                    if (mFile.Exists()) mFile.Delete();
                    FileIsDownloaded();
                });
                quitDialog.SetNegativeButton("Нет", (senderAlert, args) =>
                {

                });
                Dialog dialog = quitDialog.Create();
                dialog.Show();
            }
        }

        private void MFileOpen_Click(object sender, EventArgs e)
        {
            if (Downloaded)
            {
                OpenFile();
            }
            else
            {
                DownloadFile();
            }
        }

        public void UpdateData()
        {
            mProgressDialog.SetMessage("Получение информации...");
            mProgressDialog.Show();

            DataSetWS dataSetWS = new DataSetWS();
            dataSetWS.GetFileCompleted += DataSetWS_GetFileCompleted;
            dataSetWS.GetFileAsync(mRef);
        }

        private void DataSetWS_GetFileCompleted(object sender, MobileAppPozitiv.GetFileCompletedEventArgs e)
        {
            
            if (e.Error == null)
            {
                if (e.Cancelled)
                {
                    Toast.MakeText(this, "Отменено", ToastLength.Short).Show();
                    Finish();
                }
                else
                {
                    JObject jsonResult = JObject.Parse(e.Result);
                    JObject jsonData = (JObject)jsonResult.Property("Data").Value;

                    mFileName.Text = (string)jsonData.Property("Name").Value;
                    mFileAutor.Text = (string)jsonData.Property("Autor").Value;
                    mFileVersion.Text = (string)jsonData.Property("Version").Value;
                    mFileSize.Text = (string)jsonData.Property("Size").Value;
                    mFileExtension.Text = (string)jsonData.Property("Extension").Value;
                    mFileHref = (string)jsonData.Property("Href").Value;
                    mFileLref.Text = (string)jsonData.Property("Lref").Value;
                    mFileModDate.Text = (string)jsonData.Property("ModDate").Value;                    

                    Java.IO.File folder = new Java.IO.File(Android.OS.Environment.ExternalStorageDirectory.AbsolutePath + "/Download/1C_Files/");
                    if (folder.Exists() == false)
                    {
                        folder.Mkdir();
                    }

                    folder = new Java.IO.File(Android.OS.Environment.ExternalStorageDirectory.AbsolutePath + "/Download/1C_Files/" + mFileLref.Text.Split('/')[0]);
                    if (folder.Exists() == false)
                    {
                        folder.Mkdir();
                    }

                    mFileLref.Text = Android.OS.Environment.ExternalStorageDirectory.AbsolutePath + "/Download/1C_Files/" + mFileLref.Text;

                    FileIsDownloaded();

                    mFileInfo.Visibility = ViewStates.Visible;
                }
            }
            else
            {
                Toast.MakeText(this, e.Error.Message, ToastLength.Long).Show();
                Finish();
            }
            mProgressDialog.Dismiss();
        }

        public void FileIsDownloaded()
        {
            Java.IO.File mFile = new Java.IO.File(mFileLref.Text);
            Downloaded = mFile.Exists();
            if (Downloaded)
            {                
                mFileDelete.Visibility = ViewStates.Visible;
            }
            else
            {
                mFileDelete.Visibility = ViewStates.Invisible;
            }
            mFileDownloaded.Checked = Downloaded;
        }

        public void DownloadFile()
        {
            mWebClient = new WebClient();
            mWebClient.DownloadFileCompleted += MWebClient_DownloadFileCompleted; ;
            mWebClient.DownloadProgressChanged += MWebClient_DownloadProgressChanged; ;
            var url = new Uri("http://1c.pozitivtelecom.ru/Files/" + mFileHref);
            mProgressDialog.SetMessage("Файл загружается...");
            mProgressDialog.Show();
            mWebClient.DownloadFileAsync(url, mFileLref.Text);
        }

        private void MWebClient_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            mProgressDialog.SetMessage("Файл загружается... (" + e.ProgressPercentage.ToString() + "%)");
        }

        private void MWebClient_DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            mProgressDialog.Dismiss();
            if (e.Error == null)
            {
                FileIsDownloaded();
                OpenFile();
            }
            else
            {
                Java.IO.File mFile = new Java.IO.File(mFileLref.Text);
                if (mFile.Exists()) mFile.Delete();

                mAlertDialog.SetTitle("Произошла ошибка.");
                mAlertDialog.SetMessage("Не удалось получить файл. Попробуйте позже.");
                mAlertDialog.Show();
            }
        }

        public void OpenFile()
        {
            var mimeType = MimeTypeMap.Singleton.GetMimeTypeFromExtension(System.IO.Path.GetExtension(mFileLref.Text).Replace(".", ""));
            if (mimeType == null) mimeType = "*/*";
            var intent = new Intent();
            intent.SetAction(Intent.ActionView);
            intent.SetDataAndType(Android.Net.Uri.Parse("file://" + mFileLref.Text), mimeType);
            try
            {
                StartActivity(intent);
            }
            catch
            {
                mAlertDialog.SetTitle("Произошла ошибка.");
                mAlertDialog.SetMessage("Не найдена программа для открытия данного вида файлов.");
                mAlertDialog.Show();
            }
        }
    }
}