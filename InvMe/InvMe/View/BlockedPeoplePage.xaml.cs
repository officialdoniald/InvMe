using BLL;
using BLL.ViewModel;
using BLL.Xamarin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace InvMe.View
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BlockedPeoplePage : ContentPage
    {
        public BlockedPeoplePage()
        {
            InitializeComponent();

            InitializeBlockedUserList();
        }

        private void userListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            var listView = (ListView)sender;

            var selectedLVWPAST = (UserWithProfilePictureAndName)listView.SelectedItem;

            var searchResultPage = new UserDescriptionPage(selectedLVWPAST.user);

            Navigation.PushAsync(searchResultPage);
        }

        private void userListView_Refreshing(object sender, EventArgs e)
        {
            InitializeBlockedUserList();
        }

        private async Task InitializeBlockedUserList()
        {
            await Task.Run(() => {
                Device.BeginInvokeOnMainThread(() => {
                    userListView.ItemsSource = null;
                });

                List<UserWithProfilePictureAndName> listViewWithPictureAndSomeText = new List<UserWithProfilePictureAndName>();

                var blockedUserList = new BlockedPeopleViewModel().GetBlockedPeoples();

                if (blockedUserList != null && blockedUserList.Count != 0)
                {
                    foreach (var item in blockedUserList)
                    {
                        var blockedUser = GlobalVariables.DatabaseConnection.GetUserByID(item.BlockedUserID);

                        UserWithProfilePictureAndName listViewWith = new UserWithProfilePictureAndName()
                        {
                            user = GlobalVariables.DatabaseConnection.GetUserByID(blockedUser.ID),
                            FIRSTNAME = blockedUser.LASTNAME + " " + blockedUser.FIRSTNAME,
                            blockedPeople = item
                        };

                        listViewWith.PROFILEPICTURE = ImageSource.FromStream(() => new System.IO.MemoryStream(blockedUser.PROFILEPICTURE));

                        listViewWithPictureAndSomeText.Add(listViewWith);
                    }
                }

                Device.BeginInvokeOnMainThread(() => {
                    userListView.ItemsSource = listViewWithPictureAndSomeText;

                    userListView.IsRefreshing = false;

                    if (listViewWithPictureAndSomeText.Count != 0)
                    {
                        joinedEventNoItemLabel.IsVisible = false;
                    }
                });
            });
        }

        public void OnDelete(object sender, EventArgs e)
        {
            var mi = ((MenuItem)sender);

            var cp = ((UserWithProfilePictureAndName)mi.CommandParameter);

            var success = new BlockedPeopleViewModel().DeleteBlockedPeople(cp.blockedPeople);

            if (!string.IsNullOrEmpty(success))
                DisplayAlert(GlobalVariables.Language.Warning(), success, GlobalVariables.Language.OK());
            else InitializeBlockedUserList();
        }
    }
}