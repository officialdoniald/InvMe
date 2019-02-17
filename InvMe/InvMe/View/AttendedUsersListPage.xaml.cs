using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BLL.ViewModel;
using BLL.Xamarin;
using Model;
using Xamarin.Forms;

namespace InvMe.View
{
    public partial class AttendedUsersListPage : ContentPage
    {
        Events ThisEvent = new Events();

        List<Attended> attendedToThisEvent = new List<Attended>();

        public AttendedUsersListPage(Events events)
        {
            ThisEvent = events;

            InitializeComponent();

            RefreshAttendedList();
        }

        private void RefreshAttendedList()
        {
            Task.Run(() => {
                attendedToThisEvent = new AttendedUsersListPageViewModel().GetAttendedByEventID(ThisEvent.ID);

                List<UserWithProfilePictureAndName> attendedUserCollection = new List<UserWithProfilePictureAndName>();

                UserWithProfilePictureAndName attendedUser = new UserWithProfilePictureAndName();

                foreach (var item in attendedToThisEvent)
                {
                    attendedUser = new UserWithProfilePictureAndName();

                    attendedUser.ID = item.USERID;

                    User getUser = new User();

                    getUser = new AttendedUsersListPageViewModel().GetUserByID(item.USERID);

                    attendedUser.PROFILEPICTURE = ImageSource.FromStream(() => new System.IO.MemoryStream(getUser.PROFILEPICTURE));

                    attendedUser.FIRSTNAME = (getUser).FIRSTNAME + " " + (getUser).LASTNAME;

                    attendedUserCollection.Add(attendedUser);
                }

                Device.BeginInvokeOnMainThread(() => {
                    attendedListView.ItemsSource = attendedUserCollection;
                });
            });
        }

        private void AttendedListView_Refreshing(object sender, EventArgs e)
        {
            attendedListView.IsRefreshing = true;

            RefreshAttendedList();

            attendedListView.IsRefreshing = false;
        }

        private void attendedListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            var selectedAttendedUser = ((UserWithProfilePictureAndName)attendedListView.SelectedItem);

            User selectedUser = new User();

            selectedUser = new AttendedUsersListPageViewModel().GetUserByID(selectedAttendedUser.ID);

            Navigation.PushAsync(new UserDescriptionPage(selectedUser));
        }
    }
}