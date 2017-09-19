using System;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Support.V7.App;
using Android.Webkit;
using Mono.CSharp;
using SupportToolbar = Android.Support.V7.Widget.Toolbar;

namespace MobilePozitivApp
{
    [Activity(Label = "ActivityElementsList", Theme = "@style/MyTheme", ConfigurationChanges = Android.Content.PM.ConfigChanges.Orientation | Android.Content.PM.ConfigChanges.ScreenSize)]
    class ActivityWebView : AppCompatActivity
    {
        WebView mWebView;
        private SupportToolbar mToolbar;

        string mName;
        string mHtmlBase64;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.ActivityWebView);

            mName = Intent.GetStringExtra("name");
            mHtmlBase64 = Intent.GetStringExtra("htmlbase64");
            int mScale = Intent.GetIntExtra("zoom", 0);
            int mTop = Intent.GetIntExtra("top", 0);
            int mLeft = Intent.GetIntExtra("left", 0);

            mToolbar = FindViewById<SupportToolbar>(Resource.Id.Toolbar);
            mToolbar.Title = mName;
            SetSupportActionBar(mToolbar);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetDisplayShowHomeEnabled(true);

            var base64EncodedBytes = Convert.FromBase64String(mHtmlBase64);
            string html = Encoding.UTF8.GetString(base64EncodedBytes);

            mWebView = (WebView)FindViewById(Resource.Id.WebView);
            mWebView.Settings.BuiltInZoomControls = true;
            mWebView.Settings.DisplayZoomControls = true;
            var customWebViewClient = new CustomWebViewClient();
            customWebViewClient.OnPageLoaded += (sender, b) => 
            {
                if (mScale != 0)
                {
                    mWebView.SetInitialScale(mScale);
                    mWebView.ScrollTo(mLeft, mTop);
                }
            };
            mWebView.SetWebViewClient(customWebViewClient);
            mWebView.LoadData(html, "text/html", "UTF-8");
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    Finish();
                    return true;
                case Resource.Id.action_refresh:
                    Intent intent = new Intent();
                    intent.PutExtra("zoom", (int)(mWebView.Scale * 100));
                    intent.PutExtra("top", mWebView.ScrollY);
                    intent.PutExtra("left", mWebView.ScrollX);
                    SetResult(Result.Ok, intent);
                    Finish();
                    return true;
                case Resource.Id.action_help:
                    return true;
                default:
                    return base.OnOptionsItemSelected(item);
            }
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.action_menu_main, menu);
            IMenuItem mMenuItem = menu.FindItem(Resource.Id.action_search);
            mMenuItem.SetVisible(false);
            return base.OnCreateOptionsMenu(menu);
        }
    }

    public class CustomWebViewClient : WebViewClient
    {
        public event EventHandler<bool> OnPageLoaded;

        public override bool ShouldOverrideUrlLoading(WebView view, IWebResourceRequest request)
        {
            view.LoadUrl(request.Url.ToString());
            return true;
        }

        public override void OnPageStarted(WebView view, string url, global::Android.Graphics.Bitmap favicon)
        {
            base.OnPageStarted(view, url, favicon);
        }

        public override void OnPageFinished(WebView view, string url)
        {
            OnPageLoaded?.Invoke(this, true);
        }

        public override void OnReceivedError(WebView view, IWebResourceRequest request, WebResourceError error)
        {
            OnPageLoaded?.Invoke(this, false);
        }
    }
}