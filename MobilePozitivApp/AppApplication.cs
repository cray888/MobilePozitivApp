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
using Android.Preferences;
using Firebase;
using Firebase.Iid;
using Firebase.Messaging;

namespace MobilePozitivApp
{
    [Application]
    class AppApplication : Application, Application.IActivityLifecycleCallbacks
    {
        public AppApplication(IntPtr handle, JniHandleOwnership ownerShip) : base(handle, ownerShip)
        {

        }

        public override void OnCreate()
        {
            base.OnCreate();
            RegisterActivityLifecycleCallbacks(this);
            AppPreferences.Preferences.InitPreferences(this);
            AppAlarmManager.SetAppAlarmManager(this);
        }

        public override void OnLowMemory()
        {
            base.OnLowMemory();
            //SaveVariable();
        }

        public override void OnTerminate()
        {
            base.OnTerminate();
            UnregisterActivityLifecycleCallbacks(this);
        }

        /*void SaveVariable(bool clear = false)
        {
            ISharedPreferences mPrefs = PreferenceManager.GetDefaultSharedPreferences(this);
            ISharedPreferencesEditor mPerfsEditor = mPrefs.Edit();

            if (clear)
            {
                mPerfsEditor.PutBoolean("vIsOnline", false);
                mPerfsEditor.PutString("vUser", "");
                mPerfsEditor.PutString("vPassword", "");
            }
            else
            {
                mPerfsEditor.PutBoolean("isOnline", AppVariable.Variable.isOnline);
                mPerfsEditor.PutString("User", AppVariable.Variable.User);
                mPerfsEditor.PutString("Password", AppVariable.Variable.Passwod);
            }

            mPerfsEditor.Apply();
        }*/

        /*void LoadVariable()
        {
            if (AppVariable.Variable.isOnline == false)
            {
                ISharedPreferences mPrefs = PreferenceManager.GetDefaultSharedPreferences(this);
                AppVariable.Variable.isOnline = mPrefs.GetBoolean("visOnline", false);
                AppVariable.Variable.User = mPrefs.GetString("vUser", "");
                AppVariable.Variable.Passwod = mPrefs.GetString("vPasswod", "");
                SaveVariable(true);
            }
        }*/

        public void CheckSession(Activity Contex)
        {
            if (AppVariable.Variable.isOnline != true)
            {
                Intent intent = new Intent(Contex, typeof(ActivityLogin));
                intent.AddFlags(ActivityFlags.ClearTop | ActivityFlags.ClearTask | ActivityFlags.NewTask);
                Contex.StartActivity(intent);
            }
        }

        ///////////////////////////////////
        public void OnActivityCreated(Activity activity, Bundle savedInstanceState)
        {
            if (activity.GetType() != typeof(ActivityLogin)) CheckSession(activity);
        }

        public void OnActivityDestroyed(Activity activity)
        {

        }

        public void OnActivityPaused(Activity activity)
        {

        }

        public void OnActivityResumed(Activity activity)
        {

        }

        public void OnActivitySaveInstanceState(Activity activity, Bundle outState)
        {

        }

        public void OnActivityStarted(Activity activity)
        {

        }

        public void OnActivityStopped(Activity activity)
        {

        }
    }
}