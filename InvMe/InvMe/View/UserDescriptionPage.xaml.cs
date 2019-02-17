using BLL;
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
                }
                else
                {
                    ToolbarItem updateToolbarItem = new ToolbarItem()
                    {
                        Text = "Update",
                        Priority = 0
                    };
                    ToolbarItem settingsToolbarItem = new ToolbarItem()
                    {
                        Text = "Settings",
                        Priority = 0
                    };

                    updateToolbarItem.Clicked += UpdateToolbarItem_Activated;
                    settingsToolbarItem.Clicked += SettingsToolbarItem_Activated;

                    ToolbarItems.Add(updateToolbarItem);
                    ToolbarItems.Add(settingsToolbarItem);
                }

                firstnameLabel.Text = selectedUser.FIRSTNAME + " " + selectedUser.LASTNAME;
                bornDateLabel.Text = selectedUser.BORNDATE.ToString(GlobalVariables.DateFormatForBornDate);
                //emailLabel.Text = selectedUser.EMAIL;
            }
            else
            {
                detailStackLayout.IsVisible = false;
                blockedLabel.Text = GlobalVariables.Language.BlockedUser();
                blockedLabel.IsVisible = true;
                blockButton.IsVisible = false;
                reportButton.IsVisible = true;
            }
        }
        
        private void UpdateToolbarItem_Activated(object sender, System.EventArgs e)
        {
            Navigation.PushAsync(new UpdateProfilePage());
        }

        private void SettingsToolbarItem_Activated(object sender, System.EventArgs e)
        {
            Navigation.PushAsync(new MyAccountPage());
        }

        private async void ReportButton_Clicked(object sender, EventArgs e)
        {
            var success = new UserDescriptionViewModel().ReportUser(selectedUser);

            if (success)
            {
                try
                {
                    //Aki reportolt
                    string url = string.Format("http://invme.hu/php_files/petbelliesreppic.php?email={0}&nev={1}&host={2}", selectedUser.EMAIL, selectedUser.FIRSTNAME, 0);
                    Uri uri = new Uri(url);
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
                    request.Method = "GET";
                    WebResponse res = await request.GetResponseAsync();

                    //Akinek a képe van
                    string url1 = string.Format("http://invme.hu/php_files/petbelliesreppic.php?email={0}&nev={1}&host={2}", GlobalVariables.ActualUsersEmail, GlobalVariables.ActualUser.FIRSTNAME, 1);
                    Uri uri1 = new Uri(url1);
                    HttpWebRequest request1 = (HttpWebRequest)WebRequest.Create(uri1);
                    request.Method = "GET";
                    WebResponse res1 = await request.GetResponseAsync();
                }
                catch (Exception) { }

                await DisplayAlert("Success", "Thanks..", "OK");
                await Navigation.PopToRootAsync();
            }
            else
            {
                await DisplayAlert("Failed", "Something went wrong", "OK");
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
    }
}