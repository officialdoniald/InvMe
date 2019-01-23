using BLL;
using BLL.Languages.Regions;
using BLL.ViewModel;
using Model;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace InvMe.View
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SignUpPage : ContentPage
    {
        public SignUpPage()
        {
            InitializeComponent();
        }

        private async void SignupButton_Clicked(object sender, EventArgs e)
        {
            await Task.Run( async() =>
            {
                SignUpPageViewModel signUpPageViewModel = new SignUpPageViewModel();

                Device.BeginInvokeOnMainThread(() =>
                {
                    signupButton.IsEnabled = false;
                    uploadActivity.IsRunning = true;
                });

                string success = await signUpPageViewModel.SignUp(new User()
                {
                    EMAIL = emailEntry.Text,
                    PASSWORD = passwordEntry.Text,
                    FIRSTNAME = firstnameEntry.Text,
                    LASTNAME = lastnameEntry.Text,
                    PROFILEPICTURE = GlobalVariables.GlobalCasualImage,
                    BORNDATE = DateTime.Now
                });

                if (!string.IsNullOrEmpty(success))
                {
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        DisplayAlert(GlobalVariables.Language.Warning(), success, GlobalVariables.Language.OK());
                        uploadActivity.IsRunning = false;
                    });
                }
                else
                {
                    Device.BeginInvokeOnMainThread(() =>
                       {
                           Navigation.PopToRootAsync();
                       });
                }

                Device.BeginInvokeOnMainThread(() =>
                {
                    signupButton.IsEnabled = true;
                    uploadActivity.IsRunning = false;
                });
            });
        }

        private void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            Navigation.PushAsync(new TermsAndCondPage());
        }

        private void TapGestureRecognizer_Tapped_1(object sender, EventArgs e)
        {
            Navigation.PushAsync(new PrivaciPolicyPage());
        }

        private void Imover13Switch_Toggled(object sender, ToggledEventArgs e)
        {
            if (imover13Switch.IsToggled)
            {
                signupButton.IsEnabled = true;
            }
            else
            {
                signupButton.IsEnabled = false;
            }
        }
    }
}