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

namespace MobilePozitivApp
{
    public class AppAlarmManager
    {
        static public void SetAppAlarmManager(Context context, int seconds = 300)
        {
            //Java.Util.Calendar calendar = Java.Util.Calendar.GetInstance(Java.Util.TimeZone.GetTimeZone("UTC"));
            //calendar.TimeInMillis = Java.Lang.JavaSystem.CurrentTimeMillis();
            Intent alarmIntent = new Intent(context, typeof(AppReceiverAlarm));
            PendingIntent pendingIntent = PendingIntent.GetBroadcast(context, 0, alarmIntent, 0);
            AlarmManager alarmManager = (AlarmManager)context.GetSystemService(Context.AlarmService);
            alarmManager.Set(AlarmType.RtcWakeup, Java.Lang.JavaSystem.CurrentTimeMillis() + seconds * 1000, pendingIntent);
        }
    }
}