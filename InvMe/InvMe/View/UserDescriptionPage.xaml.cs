using BLL;
using BLL.Helper;
using BLL.ViewModel;
using Model;
using System;
using System.Net;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace InvMe.View
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class UserDescriptionPage : ContentPage
	{
        #region Properties

        private User selectedUser = new User();

        #endregion

        public UserDescriptionPage(User selectedUser)
        {
            this.selectedUser = selectedUser;

            InitializeComponent();
            
            var currentWidth = Application.Current.MainPage.Width;

            var optimalWidth = currentWidth / 3;

            profilepictureImage.HeightRequest = optimalWidth;
            profilepictureImage.WidthRequest = optimalWidth;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            profilepictureImage.Source = ImageSource.FromStream(() => new System.IO.MemoryStream(selectedUser.PROFILEPICTURE));

            if (!new UserDescriptionViewModel().IsItABlockedUser(selectedUser.ID))//ha nem blokkolt
            {
                reportButton.IsVisible = false;
                blockButton.IsVisible = false;
                detailStackLayout.IsVisible = true;

                if (selectedUser.ID != GlobalVariables.ActualUser.ID)
                {
                    reportButton.IsVisible = true;
                    blockButton.IsVisible = true;
                    Title = "User description";
                    ToolbarItems.Clear();
                }
                else
                {
                    Title = "My profile";
                }

                firstnameLabel.Text = selectedUser.FIRSTNAME + " " + selectedUser.LASTNAME;

                string old = "-";

                if (GlobalFunctionsContainer.HowOld(selectedUser.BORNDATE) >= 1)
                {
                    old = GlobalFunctionsContainer.HowOld(selectedUser.BORNDATE).ToString();
                }

                bornDateLabel.Text = old + " years old";
                //emailLabel.Text = selectedUser.EMAIL;
            }
            else
            {
                detailStackLayout.IsVisible = false;
                blockedLabel.Text = GlobalVariables.Language.BlockedUser();
                blockedLabel.IsVisible = true;
                blockButton.IsVisible = false;
                reportButton.IsVisible = true;
                ToolbarItems.Clear();
            }
        }
        
        private async void ReportButton_Clicked(object sender, EventArgs e)
        {
            var success = new UserDescriptionViewModel().ReportUser(selectedUser);

            if (success)
            {
                await DisplayAlert("Success", "Thank you very much for the report, we are investigating the user.", "OK");
                await Navigation.PopToRootAsync();
            }
            else
            {
                await DisplayAlert("Failed", "Something went wrong, please check back later!", "OK");
            }
        }

        private async void BlockButton_Clicked(object sender, EventArgs e)
        {
            var success = new BlockedPeopleViewModel().InsertBlockedPeople(
                new Model.BlockedPeople()
                {
                    UserID = GlobalVariables.ActualUser.ID,
                    BlockedUserID = selectedUser.ID
                });

            if (!string.IsNullOrEmpty(success))
                await DisplayAlert(GlobalVariables.Language.Warning(), success, GlobalVariables.Language.OK());
            else
            {
                await DisplayAlert("Success", "Successful blocked!", "OK");
                await Navigation.PopToRootAsync();
            }
        }

        private async void MoreToolbarItem_Activated(object sender, EventArgs e)
        {
            var reported = await DisplayActionSheet("More", "Cancel", null, "Update profile", "Settings");

            if (reported == "Update profile")
            {
                await Navigation.PushAsync(new UpdateProfilePage());
            }
            else if (reported == "Settings")
            {
                await Navigation.PushAsync(new MyAccountPage());
            }
        }
    }
}