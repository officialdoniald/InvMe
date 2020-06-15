using BLL;
using BLL.Helper;
using BLL.Xamarin.Helper;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace InvMe.View
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class JustActivityIndicator : ContentPage
    {
        private string isEmpty = string.Empty;

        public JustActivityIndicator()
        {
            InitializeComponent();
        }

        public JustActivityIndicator(string facebookOrLogin)
        {
            InitializeComponent();

            isEmpty = facebookOrLogin;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            GlobalVariables.GlobalCasualImage = GlobalVariables.DatabaseConnection.GetGlobalCasualImage();

            Task.Run(() =>
            {
                if (!string.IsNullOrEmpty(isEmpty))
                {
                    LocalFuntionsContainer.InitializeUsersEmailVariable();

                    LocalFuntionsContainer.InitializeUser();

                    Device.BeginInvokeOnMainThread(() => {
                        App.SetRootPage(new MainPage());
                    });
                }
                else
                {
                    Device.BeginInvokeOnMainThread(() => {
                        App.SetRootPage(new MainPage());
                    });
                }
            });
        }
    }
}