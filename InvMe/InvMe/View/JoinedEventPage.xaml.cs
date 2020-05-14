using BLL.ViewModel;
using Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace InvMe.View
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class JoinedEventPage : ContentPage
    {
        private List<BindableEvent> bindableEventList = new List<BindableEvent>();

        public JoinedEventPage()
        {
            InitializeComponent();
        }

        private void EventListView_Refreshing(object sender, EventArgs e)
        {
            eventListView.IsRefreshing = true;

            GetTheEvents();

            eventListView.IsRefreshing = false;
        }

        private void GetTheEvents()
        {
            Task.Run(() =>
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    eventListView.ItemsSource = null;

                    bindableEventList = new List<BindableEvent>();

                    bindableEventList = new JoinedEventPageViewModel().GetAttendedEvents();

                    eventListView.ItemsSource = bindableEventList;

                    if (bindableEventList.Count != 0)
                    {
                        joinedEventNoItemLabel.IsVisible = false;
                    }
                });
            });
        }

        private async void eventListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            Events ID = ((BindableEvent)eventListView.SelectedItem).Event;

            await Navigation.PushAsync(new EventDescriptionPage(ID, true));
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            GetTheEvents();
        }
    }
}