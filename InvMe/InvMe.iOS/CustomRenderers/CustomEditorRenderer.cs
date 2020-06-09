using InvMe.iOS.CustomRenderers;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(Editor), typeof(CustomEditorRenderer))]
namespace InvMe.iOS.CustomRenderers
{
    public class CustomEditorRenderer : EditorRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Editor> e)
        {
            base.OnElementChanged(e);

            if (Control != null)
            {
                Control.Layer.BorderColor = UIColor.Black.CGColor;
                Control.Layer.BorderWidth = 1f;
                Control.Layer.CornerRadius = 10;
                Control.TextColor = UIColor.Black;
            }
        }
    }
}