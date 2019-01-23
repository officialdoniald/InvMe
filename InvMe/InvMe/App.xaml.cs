using BLL;
using BLL.Helper;
using BLL.Languages.Regions;
using BLL.Xamarin;
using BLL.Xamarin.Helper;
using InvMe.View;
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

            //LocalVariablesContainer.NormalLabel = (Style)Resources["NormalLabel"];
            //LocalVariablesContainer.NavigationPageStyle = (Style)Resources["NavigationPageStyle"];

            GlobalVariables.Language = new English();

            if (!CrossConnectivity.Current.IsConnected)
            {
                while (!CrossConnectivity.Current.IsConnected) { }
            }

            GlobalFunctionsContainer.InitGlobalSettings();
            LocalFuntionsContainer.InitializeUsersEmail();

            if (!GlobalVariables.HaveToLogin)
            {
                LocalFuntionsContainer.InitializeUser();

                MainPage = new View.JustActivityIndicator(); 
            }
            else
            {
                var page = new View.LoginPage();

                MainPage = new NavigationPage(page);
                //{
                //    Style = GlobalVariables.NavigationPageStyle
                //};

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