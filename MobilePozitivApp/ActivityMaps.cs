using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

using Android.App;
using Android.Content;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Support.V7.App;
using SupportToolbar = Android.Support.V7.Widget.Toolbar;

using Java.Net;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

namespace MobilePozitivApp
{
    [Activity(Label = "ActivityMaps")]
    public class ActivityMaps : AppCompatActivity, IOnMapReadyCallback, GoogleMap.IOnMarkerClickListener
    {
        private GoogleMap map;
        private SupportToolbar mToolbar;
        private IMenu mMenu;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.ActivityMaps);

            SupportMapFragment mapFragment = (SupportMapFragment)SupportFragmentManager.FindFragmentById(Resource.Id.map);
            mapFragment.GetMapAsync(this);

            mToolbar = FindViewById<SupportToolbar>(Resource.Id.Toolbar);
            mToolbar.Title = "Карта";
            SetSupportActionBar(mToolbar);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetDisplayShowHomeEnabled(true);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    Finish();
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
            IMenuItem mMenuItem = menu.FindItem(Resource.Id.action_save_dataView);
            mMenuItem.SetVisible(false);
            return base.OnCreateOptionsMenu(menu);
        }

        public void OnMapReady(GoogleMap googleMap)
        {
            map = googleMap;
            var options = new TileOverlayOptions();
            options.InvokeTileProvider(new CustomTileProvider(0));

            map.MapType = GoogleMap.MapTypeNone;
            map.AddTileOverlay(options);
            map.SetOnMarkerClickListener(this);

            map.UiSettings.ZoomControlsEnabled = true;
            map.SetMaxZoomPreference(18);

            map.AddMarker(new MarkerOptions().SetPosition(new LatLng(-27.47093, 153.0235)).SetTitle("HOME"));

            GeoCode mGeoCode = new GeoCode("Тюмень, ул. Широтная, д. 158, к. 1");
            mGeoCode.Complited += (pos, lowerCorner, upperCorner) =>
            {
                RunOnUiThread(() =>
                {
                    map.AddMarker(new MarkerOptions().SetPosition(pos).SetTitle("HOME"));
                    map.MoveCamera(CameraUpdateFactory.NewLatLngBounds(new LatLngBounds(lowerCorner, upperCorner), 0));
                });
            };
            mGeoCode.Error += e =>
            {
                Toast.MakeText(this, e.ToString(), ToastLength.Short).Show();
            };
        }

        public bool OnMarkerClick(Marker marker)
        {
            Toast.MakeText(this, marker.Title, ToastLength.Short).Show();
            return false;
        }
    }

    class CustomTileProvider : UrlTileProvider
    {
        private static string[] TILE_URL ={
            "http://tile0.maps.2gis.com/tiles?x={0}&y={1}&z={2}&v=1.3",
            "http://a.tile.openstreetmap.org/{2}/{0}/{1}.png",
            "https://mts1.google.com/vt/lyrs=m@186112443&hl=x-local&src=app&x={0}&y={1}&z={2}&s=Galile",
            "http://a.tiles.maps.sputnik.ru/{2}/{0}/{1}.png",
            //"http://vec04.maps.yandex.net/tiles?l=map&v=17.09.27-0&x={0}&y={1}&z={2}&scale=1&lang=ru_RU",
        };

        private int service;

        private static int MIN_ZOOM = 1;
        private static int MAX_ZOOM = 18;

        public CustomTileProvider(int service) : base(256, 256)
        {
            this.service = service;
        }

        public override URL GetTileUrl(int x, int y, int zoom)
        {
            string s = string.Format(TILE_URL[service], x, y, zoom);

            if (zoom < MIN_ZOOM || zoom > MAX_ZOOM)
            {
                return null;
            }

            try
            {
                return new URL(s);
            }
            catch (Exception e)
            {
                return null;
            }
        }
    }

    public class GeoCode
    {
        public delegate void GeoCodeStateHandler(LatLng ePos, LatLng eLowerCorner, LatLng eUpperCorner);
        public delegate void GeoCodeErrorHandler(Exception e);
        public event GeoCodeStateHandler Complited;
        public event GeoCodeErrorHandler Error;

        public LatLng Pos { get; private set; }
        public LatLng LowerCorner { get; private set; }
        public LatLng UpperCorner { get; private set; }

        private WebClient mWebClient;

        private string Address;

        public GeoCode(string Address)
        {
            this.Address = Address;
            Build();
        }

        public async Task Build()
        {
            await Task.Run(() => GetData());
        }

        void GetData()
        {
            mWebClient = new WebClient();
            Uri url = new Uri(string.Format("https://geocode-maps.yandex.ru/1.x/?format=json&results=1&geocode={0}", Address));

            try
            {
                string stringResult = mWebClient.DownloadString(url);
                JObject jsonResult = JObject.Parse(stringResult);
                var pos = jsonResult["response"]["GeoObjectCollection"]["featureMember"][0]["GeoObject"]["Point"]["pos"].Value<string>();
                var lowerCorner = jsonResult["response"]["GeoObjectCollection"]["featureMember"][0]["GeoObject"]["boundedBy"]["Envelope"]["lowerCorner"].Value<string>();
                var upperCorner = jsonResult["response"]["GeoObjectCollection"]["featureMember"][0]["GeoObject"]["boundedBy"]["Envelope"]["upperCorner"].Value<string>();

                if (pos == null) return;

                Pos = GetLatLng(pos);
                LowerCorner = GetLatLng(lowerCorner);
                UpperCorner = GetLatLng(upperCorner);

                Complited?.Invoke(Pos, LowerCorner, UpperCorner);
            }
            catch (Exception e)
            {
                Error?.Invoke(e);
            }
        }

        private LatLng GetLatLng(string pos)
        {
            string[] posArr = pos.Split(' ');
            try
            {
                double latitude = double.Parse(posArr[1]);
                double longitude = double.Parse(posArr[0]);
                return new LatLng(latitude, longitude);
            }
            catch (Exception e)
            {
                pos = pos.Replace('.', ',');
                posArr = pos.Split(' ');
                double latitude = double.Parse(posArr[1]);
                double longitude = double.Parse(posArr[0]);
                return new LatLng(latitude, longitude);
            }
        }
    }
}