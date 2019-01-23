using BLL;
using BLL.ViewModel;
using BLL.Xamarin;
using BLL.Xamarin.FileStoreAndLoad;
using Plugin.Media;
using System;
using System.IO;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace InvMe.View
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class UpdateProfilePage : ContentPage
	{
        private bool addedPhoto = false;
        private Stream f;

        public UpdateProfilePage ()
		{
			InitializeComponent ();

            Device.BeginInvokeOnMainThread(() => {
                lastnameEntry.Placeholder = GlobalVariables.ActualUser.LASTNAME;
                firstnameEntry.Placeholder = GlobalVariables.ActualUser.FIRSTNAME;
                emailEntry.Placeholder = GlobalVariables.ActualUser.EMAIL;
                bornPicker.Date = GlobalVariables.ActualUser.BORNDATE;
                profilePictureImage.Source = ImageSource.FromStream(() => new MemoryStream(GlobalVariables.ActualUser.PROFILEPICTURE));
            });
        }

        private async void GalleryButton_Clicked(object sender, System.EventArgs e)
        {
            if (!CrossMedia.Current.IsPickPhotoSupported)
            {
                await DisplayAlert(GlobalVariables.Language.Warning(), GlobalVariables.Language.NoPickingFromGallery(), GlobalVariables.Language.OK());
                return;
            }

            var file = await CrossMedia.Current.PickPhotoAsync();

            if (file == null) return;

            addedPhoto = true;
            f = file.GetStream();
            GlobalVariables.SourceSelectedImageFromGallery = file.Path;
            GlobalVariables.SelectedImageFromGallery = f;

            profilePictureImage.Source = ImageSource.FromStream(() => f);
        }

        private void UpdateProfilePictureButton_Clicked(object sender, EventArgs e)
        {
            Task.Run(() =>
            {
                DisableOrEnableButtons(false);

                string success = new UpdateProfilePageViewModel().UpdateProfilePicture(addedPhoto);

                if (!string.IsNullOrEmpty(success))
                {
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        DisplayAlert(GlobalVariables.Language.Warning(), success, GlobalVariables.Language.OK());
                    });
                }
                else
                {
                    addedPhoto = false;

                    Device.BeginInvokeOnMainThread(() =>
                    {
                        DisplayAlert(GlobalVariables.Language.Success(), GlobalVariables.Language.SuccessFulUpdatedProfile(), GlobalVariables.Language.OK());
                    });
                }

                DisableOrEnableButtons(true);
            });
        }

        private void UpdatePasswordButton_Clicked(object sender, System.EventArgs e)
        {
            Task.Run(() =>
            {
                DisableOrEnableButtons(false);

                string success = new UpdateProfilePageViewModel().UpdatePassword(originalpasswordEntry.Text, passwordEntry.Text);

                if (!string.IsNullOrEmpty(success))
                {
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        DisplayAlert(GlobalVariables.Language.Warning(), success, GlobalVariables.Language.OK());
                    });
                }
                else
                {
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        DisplayAlert(GlobalVariables.Language.Success(), GlobalVariables.Language.SuccessFulUpdatedProfile(), GlobalVariables.Language.OK());
                    });
                }

                DisableOrEnableButtons(true);
            });
        }

        private void UpdateEmail_Clicked(object sender, System.EventArgs e)
        {
            Task.Run(async () =>
            {
                DisableOrEnableButtons(false);

                string success = await new UpdateProfilePageViewModel().UpdateEmailAsync(emailEntry.Text);

                if (!String.IsNullOrEmpty(success))
                {
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        DisplayAlert(GlobalVariables.Language.Warning(), success, GlobalVariables.Language.OK());
                    });
                }
                else
                {
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        FileStoreAndLoading.InsertToFile(LocalVariablesContainer.logintxt, emailEntry.Text);

                        DisplayAlert(GlobalVariables.Language.Success(), GlobalVariables.Language.SuccessFulUpdatedProfile(), GlobalVariables.Language.OK());
                    });
                }

                DisableOrEnableButtons(true);
            });
        }

        private void UpdateProfil_Clicked(object sender, System.EventArgs e)
        {
            Task.Run(() =>
            {
                DisableOrEnableButtons(false);

                string success = new UpdateProfilePageViewModel().UpdateProfile(firstnameEntry.Text, lastnameEntry.Text, bornPicker.Date);

                if (!String.IsNullOrEmpty(success))
                {
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        DisplayAlert(GlobalVariables.Language.Warning(), success, GlobalVariables.Language.OK());
                    });
                }
                else
                {
                    GlobalVariables.IsUpdatedMyProfile = true;

                    Device.BeginInvokeOnMainThread(() =>
                    {
                        DisplayAlert(GlobalVariables.Language.Success(), GlobalVariables.Language.SuccessFulUpdatedProfile(), GlobalVariables.Language.OK());
                    });
                }

                DisableOrEnableButtons(true);
            });
        }

        private void DisableOrEnableButtons(bool enable)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                updateProfileActivator.IsRunning = !enable;
                galleryButton.IsEnabled = enable;
                updateProfilePictureButton.IsEnabled = enable;
                updatePasswordButton.IsEnabled = enable;
                updateEmail.IsEnabled = enable;
                updateProfil.IsEnabled = enable;
            });
        }
    }
}