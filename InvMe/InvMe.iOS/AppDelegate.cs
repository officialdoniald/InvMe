using BLL.Helper;
using Foundation;
using ImageCircle.Forms.Plugin.iOS;
using Model;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Threading.Tasks;
using UIKit;
using UserNotifications;
using WindowsAzure.Messaging;
using Xamarin.Forms;

namespace InvMe.iOS
{
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        private NSData _deviceToken;

        private SBNotificationHub Hub { get; set; }

        NSDictionary _launshoptions;

        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            UITabBar.Appearance.BarTintColor = UIColor.White;
            UITabBar.Appearance.TintColor = UIColor.Black;
            UIProgressView.Appearance.TintColor = UIColor.LightTextColor;
            
            Xamarin.FormsMaps.Init();
            global::Xamarin.Forms.Forms.Init();
            ImageCircleRenderer.Init();
            LoadApplication(new App());

            _launshoptions = options;

            GlobalEvents.OnLogin += GlobalEvents_OnLogin;
            GlobalEvents.OnLogoff += GlobalEvents_OnLogoff;

            try
            {
                RegisterForRemoteNotifications();
            }
            catch (Exception)
            {
                App.DisplayPopUp("Sorry, you cant get push notification yet, we are currently working on the problem.");
            }

            return base.FinishedLaunching(app, options);
        }

        private void GlobalEvents_OnLogoff(object sender, object e)
        {
            try
            {
                UpdateHub();
            }
            catch (Exception)
            {

            }
        }

        private void GlobalEvents_OnLogin(object sender, object e)
        {
            try
            {
                UpdateHub();
            }
            catch (Exception)
            {

            }
        }

        public override void OnActivated(UIApplication uiApplication)
        {
            base.OnActivated(uiApplication);

            if (_launshoptions != null && _launshoptions.ContainsKey(UIApplication.LaunchOptionsRemoteNotificationKey))
            {
                var notifications = _launshoptions[UIApplication.LaunchOptionsRemoteNotificationKey] as NSDictionary;
                PresentNotification(notifications);
            }
        }

        void PresentNotification(NSDictionary dict)
        {
            NSDictionary aps = dict.ObjectForKey(new NSString("aps")) as NSDictionary;

            var msg = string.Empty;
            if (aps.ContainsKey(new NSString("alert")))
            {
                try
                {
                    msg = (aps[new NSString("alert")] as NSString).ToString();
                    
                    var splittedeventname = msg.Split(':')[1].TrimStart();
                    splittedeventname = splittedeventname.Replace(" ", "_space_");
                    splittedeventname = splittedeventname.Replace("/", "_per_");

                    Task.Run(()=> {
                        Events events = JsonConvert.DeserializeObject<Events>(Util.RequestJson("CasualRequests/GetLatestEventFromEventname/" + splittedeventname));

                        Device.BeginInvokeOnMainThread(() =>
                        {
                            GlobalEvents.OnNoificationClicked_Event(this, events);
                        });
                    });
                }
                catch (Exception ex)
                {
                    App.DisplayPopUp(ex.StackTrace);
                }
            }
        }

        void RegisterForRemoteNotifications()
        {
            if (UIDevice.CurrentDevice.CheckSystemVersion(10, 0))
            {
                UNUserNotificationCenter.Current.RequestAuthorization(UNAuthorizationOptions.Alert |
                    UNAuthorizationOptions.Sound |
                    UNAuthorizationOptions.Sound,
                    (granted, error) =>
                    {
                        if (granted)
                            InvokeOnMainThread(UIApplication.SharedApplication.RegisterForRemoteNotifications);
                    });
            }
            else if (UIDevice.CurrentDevice.CheckSystemVersion(8, 0))
            {
                var pushSettings = UIUserNotificationSettings.GetSettingsForTypes(
                UIUserNotificationType.Alert | UIUserNotificationType.Badge | UIUserNotificationType.Sound,
                new NSSet());

                UIApplication.SharedApplication.RegisterUserNotificationSettings(pushSettings);
                UIApplication.SharedApplication.RegisterForRemoteNotifications();
            }
            else
            {
                UIRemoteNotificationType notificationTypes = UIRemoteNotificationType.Alert | UIRemoteNotificationType.Badge | UIRemoteNotificationType.Sound;
                UIApplication.SharedApplication.RegisterForRemoteNotificationTypes(notificationTypes);
            }
        }

        public override void RegisteredForRemoteNotifications(UIApplication application, NSData deviceToken)
        {
            Hub = new SBNotificationHub(AppConstants.ListenConnectionString, AppConstants.NotificationHubName);

            _deviceToken = deviceToken;

            UpdateHub();
        }

        private void UpdateHub()
        {
            try
            {
                if (Hub != null)
                {
                    Hub = new SBNotificationHub(AppConstants.ListenConnectionString, AppConstants.NotificationHubName);
                }
            }
            catch (Exception)
            {

            }

            Hub.UnregisterNative((error) => { });

            Hub.UnregisterTemplate("defaultTemplate", (error) => { });

            Hub.UnregisterAll(_deviceToken, (error) =>
            {
                if (error != null)
                {
                    App.DisplayPopUp("Sorry, you cant get push notification yet, we are currently working on the problem.");
                    //App.DisplayPopUp($"Unable to call unregister {error}");
                    //(App.Current.MainPage as MainPage)?.AddMessage(error.ToString());
                    return;
                }

                var tags = new NSSet(AppConstants.SubscriptionTags.ToArray());
                Hub.RegisterNative(_deviceToken, tags, (errorCallback) =>
                {
                    if (errorCallback != null)
                    {
                        //App.DisplayPopUp($"RegisterNativeAsync error: {errorCallback}");
                        App.DisplayPopUp("Sorry, you cant get push notification yet, we are currently working on the problem.");
                    }
                });

                var templateExpiration = DateTime.Now.AddDays(120).ToString(System.Globalization.CultureInfo.CreateSpecificCulture("en-US"));
                Hub.RegisterTemplate(_deviceToken, "defaultTemplate", AppConstants.APNTemplateBody, templateExpiration, tags, (errorCallback) =>
                {
                    if (errorCallback != null)
                    {
                        if (errorCallback != null)
                        {
                            //App.DisplayPopUp($"RegisterTemplateAsync error: {errorCallback}");
                            App.DisplayPopUp("Sorry, you cant get push notification yet, we are currently working on the problem.");
                        }
                    }
                });
            });
        }

        public override void ReceivedRemoteNotification(UIApplication application, NSDictionary userInfo)
        {
            ProcessNotification(userInfo, false);
        }

        // Check to see if the dictionary has the aps key.  This is the notification payload you would have sent
        void ProcessNotification(NSDictionary options, bool fromFinishedLaunching)
        {
            // make sure we have a payload
            if (options != null && options.ContainsKey(new NSString("aps")))
            {
                // get the APS dictionary and extract message payload. Message JSON will be converted
                // into a NSDictionary so more complex payloads may require more processing
                NSDictionary aps = options.ObjectForKey(new NSString("aps")) as NSDictionary;
                string payload = string.Empty;
                NSString payloadKey = new NSString("alert");
                if (aps.ContainsKey(payloadKey))
                {
                    payload = aps[payloadKey].ToString();
                }

                if (!string.IsNullOrWhiteSpace(payload))
                {
                    //(App.Current.MainPage as MainPage)?.AddMessage(payload);
                }
            }
            else
            {
                App.DisplayPopUp("Received request to process notification but there was no payload.");
                //(App.Current.MainPage as MainPage)?.AddMessage();
            }
        }
    }
}