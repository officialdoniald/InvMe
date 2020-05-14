using Plugin.Connectivity;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace InvMe.View
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NoConnection : ContentPage
    {
        private bool wasNotConn = false;

        public NoConnection()
        {
            InitializeComponent();

            CrossConnectivity.Current.ConnectivityChanged += async (sender, args) =>
            {
                if (CrossConnectivity.Current.IsConnected && !wasNotConn)
                {
                    wasNotConn = true;

                    App.SetRootPage(new MainPage());
                }
                else
                {
                    wasNotConn = false;
                }
            };
        }

        public NoConnection(bool isFromLogin)
        {
            CrossConnectivity.Current.ConnectivityChanged += async (sender, args) =>
            {
                if (CrossConnectivity.Current.IsConnected && !wasNotConn)
                {
                    wasNotConn = true;

                    var page = new LoginPage();

                    App.SetRootPage(page);
                }
                else
                {
                    wasNotConn = false;
                }
            };
        }
    }
}