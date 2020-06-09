using BLL;
using BLL.ViewModel;
using ImageCircle.Forms.Plugin.Abstractions;
using Microsoft.AspNetCore.SignalR.Client;
using Model;
using Model.UI;
using Plugin.ExternalMaps;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
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

        List<Attended> attendedToThisEvent = new List<Attended>();

        readonly EventDescriptionPageViewModel mvvm = new EventDescriptionPageViewModel();

        public ObservableCollection<AttendedProfilePicUI> AttendedProfilePicUIs = new ObservableCollection<AttendedProfilePicUI>();

        readonly Events ThisEvent = new Events();

        Attended attend = new Attended();

        bool isAttended = false;

        int howMany = 0;

        User owner = new User();

        Pin ppin = new Pin();

        Pin mpin = new Pin();

        #endregion

        public EventDescriptionPage(Events events)
        {
            InitializeComponent();

            ThisEvent = events;

            IsAttendedorNot();

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
                if (eventid == ThisEvent.ID)
                {
                    var count = Convert.ToInt32(howManyLabel.Text.Split('/')[1]);

                    if (attend)
                    {
                        howMany++;
                        count++;
                    }
                    else
                    {
                        howMany--;
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

                    InitAttendedUserProfilePicList();
                }
            });
        }

        private async Task IsAttendedorNot()
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

        private async Task SeeTheEvent()
        {
            ConnectToSignalR();

            await Task.Run(async () =>
            {
                owner = mvvm.GetUserByID(ThisEvent.CREATEUID);

                Device.BeginInvokeOnMainThread(() =>
                {
                    ownerProfileImage.Source = ImageSource.FromStream(() => new System.IO.MemoryStream(owner.PROFILEPICTURE));
                    ownerNameLabel.Text = owner.FIRSTNAME + " " + owner.LASTNAME;
                });

                await InitAttendedUserProfilePicList();

                Device.BeginInvokeOnMainThread(() =>
                {
                    eventNameLabel.Text = ThisEvent.EVENTNAME;
                });

                DateTimeOffset dto_begin = new DateTimeOffset();
                DateTimeOffset dto_end = new DateTimeOffset();
                dto_begin = ThisEvent.FROM.ToLocalTime();
                dto_end = ThisEvent.TO.ToLocalTime();

                Device.BeginInvokeOnMainThread(() =>
                {
                    startDateDay.Text = dto_begin.ToString("MMM");
                    startDateMonth.Text = dto_begin.ToString("dd");
                    startDateTime.Text = dto_begin.ToString("h:mm tt");

                    if (ThisEvent.FROM == ThisEvent.TO)
                    {
                        hyphenLabel.IsVisible = false;
                        endDateLabel.IsVisible = false;
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

                        meetlabel.IsVisible = false;
                        eventlabel.IsVisible = false;
                        eventStack.IsVisible = false;
                        meetStack.IsVisible = false;
                        GetDirectionMeetingButton.IsVisible = false;
                        GetDirectionPlaceButton.IsVisible = false;
                    }
                    else
                    {
                        meetlabel.IsVisible = true;
                        eventlabel.IsVisible = true;
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

                        Map eventPlaceMap = new Map()
                        {
                            IsShowingUser = true,
                            HeightRequest = 200
                        };
                        eventPlaceMap.Pins.Add(ppin);

                        Map meetPlaceMap = new Map()
                        {
                            IsShowingUser = true,
                            HeightRequest = 200
                        };

                        meetPlaceMap.Pins.Add(mpin);

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

                    if (string.IsNullOrEmpty(ThisEvent.DESCRIPTION))
                    {
                        descriptionLabel.Text = "-";
                    }
                    else
                    {
                        descriptionLabel.Text = ThisEvent.DESCRIPTION;
                    }

                    howMany = attendedToThisEvent.Count;

                    if (ThisEvent.HOWMANY == 1)
                    {
                        howManyLabel.Text = "Anyone" + "/" + attendedToThisEvent.Count;
                    }
                    else
                    {
                        howManyLabel.Text = ThisEvent.HOWMANY.ToString() + "/" + attendedToThisEvent.Count;
                    }
                });
            });
        }

        private async Task InitAttendedUserProfilePicList()
        {
            await Task.Run(() =>
            {
                attendedToThisEvent = mvvm.GetAttendedByEventID(ThisEvent.ID);

                Device.BeginInvokeOnMainThread(() =>
                {
                    AttendedProfilePicUIs = new ObservableCollection<AttendedProfilePicUI>();

                    if (attendedToThisEvent.Count > 5)
                    {
                        listAttendedMembersFrame.IsVisible = true;
                    }
                    else
                    {
                        listAttendedMembersFrame.IsVisible = false;
                    }
                });

                foreach (var item in attendedToThisEvent.Take(5))
                {
                    var user = mvvm.GetUserByID(item.USERID);

                    Device.BeginInvokeOnMainThread(() =>
                    {
                        AttendedProfilePicUIs.Add(new AttendedProfilePicUI()
                        {
                            User = user,
                            ProfilePicture = ImageSource.FromStream(() => new System.IO.MemoryStream(user.PROFILEPICTURE))
                        });
                    });
                }

                Device.BeginInvokeOnMainThread(() =>
                {
                    BindableLayout.SetItemsSource(AttendedIconsListStacklayout, AttendedProfilePicUIs);
                });
            });
        }

        private async void SubmitOrDelete()
        {
            attend = new Attended();
            attend.USERID = GlobalVariables.ActualUser.ID;
            attend.EVENTID = ThisEvent.ID;

            bool success;
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
                Plugin.ExternalMaps.Abstractions.NavigationType.Driving);
        }

        private void GetDirectionMeetingButton_Clicked(object sender, EventArgs e)
        {
            CrossExternalMaps.Current.NavigateTo(
                mpin.Label,
                mpin.Position.Latitude,
                mpin.Position.Longitude,
                Plugin.ExternalMaps.Abstractions.NavigationType.Driving);
        }

        private async void MoreToolbarItem_Activated(object sender, EventArgs e)
        {
            string[] moreMenuItem = new string[] { ThisEvent.HOWMANY != 1 && howMany >= ThisEvent.HOWMANY ? "" : isAttended ? "Leave" : "Attend", "Report event" };

            var reported = await DisplayActionSheet("More", "Cancel", null, moreMenuItem);

            if (reported == "Report event")
            {
                Report();
            }
            else if (reported == "Leave" || reported == "Attend")
            {
                if (ThisEvent.CREATEUID == GlobalVariables.ActualUser.ID)
                {
                    await DisplayAlert(GlobalVariables.Language.Warning(), "You can't leave your event!", GlobalVariables.Language.OK());
                }
                else
                {
                    SubmitOrDelete();
                }
            }
        }

        private async void Report()
        {
            var success = new EventDescriptionPageViewModel().ReportEvent(ThisEvent);

            if (success)
            {
                await DisplayAlert("Success", "Thank you very much for the report, we are investigating the event.", "OK");
                await Navigation.PopToRootAsync();
            }
            else
            {
                await DisplayAlert("Failed", "Something went wrong, please check back later!", "OK");
            }
        }

        private void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            Navigation.PushAsync(new UserDescriptionPage(owner));
        }

        private void TapGestureRecognizer_Tapped_1(object sender, EventArgs e)
        {
            Navigation.PushAsync(new UserDescriptionPage(((AttendedProfilePicUI)((CircleImage)sender).BindingContext).User));
        }

        private void TapGestureRecognizer_Tapped_2(object sender, EventArgs e)
        {
            Navigation.PushAsync(new AttendedUsersListPage(ThisEvent));
        }
    }
}