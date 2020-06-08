using System.ComponentModel;
using Android.Content;
using Android.Graphics.Drawables;
using Android.Views;
using BLL.Xamarin.CustomRenderers;
using InvMe.Droid.CustomRenderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(ExtendedViewCell), typeof(ExtendedViewCellRenderer))]
namespace InvMe.Droid.CustomRenderers
{
    public class ExtendedViewCellRenderer : ViewCellRenderer
    {

        private Android.Views.View _cellCore;
        private Drawable _unselectedBackground;
        private bool _selected;

        protected override Android.Views.View GetCellCore(Cell item, Android.Views.View convertView, ViewGroup parent, Context context)
        {
            var _cellCore = base.GetCellCore(item, convertView, parent, context);

            var extendedViewCell = (ExtendedViewCell)item;

            MainActivity.AndroidEvents.OnAndroidThemeChangeNeeded_Event(null, Resource.Style.MainTransparentTheme_Base);

            return _cellCore;
        }

        protected override void OnCellPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            base.OnCellPropertyChanged(sender, args);

            var extendedViewCell = (ExtendedViewCell)sender;

            MainActivity.AndroidEvents.OnAndroidThemeChangeNeeded_Event(null, Resource.Style.MainTransparentTheme_Base);
        }
    }
}