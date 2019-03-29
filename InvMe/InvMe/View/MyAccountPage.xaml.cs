using BLL;
using BLL.Helper;
using BLL.ViewModel;
using BLL.Xamarin;
using BLL.Xamarin.FileStoreAndLoad;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace InvMe.View
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class MyAccountPage : ContentPage
	{
		public MyAccountPage ()
		{
			InitializeComponent ();
		}

        private void LogoutButton_Clicked(object sender, EventArgs e)
        {
            DisableEnableButtons(false);

            FileStoreAndLoading.InsertToFile(LocalVariablesContainer.logintxt, String.Empty);

            var page = new LoginPage();

            Navigation.PushModalAsync(new NavigationPage(page));

            DisableEnableButtons(true);
        }

        private async void DeleteButton_Clicked(object sender, EventArgs e)
        {
            var action = await DisplayActionSheet(GlobalVariables.Language.AreYouSure(), GlobalVariables.Language.Delete(), GlobalVariables.Language.Cancel());

            if (action == GlobalVariables.Language.Delete())
            {
                await Task.Run(() =>
                {
                    DisableEnableButtons(false);

                    string success = new MyAccountPageViewModel().DeleteAccount();

                    if (!string.IsNullOrEmpty(success))
                    {
                        Device.BeginInvokeOnMainThread(() =>
                        {
                            DisplayAlert(GlobalVariables.Language.Warning(), success, GlobalVariables.Language.OK());
                        });
                    }
                    else
                    {
                        FileStoreAndLoading.InsertToFile(LocalVariablesContainer.logintxt, string.Empty);

                        GlobalFunctionsContainer.SendEmail("deleteaccount", GlobalVariables.ActualUser.EMAIL, GlobalVariables.ActualUser.FIRSTNAME, GlobalVariables.ActualUser.LASTNAME, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty);

                        var page = new LoginPage();

                        Navigation.PushModalAsync(new NavigationPage(page));
                    }

                    DisableEnableButtons(true);
                });
            }
        }

        private void UpdateButton_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new UpdateProfilePage());
        }

        private void ImpressButton_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new ImpressPage());
        }

        private void DisableEnableButtons(bool state)
        {
            Device.BeginInvokeOnMainThread(()=> {
                //impressButton.IsEnabled = state;
                logoutButton.IsEnabled = state;
                settingsButton.IsEnabled = state;
                deleteActivity.IsRunning = !state;
            });
        }

        private void BlockButton_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new BlockedPeoplePage());
        }

        private void SettingsButton_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new SettingsPage());
        }
    }
}