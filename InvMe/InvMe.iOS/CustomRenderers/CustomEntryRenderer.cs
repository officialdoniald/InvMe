using BLL.Xamarin.CustomRenderers;
using InvMe.iOS.CustomRenderers;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(CustomEntry), typeof(CustomEntryRenderer))]
namespace InvMe.iOS.CustomRenderers
{
    public class CustomEntryRenderer : EntryRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);

            if (Control != null)
            {
                Control.Layer.BorderColor = UIColor.White.CGColor;
                Control.Layer.BorderWidth = 1f;
                Control.TextColor = UIColor.White;
            }
        }
    }
}