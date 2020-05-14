using BLL;
using BLL.Xamarin;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace InvMe.View
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class SettingsPage : ContentPage
	{
		public SettingsPage ()
		{
			InitializeComponent ();

            if (GlobalVariables.AutomaticUserLocation)
            {
                OnAutomaticUserLocation.IsToggled = true;
            }
		}

        private async void OnAutomaticUserLocation_Toggled(object sender, ToggledEventArgs e)
        {
            if (OnAutomaticUserLocation.IsToggled)
            {
                await SecureStorage.SetAsync(LocalVariablesContainer.userlocationfile, 1.ToString());

                GlobalVariables.AutomaticUserLocation = true;
            }
            else
            {
                await SecureStorage.SetAsync(LocalVariablesContainer.userlocationfile, 0.ToString());

                GlobalVariables.AutomaticUserLocation = false;
            }
        }
    }
}