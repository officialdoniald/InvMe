using BLL;
using BLL.ViewModel;
using BLL.Xamarin;
using Model;
using Plugin.Geolocator;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Essentials;
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

        private Plugin.Geolocator.Abstractions.IGeolocator locator;

        private Xamarin.Forms.Maps.Map map;
        private Xamarin.Forms.Maps.Map map2;

        #endregion

        public AddEventPage() { }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            LocalVariablesContainer.HowManyTimesUserLocationPopup += 1;

            InitializeAsync();
        }

        private async Task InitializeAsync()
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

            if (Device.RuntimePlatform == Device.iOS)
            {
                var userpos = await PermissonCheck();

                map = new Xamarin.Forms.Maps.Map()
                {
                    WidthRequest = 320,
                    HeightRequest = 200,
                    MapType = MapType.Street
                };

                map2 = new Xamarin.Forms.Maps.Map()
                {
                    WidthRequest = 320,
                    HeightRequest = 200,
                    MapType = MapType.Street
                };

                if (userpos.Longitude != 0 && userpos.Latitude != 0)
                {
                    map.IsShowingUser = true;
                    map2.IsShowingUser = true;
                }

                map.MapClicked += Map_MapClicked;
                map2.MapClicked += Map2_MapClicked;

                eventStack.Children.Add(map);

                meetStack.Children.Add(map2);
            }
            else
            {
                GetUserLocation();
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

        private void Map_MapClicked(object sender, MapClickedEventArgs e)
        {
            var map = (Xamarin.Forms.Maps.Map)sender;

            map.Pins.Clear();

            map.Pins.Add(new Pin()
            {
                Position = e.Position,
                Label = "Event place"
            });

            GlobalVariables.EventCord = e.Position.Latitude + ";" + e.Position.Longitude + "";
        }

        private void Map2_MapClicked(object sender, MapClickedEventArgs e)
        {
            var map = (Xamarin.Forms.Maps.Map)sender;

            map.Pins.Clear();

            map.Pins.Add(new Pin()
            {
                Position = e.Position,
                Label = "Meeting place"
            });

            GlobalVariables.MeetingCord = e.Position.Latitude + ";" + e.Position.Longitude + "";
        }

        private async void GetUserLocation()
        {
            var userpos = await PermissonCheck();

            Device.BeginInvokeOnMainThread(() =>
            {
                map = new Xamarin.Forms.Maps.Map()
                {
                    WidthRequest = 320,
                    HeightRequest = 200,
                    MapType = MapType.Street
                };

                map2 = new Xamarin.Forms.Maps.Map()
                {
                    WidthRequest = 320,
                    HeightRequest = 200,
                    MapType = MapType.Street
                };

                map.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(userpos.Latitude, userpos.Longitude), Distance.FromMeters(300)));
                map2.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(userpos.Latitude, userpos.Longitude), Distance.FromMeters(300)));

                map.MapClicked += Map_MapClicked1;
                map2.MapClicked += Map2_MapClicked1; ;

                eventStack.Children.Add(map);

                meetStack.Children.Add(map2);
            });
        }

        private void Map_MapClicked1(object sender, MapClickedEventArgs e)
        {
            var map = (Xamarin.Forms.Maps.Map)sender;

            map.Pins.Clear();

            map.Pins.Add(new Pin()
            {
                Position = e.Position,
                Label = "Event place"
            });

            GlobalVariables.EventCord = e.Position.Latitude + ";" + e.Position.Longitude + "";
        }

        private void Map2_MapClicked1(object sender, MapClickedEventArgs e)
        {
            var map = (Xamarin.Forms.Maps.Map)sender;

            map.Pins.Clear();

            map.Pins.Add(new Pin()
            {
                Position = e.Position,
                Label = "Meeting place"
            });

            GlobalVariables.MeetingCord = e.Position.Latitude + ";" + e.Position.Longitude + "";
        }

        private async Task<Location> PermissonCheck()
        {
            try
            {
                if (LocalVariablesContainer.HowManyTimesUserLocationPopup >= 1)
                {
                    if (GlobalVariables.AutomaticUserLocation)
                    {
                        var request = new GeolocationRequest(GeolocationAccuracy.Medium);
                        var location = await Geolocation.GetLocationAsync(request);

                        if (location != null)
                        {
                            Device.BeginInvokeOnMainThread(()=> {
                                map.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(location.Latitude, location.Longitude), Distance.FromMeters(300)));
                                map2.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(location.Latitude, location.Longitude), Distance.FromMeters(300)));
                            });

                            return location;
                        }
                        else
                        {
                            return new Location();
                        }
                    }
                    else
                    {
                        var status = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Location);

                        if (await CrossPermissions.Current.ShouldShowRequestPermissionRationaleAsync(Permission.Location))
                        {
                            await DisplayAlert("Need location", "Gunna need that location", "OK");
                        }

                        var action = await DisplayActionSheet(GlobalVariables.Language.NeedUserLocation(), GlobalVariables.Language.OK(), GlobalVariables.Language.Cancel());

                        if (action == GlobalVariables.Language.OK())
                        {
                            var results = await CrossPermissions.Current.RequestPermissionsAsync(new[] { Permission.Location });
                            status = results[Permission.Location];

                            if (status == Plugin.Permissions.Abstractions.PermissionStatus.Granted)
                            {
                                var request = new GeolocationRequest(GeolocationAccuracy.Medium);
                                var location = await Geolocation.GetLocationAsync(request);

                                if (location != null)
                                {


                                    return location;
                                }
                                else
                                {
                                    return new Location();
                                }
                            }
                        }
                        else
                        {
                            return new Location();
                        }

                        return new Location();
                    }
                }
                else
                {
                    return new Location();
                }
            }
            catch (Exception)
            {
                return new Location();
            }
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
                eventCreateObject.DESCRIPTION = descriptionEditor.Text;
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