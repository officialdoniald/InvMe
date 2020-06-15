using Android.App;
using Android.Content.PM;
using Android.Views;
using Android.OS;
using ImageCircle.Forms.Plugin.Droid;
using Android.Runtime;
using System;
using Android;
using Android.Support.V4.App;
using BLL.Helper;
using Android.Content;
using System.Collections.Generic;
using System.Threading.Tasks;
using Android.Gms.Common;
using Android.Util;
using BLL;
using Model;
using Newtonsoft.Json;
using WindowsAzure.Messaging;

namespace InvMe.Droid
{
    [Activity(Label = "InvMe!", Icon = "@drawable/icon", Theme = "@style/MainTheme", MainLauncher = false, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait, LaunchMode = LaunchMode.SingleTop)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            GlobalEvents.OnLogin += GlobalEvents_OnLogin;
            GlobalEvents.OnLogoff += GlobalEvents_OnLogoff;

            base.OnCreate(savedInstanceState);
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;
            Window window = this.Window;
            window.ClearFlags(WindowManagerFlags.TranslucentStatus);
            window.AddFlags(WindowManagerFlags.DrawsSystemBarBackgrounds);
            window.SetStatusBarColor(Android.Graphics.Color.Rgb(112, 196, 152));
            Xamarin.FormsMaps.Init(this, savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            ImageCircleRenderer.Init();

            ActivityCompat.RequestPermissions(this, new String[] { Manifest.Permission.AccessFineLocation }, 1);
            AndroidEvents.OnAndroidThemeChangeNeeded += MyEvents_OnReceived;
            LoadApplication(new App());
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        public override void OnBackPressed() { }

        private void MyEvents_OnReceived(object sender, int id)
        {
            RunOnUiThread(() =>
            {
                SetTheme(id);
            });
        }

        public class AndroidEvents
        {
            public static event EventHandler<int> OnAndroidThemeChangeNeeded;

            public static void OnAndroidThemeChangeNeeded_Event(object sender, int id)
            {
                OnAndroidThemeChangeNeeded?.Invoke(sender, id);
            }
        }
        protected override void OnNewIntent(Intent intent)
        {
            if (intent.Extras != null)
            {
                var message = intent.GetStringExtra("message");

                Events events = JsonConvert.DeserializeObject<Events>(Util.RequestJson("Events/GetEventsByID/" + Convert.ToInt32(message.Split(";")[0])));

                GlobalEvents.OnNoificationClicked_Event(this, events);
            }

            base.OnNewIntent(intent);
        }

        private void GlobalEvents_OnLogoff(object sender, object e)
        {
            var token = App.ReadFromSafeStorageTheNotiToken();

            UpdateTags(token, new List<string>() { "default" });
        }

        private void GlobalEvents_OnLogin(object sender, object e)
        {
            Task.Run(() =>
            {
                if (!IsPlayServiceAvailable())
                {
                    throw new Exception("This device does not have Google Play Services and cannot receive push notifications.");
                }

                CreateNotificationChannel();
            });
        }

        bool IsPlayServiceAvailable()
        {
            int resultCode = GoogleApiAvailability.Instance.IsGooglePlayServicesAvailable(this);
            if (resultCode != ConnectionResult.Success)
            {
                if (GoogleApiAvailability.Instance.IsUserResolvableError(resultCode))
                    Log.Debug(AppConstants.DebugTag, GoogleApiAvailability.Instance.GetErrorString(resultCode));
                else
                {
                    Log.Debug(AppConstants.DebugTag, "This device is not supported");
                }
                return false;
            }
            return true;
        }

        void CreateNotificationChannel()
        {
            // Notification channels are new as of "Oreo".
            // There is no need to create a notification channel on older versions of Android.
            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                var channelName = AppConstants.NotificationChannelName;
                var channelDescription = String.Empty;
                var channel = new NotificationChannel(channelName, channelName, NotificationImportance.Default)
                {
                    Description = channelDescription
                };

                var notificationManager = (NotificationManager)GetSystemService(NotificationService);
                notificationManager.CreateNotificationChannel(channel);
            }

            var token = App.ReadFromSafeStorageTheNotiToken();

            UpdateTags(token, new List<string>() { GlobalVariables.ActualUser.EMAIL });
        }

        public async void UpdateTags(string ID, List<string> gr)
        {
            var notificationManager = (NotificationManager)GetSystemService(NotificationService);

            var hub = new NotificationHub(AppConstants.NotificationHubName, AppConstants.ListenConnectionString, ApplicationContext);
            
            try
            {
                await Task.Run(() => { hub.Unregister(); });
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
            }

            var tags = new List<string>() { };

            foreach (var m in gr)
            {
                tags.Add(m);
            }

            try
            {
                await Task.Run(() =>
                {
                    hub.Register(ID, tags.ToArray());
                });
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
            }
        }
    }
}