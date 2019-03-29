using BLL;
using BLL.Xamarin;
using BLL.Xamarin.FileStoreAndLoad;
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

        private void OnAutomaticUserLocation_Toggled(object sender, ToggledEventArgs e)
        {
            if (OnAutomaticUserLocation.IsToggled)
            {
                FileStoreAndLoading.InsertToFile(LocalVariablesContainer.userlocationfile, 1.ToString());

                GlobalVariables.AutomaticUserLocation = true;
            }
            else
            {
                FileStoreAndLoading.InsertToFile(LocalVariablesContainer.userlocationfile, 0.ToString());

                GlobalVariables.AutomaticUserLocation = false;
            }
        }
    }
}