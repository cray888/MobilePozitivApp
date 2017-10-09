using System;
using Android.App;
using Android.Content;
using Android.Media;
using Android.Util;
using Firebase.Messaging;
using Android;
using Android.Widget;
using Newtonsoft.Json.Linq;

namespace MobilePozitivApp
{
    [Service]
    [IntentFilter(new[] { "com.google.firebase.MESSAGING_EVENT" })]
    public class AppFirebaseMessagingService : FirebaseMessagingService
    {
        public override void OnMessageReceived(RemoteMessage message)
        {
            if (message.GetNotification() != null)
            {
                int nid = message.Data.ContainsKey("id") ? int.Parse(message.Data["id"]) : AppVariable.nIDFcm;
                string nref = message.Data.ContainsKey("ref") ? message.Data["ref"] : string.Empty;
                string nreflistmod = message.Data.ContainsKey("reflistmod") ? message.Data["reflistmod"] : string.Empty;
                string nname = message.Data.ContainsKey("name") ? message.Data["name"] : string.Empty;

                ShowNotification(message.GetNotification().Title, message.GetNotification().Body, nid, nref, nreflistmod, nname);
            }
            else
            {
                if (message.Data.Count > 0)
                {
                    if (message.Data["confirmation"] == "true")
                    {
                        if (!AppPreferences.Preferences.GetPreferences("remember", false))
                        {
                            return;
                        }

                        if (!AppVariable.Variable.isOnline)
                        {
                            AppPreferences.Preferences.LoadPreferences();
                        }

                        DataSetWS dataSetWS = new DataSetWS();
                        dataSetWS.SetMessageCompleted += DataSetWS_SetMessageCompleted;
                        dataSetWS.SetMessageAsync("{\"action\" : \"setstate\", \"id\" : " + message.Data["id"] + ", \"status\" : \"recive\"}");
                    }
                }
            }
        }

        private void DataSetWS_SetMessageCompleted(object sender, MobileAppPozitiv.SetMessageCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                if (e.Cancelled) {} else {}
            }
            else
            {
                Toast.MakeText(this, e.Error.Message, ToastLength.Long).Show();
            }
        }

        void ShowNotification(string messageTytle, string messageBody, int id, string Ref, string RefListMod, string Name)
        {
            Intent intent;
            if (Ref != string.Empty && RefListMod != string.Empty)
            {
                intent = new Intent(this, typeof(ActivityDataView));
                intent.PutExtra("reflistmod", RefListMod);
                intent.PutExtra("ref", Ref);
                intent.PutExtra("name", Name);
            }
            else
            { 
                intent = new Intent(this, typeof(ActivityMain));
                intent.PutExtra("openmessages", true);
            }
            
            var pendingIntent = PendingIntent.GetActivity(this, AppVariable.nIDFcm, intent, PendingIntentFlags.OneShot);
            var notificationBuilder = new Notification.Builder(this)
                .SetSmallIcon(Resource.Drawable.ic_action_unread)
                .SetContentTitle(messageTytle)
                .SetContentText(messageBody)
                .SetAutoCancel(true)
                .SetDefaults(NotificationDefaults.Lights | NotificationDefaults.Sound | NotificationDefaults.Vibrate)
                .SetContentIntent(pendingIntent);
            Notification notification = new Notification.BigTextStyle(notificationBuilder).BigText(messageBody).Build();
            var notificationManager = GetSystemService(Context.NotificationService) as NotificationManager;
            notificationManager.Notify(id, notification);
        }
    }
}