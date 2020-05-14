using BLL;
using BLL.Xamarin;
using Plugin.Connectivity;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration.AndroidSpecific;

namespace InvMe.View
{
    public partial class MainPage : Xamarin.Forms.TabbedPage
    {
        private bool wasNotConn = false;

        public MainPage()
        {
            InitializeComponent();

            if (Device.OS == TargetPlatform.Android)
            {
                On<Xamarin.Forms.PlatformConfiguration.Android>().SetToolbarPlacement(ToolbarPlacement.Bottom);
                On<Xamarin.Forms.PlatformConfiguration.Android>().SetBarItemColor(Color.Gray);
                On<Xamarin.Forms.PlatformConfiguration.Android>().SetBarSelectedItemColor(Color.Black);
            }

            CrossConnectivity.Current.ConnectivityChanged += async (sender, args) =>
            {
                if (!CrossConnectivity.Current.IsConnected && !wasNotConn)
                {
                    wasNotConn = true;

                    App.SetRootPage(new NoConnection());
                }
                else
                {
                    wasNotConn = false;

                }
            };

            var homePage = new HomePage(new Model.Hashtags());
            var searchPage = new AddEventPage();
            var peopleSearchPage = new JoinedEventPage();
            var uploadPhotoPage = new HashtagsPage();
            var myAccountPage = new UserDescriptionPage(GlobalVariables.ActualUser);

            Device.BeginInvokeOnMainThread(() =>
            {
                NavigationPage.SetHasBackButton(this, false);
                NavigationPage.SetHasNavigationBar(this, false);

                var navigationHomePage = new NavigationPage(homePage);
                navigationHomePage.Icon = LocalVariablesContainer.homepng;
                NavigationPage.SetHasNavigationBar(homePage, true);

                var navigationSearchPage = new NavigationPage(searchPage);
                navigationSearchPage.Icon = LocalVariablesContainer.addpng;
                NavigationPage.SetHasNavigationBar(searchPage, true);

                var navigationPeopleSearchPage = new NavigationPage(peopleSearchPage);
                navigationPeopleSearchPage.Icon = LocalVariablesContainer.calendarpng;
                NavigationPage.SetHasNavigationBar(peopleSearchPage, true);

                var navigationUploadPhotoPage = new NavigationPage(uploadPhotoPage);
                navigationUploadPhotoPage.Icon = LocalVariablesContainer.hashtagspng;
                NavigationPage.SetHasNavigationBar(uploadPhotoPage, true);

                var navigationMyAccountPage = new NavigationPage(myAccountPage);
                navigationMyAccountPage.Icon = LocalVariablesContainer.settingspng;
                NavigationPage.SetHasNavigationBar(myAccountPage, true);

                Children.Add(navigationHomePage);
                Children.Add(navigationSearchPage);
                Children.Add(navigationPeopleSearchPage);
                Children.Add(navigationUploadPhotoPage);
                Children.Add(navigationMyAccountPage);
            });
        }
    }
}