using BLL;
using BLL.ViewModel;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace InvMe.View
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ForgotPasswordPage : ContentPage
	{
		public ForgotPasswordPage ()
		{
			InitializeComponent ();
        }

        private void sendNewPassword_ClickedAsync(object sender, EventArgs e)
        {
            Task.Run(async ()=> {

                Device.BeginInvokeOnMainThread(()=> {
                    loginActivator.IsRunning = true;
                    sendNewPassword.IsEnabled = false;
                });

                var success = await new ForgotPasswordPageViewModel().SendEmailAsync(emailEntry.Text);

                if (String.IsNullOrEmpty(success))
                {
                    Device.BeginInvokeOnMainThread(() => {
                        Navigation.PopToRootAsync();
                    });
                }
                else
                {
                    Device.BeginInvokeOnMainThread(() => {
                        DisplayAlert(GlobalVariables.Language.Warning(), success, GlobalVariables.Language.OK());
                    });
                }

                Device.BeginInvokeOnMainThread(() => {
                    loginActivator.IsRunning = false;
                    sendNewPassword.IsEnabled = true;
                });
            });
        }

        void Handle_Completed(object sender, System.EventArgs e)
        {
            sendNewPassword_ClickedAsync(this, new EventArgs());
        }
    }
}