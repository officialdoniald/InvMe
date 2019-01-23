using BLL;
using BLL.ViewModel;
using BLL.Xamarin.MapClasses;
using Model;
using Plugin.Geolocator;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Xaml;

namespace InvMe.View
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddEventPage : ContentPage
    {
        #region Properties
        
        private bool noMatterHowLong = false;
        private bool onlineEvent = false;
        private bool noMatterHowManyPerson = false;

        Plugin.Geolocator.Abstractions.Position position = new Plugin.Geolocator.Abstractions.Position();

        private Plugin.Geolocator.Abstractions.IGeolocator locator;

        private CustomMap map;
        private CustomMap map2;

        #endregion

        public AddEventPage()
        {
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            Initialize();
        }

        private void Initialize()
        {
            GlobalVariables.EventCord = "";
            GlobalVariables.MeetingCord = "";

            noMatterHowLong = false;
            onlineEvent = false;
            noMatterHowManyPerson = false;

            InitializeComponent();

            locator = CrossGeolocator.Current;
            locator.DesiredAccuracy = 50;
            whenbegindatePicker.Format = "d/M/yyyy";
            beginclock.Time = DateTimeOffset.Now.TimeOfDay;
            beginclock.Format = "h:mm tt";
            enddatePicker.Format = "d/M/yyyy";
            endclock.Time = DateTimeOffset.Now.TimeOfDay;
            endclock.Format = "h:mm tt";

            var today = DateTime.Now;

            var tomorrow = today.AddDays(60);

            whenbegindatePicker.MinimumDate = today;
            whenbegindatePicker.MaximumDate = tomorrow;

            enddatePicker.MinimumDate = today;
            enddatePicker.MaximumDate = tomorrow;

            if (Device.OS == TargetPlatform.iOS)
            {
                map = new CustomMap()
                {
                    WidthRequest = 320,
                    HeightRequest = 200,
                    MapType = MapType.Street,
                    IsShowingUser = true,
                    kind = "event"
                };

                map2 = new CustomMap()
                {
                    WidthRequest = 320,
                    HeightRequest = 200,
                    MapType = MapType.Street,
                    IsShowingUser = true,
                    kind = "meeting"
                };

                map2.CustomPins = new List<CustomPin> { };

                map.CustomPins = new List<CustomPin> { };

                eventStack.Children.Add(map);

                meetStack.Children.Add(map2);

                map.isJustShow = false;
                map2.isJustShow = false;
            }
            else
            {
                Task.Run(async () => {
                    position = await locator.GetPositionAsync();
                }).ContinueWith(delegate
                {
                    GetUserLocation();
                });
            }
        }

        private void DiableEnableButtons(bool state)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                addActivityIndicator.IsRunning = !state;
                submitButton.IsEnabled = state;
            });
        }

        private void GetUserLocation()
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                map = new CustomMap()
                {
                    WidthRequest = 320,
                    HeightRequest = 200,
                    MapType = MapType.Street,
                    longitude = position.Longitude,
                    latitude = position.Latitude,
                    kind = "event"
                };

                map2 = new CustomMap()
                {
                    WidthRequest = 320,
                    HeightRequest = 200,
                    MapType = MapType.Street,
                    longitude = position.Longitude,
                    latitude = position.Latitude,
                    kind = "meeting"
                };

                map.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(position.Latitude, position.Longitude), Distance.FromMeters(300)));
                map2.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(position.Latitude, position.Longitude), Distance.FromMeters(300)));

                map2.CustomPins = new List<CustomPin> { };

                map.CustomPins = new List<CustomPin> { };

                eventStack.Children.Add(map);

                meetStack.Children.Add(map2);

                map.isJustShow = false;
                map2.isJustShow = false;
            });
        }

        private void NomatterHowLong_Toggled(object sender, ToggledEventArgs e)
        {
            toStackLayout.IsVisible = !toStackLayout.IsVisible;
            noMatterHowLong = !noMatterHowLong;
        }

        private void OnlineEventSwitch_Toggled(object sender, ToggledEventArgs e)
        {
            mapStackLayout.IsVisible = !mapStackLayout.IsVisible;
            onlineEvent = !onlineEvent;
        }

        private void EnyoneSwitch_Toggled(object sender, ToggledEventArgs e)
        {
            howmanypersonStackLayout.IsVisible = !howmanypersonStackLayout.IsVisible;
            noMatterHowManyPerson = !noMatterHowManyPerson;
        }

        private void DisabelEnableButtons(bool state)
        {
            addActivityIndicator.IsRunning = !state;
            submitButton.IsEnabled = state;
        }

        private void SubmitButton_Clicked(object sender, EventArgs e)
        {
            Task.Run(() =>
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    DisabelEnableButtons(false);
                });

                Events eventCreateObject = new Events();

                eventCreateObject.FROM = (whenbegindatePicker.Date + beginclock.Time).ToUniversalTime();

                eventCreateObject.EVENTNAME = eventNameEntry.Text;

                if (noMatterHowLong)
                {
                    eventCreateObject.TO = eventCreateObject.FROM;
                }
                else
                {
                    eventCreateObject.TO = (enddatePicker.Date + endclock.Time).ToUniversalTime();
                }

                if (onlineEvent)
                {
                    eventCreateObject.PLACECORD = "";
                    eventCreateObject.MEETINGCORD = "";
                    eventCreateObject.TOWN = "";
                    eventCreateObject.PLACE = "";
                    eventCreateObject.ONLINE = 1;
                }
                else
                {
                    eventCreateObject.PLACECORD = GlobalVariables.EventCord;
                    eventCreateObject.MEETINGCORD = GlobalVariables.MeetingCord;
                    eventCreateObject.TOWN = eventTownEntry.Text;
                    eventCreateObject.PLACE = eventPlaceEntry.Text;
                    eventCreateObject.ONLINE = 0;
                }

                if (noMatterHowManyPerson)
                {
                    eventCreateObject.HOWMANY = 1;
                }

                string success = new AddEventPageViewModel().AddEvent(eventCreateObject, onlineEvent, noMatterHowManyPerson, howmanyPersonEntry.Text);

                if (string.IsNullOrEmpty(success))
                {
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        DisplayAlert(GlobalVariables.Language.Success(), GlobalVariables.Language.SuccessAddedd(), GlobalVariables.Language.OK());

                        Navigation.PushAsync(new HomePage(new Hashtags()));
                    });
                }
                else
                {
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        DisplayAlert(GlobalVariables.Language.Warning(), success, GlobalVariables.Language.OK());
                    });
                }

                Device.BeginInvokeOnMainThread(() =>
                {
                    DisabelEnableButtons(true);
                });
            });
        }
    }
}