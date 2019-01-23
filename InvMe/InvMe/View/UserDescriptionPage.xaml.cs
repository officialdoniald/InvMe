using BLL;
using Model;
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

            if (selectedUser.ID != GlobalVariables.ActualUser.ID)
            {
                NavigationPage.SetHasNavigationBar(this, false);
            }

            var currentWidth = Application.Current.MainPage.Width;

            var optimalWidth = currentWidth / 3;

            profilepictureImage.HeightRequest = optimalWidth;
            profilepictureImage.WidthRequest = optimalWidth;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            profilepictureImage.Source = ImageSource.FromStream(() => new System.IO.MemoryStream(selectedUser.PROFILEPICTURE));
            firstnameLabel.Text = selectedUser.FIRSTNAME + " " + selectedUser.LASTNAME;
            bornDateLabel.Text = selectedUser.BORNDATE.ToString(GlobalVariables.DateFormatForBornDate);

            //emailLabel.Text = selectedUser.EMAIL;
        }

        private void UpdateToolbarItem_Activated(object sender, System.EventArgs e)
        {
            Navigation.PushAsync(new UpdateProfilePage());
        }

        private void SettingsToolbarItem_Activated(object sender, System.EventArgs e)
        {
            Navigation.PushAsync(new MyAccountPage());
        }
    }
}