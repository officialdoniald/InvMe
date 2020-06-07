using BLL;
using BLL.ViewModel;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace InvMe.View
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class HomePage : ContentPage
	{
        #region Properties

        User user = new User();

        Hashtags hashtag = new Hashtags();

        #endregion

        public HomePage(Hashtags hashtag)
        {
            this.hashtag = hashtag;

            InitializeComponent();

            if (!string.IsNullOrEmpty(hashtag.HASHTAG)) {
                Title = "#" + hashtag.HASHTAG;
            }else {
                Title = "Events"; 
            }          

            GetTheEvents();
        }
        
        private void GetTheEvents()
        {
            Task.Run(()=> {
                Device.BeginInvokeOnMainThread(()=> {
                    
                    eventListView.IsRefreshing = true;

                    List <Events> eventsFromDB = new List<Events>();

                    List<BindableEvent> bindableEventList = new List<BindableEvent>();

                    List<Events> eventList = new List<Events>();

                    eventsFromDB = new HomePageViewModel().GetEvent();

                    if (string.IsNullOrEmpty(hashtag.TOWN))
                    {
                        hashtag.TOWN = "";
                    }
                    if (string.IsNullOrEmpty(hashtag.HASHTAG))
                    {
                        hashtag.HASHTAG = "";
                    }

                    DateTimeOffset dts = DateTimeOffset.Now;

                    foreach (var item in eventsFromDB)
                    {
                        DateTimeOffset dto_begin = item.FROM;
                        DateTimeOffset dto_end = item.TO;
                        dto_begin = dto_begin.ToLocalTime();
                        dto_end = dto_end.ToLocalTime();

                        if (((dts >= dto_begin && dts < dto_end) || (dts < dto_end || (item.FROM == item.TO && dts < dto_begin)) || (dts <= dto_begin && dts > dto_end)))
                        {
                            if (string.IsNullOrEmpty(hashtag.HASHTAG) &&
                                string.IsNullOrEmpty(hashtag.TOWN))
                            {
                                eventList.Add(item);
                            }
                            else if (item.ONLINE != 1)
                            {
                                if (hashtag.TOWN == "Online") {; }
                                else if (!string.IsNullOrEmpty(hashtag.TOWN.ToLower()) &&
                                    string.IsNullOrEmpty(hashtag.HASHTAG) &&
                                  item.TOWN.ToLower().Contains(hashtag.TOWN.ToLower()))
                                {
                                    eventList.Add(item);
                                }
                                else if (string.IsNullOrEmpty(hashtag.TOWN) &&
                                    !string.IsNullOrEmpty(hashtag.HASHTAG.ToLower()) &&
                                  item.EVENTNAME.ToLower().Contains(hashtag.HASHTAG.ToLower()))
                                {
                                    eventList.Add(item);
                                }
                                else if (!string.IsNullOrEmpty(hashtag.TOWN.ToLower()) &&
                                    !string.IsNullOrEmpty(hashtag.HASHTAG.ToLower()) &&
                                  item.EVENTNAME.ToLower().Contains(hashtag.HASHTAG.ToLower()) &&
                                  item.TOWN.ToLower().Contains(hashtag.TOWN.ToLower()))
                                {
                                    eventList.Add(item);
                                }
                            }
                            else if (item.ONLINE == 1)
                            {
                                if (!string.IsNullOrEmpty(hashtag.HASHTAG.ToLower()) &&
                                    !string.IsNullOrEmpty(hashtag.TOWN.ToLower()))
                                {
                                    if (item.EVENTNAME.ToLower().Contains(hashtag.HASHTAG.ToLower()) &&
                                  hashtag.TOWN == "Online")
                                    {
                                        eventList.Add(item);
                                    }
                                }
                                else if ((!string.IsNullOrEmpty(hashtag.HASHTAG.ToLower()) &&
                                    item.EVENTNAME.ToLower().Contains(hashtag.HASHTAG.ToLower())) ||
                                    hashtag.TOWN == "Online")
                                {
                                    eventList.Add(item);
                                }
                            }
                        }
                    }

                    foreach (var item in eventList)
                    {
                        var attendedList = GlobalVariables.DatabaseConnection.GetAttendedByEventID(item.ID).Take(3);

                        List<ImageSource> attendedPictures = new List<ImageSource>();

                        foreach (var attend in attendedList)
                        {
                            var source = ImageSource.FromStream(() => new System.IO.MemoryStream(GlobalVariables.DatabaseConnection.GetUserByID(attend.USERID).PROFILEPICTURE));

                            attendedPictures.Add(source);
                        }

                        BindableEvent bindableEvent = new BindableEvent();

                        bindableEvent.Event = item;

                        DateTimeOffset dto_begin = item.FROM;
                        DateTimeOffset dto_end = item.TO;
                        dto_begin = dto_begin.ToLocalTime();
                        dto_end = dto_end.ToLocalTime();

                        bindableEvent.EVENTNAME = item.EVENTNAME;
                        bindableEvent.FROM = dto_begin.ToString(GlobalVariables.DateFormatForEventsList);
                        bindableEvent.DAY = dto_begin.ToString("dd");
                        bindableEvent.MONTH = dto_begin.ToString("MMM");
                        bindableEvent.TIME = dto_begin.ToString("h:mm tt");
                        bindableEvent.AttendedPictures = attendedPictures;
                        bindableEvent.IsMoreThenThreeAttended = attendedList.Count() > 3 ? true : false;

                        if (item.ONLINE == 1) bindableEvent.TOWN = "Online";
                        else bindableEvent.TOWN = item.TOWN + ", " + item.PLACE;

                        if (item.FROM == item.TO) bindableEvent.TO = "No matter";
                        else bindableEvent.TO = dto_end.ToString(GlobalVariables.DateFormatForEventsList);

                        bindableEventList.Add(bindableEvent);
                    }

                    eventListView.IsRefreshing = false;

                    eventListView.ItemsSource = bindableEventList;

                    if (bindableEventList.Count != 0)
                    {
                        joinedEventNoItemLabel.IsVisible = false;
                    }
                    else
                    {
                        joinedEventNoItemLabel.IsVisible = true;
                    }
                });
            });
        }

        private async void eventListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            Events selectedEvent = ((BindableEvent)eventListView.SelectedItem).Event;

            await Navigation.PushAsync(new EventDescriptionPage(selectedEvent));
        }

        private void onlineSwitch_Toggled(object sender, ToggledEventArgs e)
        {
            if (onlineSwitch.IsToggled)
            {
                hashtag.TOWN = "Online";
                townEntry.IsVisible = !townEntry.IsVisible;
                townImage.IsVisible = !townImage.IsVisible;
            }
            else
            {
                hashtag.TOWN = townEntry.Text;
                townEntry.IsVisible = !townEntry.IsVisible;
                townImage.IsVisible = !townImage.IsVisible;
            }
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            GetHashtag();
            GetTheEvents();
        }

        private Hashtags GetHashtag()
        {
            if (!onlineSwitch.IsToggled) hashtag.TOWN = townEntry.Text;
            if (!string.IsNullOrEmpty(hashtagEntry.Text) ||
                !string.IsNullOrEmpty(townEntry.Text) ||
                hashtag.TOWN == "Online")
            {
                hashtag.HASHTAG = "";
                hashtag.HASHTAG = hashtagEntry.Text;
            }
            if (string.IsNullOrEmpty(hashtagEntry.Text) &&
                string.IsNullOrEmpty(townEntry.Text))
            {
                hashtag = new Hashtags();
            }

            return hashtag;
        }

        private void EventListView_Refreshing(object sender, EventArgs e)
        {
            GetTheEvents();
        }

        private void FilterButton_Clicked(object sender, EventArgs e)
        {
            filterGrid.IsVisible = !filterGrid.IsVisible;
        }
    }
}