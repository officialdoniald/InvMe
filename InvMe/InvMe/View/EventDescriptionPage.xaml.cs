using BLL;
using BLL.Helper;
using BLL.ViewModel;
using BLL.Xamarin;
using BLL.Xamarin.MapClasses;
using Microsoft.AspNetCore.SignalR.Client;
using Model;
using Plugin.ExternalMaps;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Xaml;

namespace InvMe.View
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EventDescriptionPage : ContentPage
    {
        #region Properties

        ToolbarItem submitordeleteButton = new ToolbarItem();

        Events ThisEvent = new Events();

        Attended attend = new Attended();

        bool isAttended = false;

        Pin ppin = new Pin();

        Pin mpin = new Pin();

        #endregion

        public EventDescriptionPage(Events events)
        {
            InitializeComponent();

            ThisEvent = events;

            isAttendedorNot();

            SeeTheEvent();
        }

        public EventDescriptionPage(Events events, bool isAttended)
        {
            InitializeComponent();

            this.isAttended = isAttended;

            ThisEvent = events;

            SeeTheEvent();
        }

        private async void ConnectToSignalR()
        {
            var hubConnection = new HubConnectionBuilder().WithAutomaticReconnect().WithUrl("http://193.39.13.210" + "/InvMeHub").Build();

            await hubConnection.StartAsync();

            hubConnection.On<int, bool>("ReceiveMessage", (eventid, attend) =>
            {
                var count = Convert.ToInt32(howManyLabel.Text.Split('/')[1]);

                if (attend)
                {
                    count++;
                }
                else
                {
                    count--;
                }

                if (ThisEvent.HOWMANY == 1)
                {
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        howManyLabel.Text = "Anyone" + "/" + count;
                    });
                }
                else
                {
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        howManyLabel.Text = ThisEvent.HOWMANY.ToString() + "/" + count;
                    });
                }

                if (!isAttended && ThisEvent.HOWMANY <= count)
                {
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        ToolbarItems.Clear();
                    });
                }
                else if (isAttended)
                {
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        ToolbarItems.Clear();

                        submitordeleteButton.Clicked += submitordeleteButton_Clicked;
                        ToolbarItems.Add(submitordeleteButton);

                        if (isAttended) submitordeleteButton.Text = "Leave";
                        else submitordeleteButton.Text = "Attend";
                    });
                }
            });

            //async Task SendMessage(string user, string message)
            //{
            //    await hubConnection.InvokeAsync("SendMessage", user, message);
            //}
        }

        private void isAttendedorNot()
        {
            if (new EventDescriptionPageViewModel().GetAttended(ThisEvent.ID))
            {
                attend = GlobalVariables.Attended;
                isAttended = true;
            }
            else
            {
                attend = new Attended();
                isAttended = false;
            }
        }

        private void SeeTheEvent()
        {
            ConnectToSignalR();

            List<Attended> attendedToThisEvent = new EventDescriptionPageViewModel().GetAttendedByEventID(ThisEvent.ID);

            eventNameLabel.Text = ThisEvent.EVENTNAME;

            DateTimeOffset dto_begin = new DateTimeOffset();
            DateTimeOffset dto_end = new DateTimeOffset();
            dto_begin = ThisEvent.FROM.ToLocalTime();
            dto_end = ThisEvent.TO.ToLocalTime();

            startDateDay.Text = dto_begin.ToString("MMM");
            startDateMonth.Text = dto_begin.ToString("dd");
            startDateTime.Text = dto_begin.ToString("h:mm tt");

            if (ThisEvent.FROM == ThisEvent.TO)
            {
                endDateDay.Text = string.Empty;
                endDateMonth.Text = "No matter";
                endDateTime.Text = string.Empty;
            }
            else
            {
                endDateDay.Text = dto_end.ToString("MMM");
                endDateMonth.Text = dto_end.ToString("dd");
                endDateTime.Text = dto_end.ToString("h:mm tt");
            }

            if (ThisEvent.ONLINE == 1)
            {
                eventTownLabel.Text = "Online";

                eventStack.IsVisible = false;
                meetStack.IsVisible = false;
                GetDirectionMeetingButton.IsVisible = false;
                GetDirectionPlaceButton.IsVisible = false;
            }
            else
            {
                eventTownLabel.Text = ThisEvent.TOWN + ", " + ThisEvent.PLACE;

                Position ppos = new Position(Convert.ToDouble(ThisEvent.PLACECORD.Split(';')[0]),
                    Convert.ToDouble(ThisEvent.PLACECORD.Split(';')[1]));
                Position mpos = new Position(Convert.ToDouble(ThisEvent.MEETINGCORD.Split(';')[0]),
                    Convert.ToDouble(ThisEvent.MEETINGCORD.Split(';')[1]));

                ppin = new Pin();
                mpin = new Pin();

                ppin.Position = ppos;
                ppin.Label = ThisEvent.EVENTNAME + ", " + ThisEvent.PLACE;
                ppin.Type = PinType.Place;
                ppin.Address = "";
                mpin.Position = mpos;
                mpin.Label = "Meeting place";
                mpin.Type = PinType.Place;
                mpin.Address = "";

                var pin = new CustomPin
                {
                    Pin = ppin,
                    Id = "Xamarin",
                    Url = "http://invme.Hu"
                };
                var pin2 = new CustomPin
                {
                    Pin = mpin,
                    Id = "Xamarin2",
                    Url = "http://invme.Hu"
                };

                CustomMap eventPlaceMap = new CustomMap()
                {
                    IsShowingUser = true,
                    HeightRequest = 200,
                    isJustShow = true,
                    longitude = pin.Pin.Position.Longitude,
                    latitude = pin.Pin.Position.Latitude
                };
                eventPlaceMap.CustomPins = new List<CustomPin> { pin };
                eventPlaceMap.Pins.Add(pin.Pin);

                CustomMap meetPlaceMap = new CustomMap()
                {
                    IsShowingUser = true,
                    HeightRequest = 200,
                    isJustShow = true,
                    longitude = pin2.Pin.Position.Longitude,
                    latitude = pin2.Pin.Position.Latitude
                };
                meetPlaceMap.CustomPins = new List<CustomPin> { pin2 };
                meetPlaceMap.Pins.Add(pin2.Pin);

                eventStack.Children.Add(eventPlaceMap);
                meetStack.Children.Add(meetPlaceMap);

                eventPlaceMap.MoveToRegion(
                    MapSpan.FromCenterAndRadius(
                    ppos, Distance.FromMiles(1.0)));
                eventPlaceMap.IsShowingUser = true;

                meetPlaceMap.MoveToRegion(
                    MapSpan.FromCenterAndRadius(
                        mpos, Distance.FromMiles(1.0)));
                meetPlaceMap.IsShowingUser = true;
            }

            if (ThisEvent.HOWMANY == 1)
            {
                howManyLabel.Text = "Anyone" + "/" + attendedToThisEvent.Count;
            }
            else
            {
                howManyLabel.Text = ThisEvent.HOWMANY.ToString() + "/" + attendedToThisEvent.Count;
            }
            if (ThisEvent.HOWMANY != 1 && attendedToThisEvent.Count >= ThisEvent.HOWMANY)
            {
                ToolbarItems.Clear();
            }
            else
            {
                submitordeleteButton.Clicked += submitordeleteButton_Clicked;
                ToolbarItems.Add(submitordeleteButton);

                if (isAttended) submitordeleteButton.Text = "Leave";
                else submitordeleteButton.Text = "Attend";
            }
        }

        private async void submitordeleteButton_Clicked(object sender, EventArgs e)
        {
            bool success = false;

            attend = new Attended();
            attend.USERID = GlobalVariables.ActualUser.ID;
            attend.EVENTID = ThisEvent.ID;

            if (isAttended)
            {
                success = new EventDescriptionPageViewModel().DeleteAttended(attend);

                if (success)
                {
                    await DisplayAlert(GlobalVariables.Language.Success(), GlobalVariables.Language.SuccessFulUnSubscibed(), GlobalVariables.Language.OK());

                    await Navigation.PopAsync();
                }
                else
                {
                    await DisplayAlert(GlobalVariables.Language.Warning(), GlobalVariables.Language.SomethingWentWrong(), GlobalVariables.Language.OK());
                }
            }
            else
            {
                success = new EventDescriptionPageViewModel().InsertAttended(attend);

                if (success)
                {
                    await DisplayAlert(GlobalVariables.Language.Success(), GlobalVariables.Language.SuccessFulSubsrcibed(), GlobalVariables.Language.OK());

                    await Navigation.PopAsync();
                }
                else
                {
                    await DisplayAlert(GlobalVariables.Language.Warning(), GlobalVariables.Language.SomethingWentWrong(), GlobalVariables.Language.OK());
                }
            }
        }

        private async void GetDirectionPlaceButton_Clicked(object sender, EventArgs e)
        {
            await CrossExternalMaps.Current.NavigateTo(
                ppin.Label,
                ppin.Position.Latitude,
                ppin.Position.Longitude,
                Plugin.ExternalMaps.Abstractions.NavigationType.Driving
                );
        }

        private void GetDirectionMeetingButton_Clicked(object sender, EventArgs e)
        {
            CrossExternalMaps.Current.NavigateTo(
                mpin.Label,
                mpin.Position.Latitude,
                mpin.Position.Longitude,
                Plugin.ExternalMaps.Abstractions.NavigationType.Driving
                );
        }

        void Handle_Clicked(object sender, System.EventArgs e)
        {
            Navigation.PushAsync(new AttendedUsersListPage(ThisEvent));
        }

        private async void ReportButton_Clicked(object sender, EventArgs e)
        {
            var success = new EventDescriptionPageViewModel().ReportEvent(ThisEvent);

            if (success)
            {
                await DisplayAlert("Success", "Thanks..", "OK");
                await Navigation.PopToRootAsync();
            }
            else
            {
                await DisplayAlert("Failed", "Something went wrong", "OK");
            }
        }
    }
}