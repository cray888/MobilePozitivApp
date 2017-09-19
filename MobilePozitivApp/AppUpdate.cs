using System;
using System.Net;

using Android.App;
using Android.Content;

using Newtonsoft.Json.Linq;
using Android.Widget;

namespace MobilePozitivApp
{
    class AppUpdate
    {
        private Activity mContext;
        private WebClient mWebClient;
        private ProgressDialog mProgressDialog;

        string downloadsPath;
        string localPathFile;

        public AppUpdate(Activity context)
        {
            mContext = context;
        }

        public bool CheckUpdate(int curentVersion)
        {
            downloadsPath = Android.OS.Environment.ExternalStorageDirectory.AbsolutePath + "/Download";
            string localPathFileApk = System.IO.Path.Combine(downloadsPath, "app.apk");
            Java.IO.File mFileApk = new Java.IO.File(localPathFileApk);
            if (mFileApk.Exists()) mFileApk.Delete();

            mWebClient = new WebClient();
            var url = new Uri("http://1c.pozitivtelecom.ru/MobileApp/version.json");
            string stringResult;
            JValue jVersion;

            try
            {
                stringResult = mWebClient.DownloadString(url);
                JObject jsonResult = JObject.Parse(stringResult);
                jVersion = (JValue)jsonResult["Version"];                
            }
            catch
            {
                return false;
            }
            long version = (long)jVersion.Value;
            return curentVersion != version;
        }

        public void Update()
        {
            mWebClient = new WebClient();
            mWebClient.DownloadFileCompleted += WebClient_DownloadFileCompleted;
            mWebClient.DownloadProgressChanged += WebClient_DownloadProgressChanged;

            var url = new Uri("http://1c.pozitivtelecom.ru/MobileApp/app.zip");

            string localFilename = "app.zip";
            localPathFile = System.IO.Path.Combine(downloadsPath, localFilename);

            Java.IO.File mFileZip = new Java.IO.File(localPathFile);
            if (mFileZip.Exists()) mFileZip.Delete();

            mProgressDialog = new ProgressDialog(mContext);
            mProgressDialog.Indeterminate = true;
            mProgressDialog.SetProgressStyle(ProgressDialogStyle.Spinner);
            mProgressDialog.SetCancelable(false);
            mProgressDialog.SetMessage("Обновление... (0%)");
            mProgressDialog.Show();

            mWebClient.DownloadFileAsync(url, localPathFile);
        }

        private void WebClient_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            mProgressDialog.SetMessage("Получение обновления... (" + e.ProgressPercentage.ToString() + "%)");
        }

        private void WebClient_DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            mProgressDialog.Dismiss();
            if (e.Error == null)
            {
                string localPathFileApk = System.IO.Path.Combine(downloadsPath, "app.apk");
                Java.IO.File mFileApk = new Java.IO.File(localPathFileApk);
                if (mFileApk.Exists()) mFileApk.Delete();

                if (!AppZip.Unzip(localPathFile, downloadsPath))
                {
                    var mAlert = new AlertDialog.Builder(mContext)
                        .SetTitle("Произошла ошибка.")
                        .SetMessage("Не удалось расспоковать обновление. Попробуйте позже.")
                        .SetCancelable(true)
                        .Show();
                    return;
                }

                Java.IO.File mFileZip = new Java.IO.File(localPathFile);
                if (mFileZip.Exists()) mFileZip.Delete();                

                mContext.Finish();
                Intent promptInstall = new Intent(Intent.ActionView);
                promptInstall.SetDataAndType(Android.Net.Uri.FromFile(mFileApk), "application/vnd.android.package-archive");
                promptInstall.SetFlags(ActivityFlags.NewTask);
                mContext.StartActivity(promptInstall);
            }
            else
            {
                Toast.MakeText(mContext, e.Error.Message, ToastLength.Short);
                var mAlert = new AlertDialog.Builder(mContext)
                    .SetTitle("Произошла ошибка.")
                    .SetMessage("Не удалось получить обновление. Попробуйте позже.")
                    .SetCancelable(true)
                    .Show();
            }
        }
    }
}