using Foundation;
using ImageCircle.Forms.Plugin.iOS;
using UIKit;

namespace InvMe.iOS
{
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            UITabBar.Appearance.BarTintColor = UIColor.White;
            UITabBar.Appearance.TintColor = UIColor.Black;
            UIProgressView.Appearance.TintColor = UIColor.LightTextColor;
            
            Xamarin.FormsMaps.Init();
            global::Xamarin.Forms.Forms.Init();
            ImageCircleRenderer.Init();
            LoadApplication(new App());

            return base.FinishedLaunching(app, options);
        }
    }
}