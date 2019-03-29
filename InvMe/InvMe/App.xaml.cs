using BLL;
using BLL.Helper;
using BLL.Languages.Regions;
using BLL.Xamarin.Helper;
using Plugin.Connectivity;
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
            // Handle when your app resumes
        }
    }
}