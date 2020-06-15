using BLL;
using BLL.Helper;
using BLL.Languages.Regions;
using BLL.Xamarin.Helper;
using InvMe.View;
using Model;
using Plugin.Connectivity;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace InvMe
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            GlobalEvents.OnNoificationClicked += GlobalEvents_OnNoificationClicked;

            GlobalVariables.Language = new English();

            if (!CrossConnectivity.Current.IsConnected)
            {
                while (!CrossConnectivity.Current.IsConnected) { }
            }

            GlobalFunctionsContainer.InitGlobalSettings();
            LocalFuntionsContainer.InitializeUsersEmail();
            LocalFuntionsContainer.InitFirstUserLocationRequestFile();

            if (!GlobalVariables.HaveToLogin)
            {
                LocalFuntionsContainer.InitializeUser();

                MainPage = new View.JustActivityIndicator(); 
            }
            else
            {
                var page = new View.LoginPage();

                MainPage = new NavigationPage(page);

                NavigationPage.SetHasNavigationBar(page, false);
            }
        }

        private void GlobalEvents_OnNoificationClicked(object sender, Events e)
        {
            GlobalVariables.NotificationEvents = e;
            GlobalVariables.NeedToNavigateToEventFromNotification = true;
        }

        public static void DisplayPopUp(string message)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                App.Current.MainPage.DisplayAlert("Failed", message, "OK");
            });
        }

        public static void SetRootPage(Page newRootPage)
        {
            Device.BeginInvokeOnMainThread(() => {
                App.Current.MainPage = new NavigationPage(newRootPage);
            });
        }

        public async static void WriteToSafeStorageTheNotiToken(string token)
        {
            await SecureStorage.SetAsync(GlobalVariables.StoredNotiToken, token);
        }

        public static string ReadFromSafeStorageTheNotiToken()
        {
            return SecureStorage.GetAsync(GlobalVariables.StoredNotiToken).Result;
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            if (GlobalVariables.NeedToNavigateToEventFromNotification)
            {
                Device.BeginInvokeOnMainThread(async () => {
                    await App.Current.MainPage.Navigation.PushAsync(new EventDescriptionPage(GlobalVariables.NotificationEvents));
                });

                GlobalVariables.NeedToNavigateToEventFromNotification = false;
            }
        }
    }
}