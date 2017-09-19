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
using System.Threading;
using Android.Util;

namespace MobilePozitivApp
{
    [Service(Exported = true, Name = "ru.pozitiv.mobileapp.service")]
    class AppService : Service
    {
        Handler handler;
        Action runnable;

        static readonly int DELAY_BETWEEN_LOG_MESSAGES = 5000;

        public override void OnCreate()
        {
            base.OnCreate();

            Notification.Builder builder = new Notification.Builder(this)
                .SetSmallIcon(Resource.Drawable.MainIcon);
            Notification notification;
            if (Build.VERSION.SdkInt < BuildVersionCodes.JellyBean)
                notification = builder.Notification;
            else
                notification = builder.Build();

            StartForeground(777, notification);

            handler = new Handler();
            runnable = new Action(() =>
            {
                Toast.MakeText(this, "Привет из Service", ToastLength.Short);
                handler.PostDelayed(runnable, DELAY_BETWEEN_LOG_MESSAGES);
            });
            handler.PostDelayed(runnable, DELAY_BETWEEN_LOG_MESSAGES);
        }

        public override IBinder OnBind(Intent intent)
        {
            return null;
        }
    }
        
    /*[Service(Exported = true, Name = "ru.pozitiv.mobileapp.service")]
    class AppService : Service
    {
        static readonly int DELAY_BETWEEN_LOG_MESSAGES = 300000; // milliseconds
        //static readonly int NOTIFICATION_ID = 10000;

        //bool isStarted;
        Handler handler;
        Action runnable;

        public override void OnCreate()
        {
            base.OnCreate();

            handler = new Handler();

            runnable = new Action(() =>
            {
                Toast.MakeText(this, "Привет из Service", ToastLength.Short);
                handler.PostDelayed(runnable, DELAY_BETWEEN_LOG_MESSAGES);
            });
        }

        public override IBinder OnBind(Intent intent)
        {
            throw new NotImplementedException();
        }
    }*/
}