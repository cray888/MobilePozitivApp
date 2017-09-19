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
                int nid = message.Data.ContainsKey("id") ? AppVariable.nIDFcm: int.Parse(message.Data["id"]);
                ShowNotification(message.GetNotification().Title, message.GetNotification().Body, nid);
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

        void ShowNotification(string messageTytle, string messageBody, int id)
        {
            var intent = new Intent(this, typeof(ActivityMain));
            intent.PutExtra("openmessages", true);
            intent.AddFlags(ActivityFlags.ClearTop);            
            var pendingIntent = PendingIntent.GetActivity(this, AppVariable.nIDFcm, intent, PendingIntentFlags.OneShot);
            var defaultSoudnUri = RingtoneManager.GetDefaultUri(RingtoneType.Notification);
            var notificationBuilder = new Notification.Builder(this)
                .SetSmallIcon(Resource.Drawable.ic_action_unread)
                .SetContentTitle(messageTytle)
                .SetContentText(messageBody)
                .SetAutoCancel(true)
                .SetSound(defaultSoudnUri)
                .SetContentIntent(pendingIntent);
            var notificationManager = GetSystemService(Context.NotificationService) as NotificationManager;
            notificationManager.Notify(id, notificationBuilder.Build());
        }
    }
}