using System;
using System.Collections.Generic;
using System.ComponentModel;
using Android.Content;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.Widget;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Maps.Android;
using Xamarin.Forms.Platform.Android;
using Android.Locations;
using Android.OS;
using Android.Runtime;
using BLL.Xamarin.MapClasses;
using InvMe.Droid.CustomMapRenderer;
using BLL;

[assembly: ExportRenderer(typeof(CustomMap), typeof(CustomMapRenderer))]
namespace InvMe.Droid.CustomMapRenderer
{
    public class CustomMapRenderer : MapRenderer, GoogleMap.IInfoWindowAdapter, IOnMapReadyCallback
    {
        private GoogleMap _map;
        private List<CustomPin> _customPins;
        private bool _isDrawn;
        private bool tappedOnTheElement = false;
        bool isJustShow = true;
        Double lon, lat;
        string filename;

        protected override void OnElementChanged(ElementChangedEventArgs<Map> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null)
            {
                _map.InfoWindowClick -= OnInfoWindowClick;
            }

            if (e.NewElement != null)
            {
                var formsMap = (CustomMap)e.NewElement;

                if (formsMap.kind == "event")
                {
                    filename = "event";
                }
                else
                {
                    filename = "meet";
                }
                if (!formsMap.isJustShow) { isJustShow = false; }

                lon = formsMap.longitude;
                lat = formsMap.latitude;

                _customPins = formsMap.CustomPins;
                ((MapView)Control).GetMapAsync(this);
            }
        }

        public void OnMapReady(GoogleMap googleMap)
        {
            _map = googleMap;

            CameraPosition.Builder builder = CameraPosition.InvokeBuilder();
            builder.Target(new LatLng(lat, lon));
            CameraPosition cameraPosition = builder.Build();
            cameraPosition.Zoom = 10;
            CameraUpdate cameraUpdate = CameraUpdateFactory.NewCameraPosition(cameraPosition);
            _map.MoveCamera(cameraUpdate);
            //itt kene majd talan pint rakni
            if (!isJustShow)
            {
                _map.MapClick += _map_MapClick;
            }
            else
            {
                if (_map != null && _customPins != null)
                {
                    MarkerOptions markerOpt1 = new MarkerOptions();
                    markerOpt1.SetPosition(new LatLng(_customPins[0].Pin.Position.Latitude, _customPins[0].Pin.Position.Longitude));
                    markerOpt1.SetTitle(_customPins[0].Pin.Label);
                    markerOpt1.SetIcon(BitmapDescriptorFactory.FromResource(Resource.Drawable.pin));
                    _map.AddMarker(markerOpt1);
                }
            }
            _map.InfoWindowClick += OnInfoWindowClick;
            _map.SetInfoWindowAdapter(this);
        }

        private void _map_MapClick(object sender, GoogleMap.MapClickEventArgs e)
        {
            if (!tappedOnTheElement)
            {
                var position = new Position(e.Point.Latitude, e.Point.Longitude);

                if (filename == "event")
                {
                    GlobalVariables.EventCord = e.Point.Latitude + ";" + e.Point.Longitude + "";
                }
                else
                {
                    GlobalVariables.MeetingCord = e.Point.Latitude + ";" + e.Point.Longitude + "";
                }

                string meetorplace = "Event place";

                if (filename != "event")
                {
                    meetorplace = "Meeting place";
                }

                var pin = new CustomPin
                {
                    Pin = new Pin
                    {
                        Type = PinType.Place,
                        Position = position,
                        Label = meetorplace
                    }
                };

                _customPins = new List<CustomPin>();
                _customPins.Add(pin);
                var marker = new MarkerOptions();

                marker.SetPosition(new LatLng(pin.Pin.Position.Latitude, pin.Pin.Position.Longitude));

                marker.SetTitle(pin.Pin.Label);

                marker.SetSnippet(pin.Pin.Address);

                marker.SetIcon(BitmapDescriptorFactory.FromResource(Resource.Drawable.pin));
                _map.Clear();
                _map.AddMarker(marker);
            }
            else
            {
                tappedOnTheElement = false;
            }
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName.Equals("VisibleRegion") && !_isDrawn)
            {
                _map.Clear();

                foreach (var pin in _customPins)
                {
                    var marker = new MarkerOptions();

                    marker.SetPosition(new LatLng(pin.Pin.Position.Latitude, pin.Pin.Position.Longitude));

                    marker.SetTitle(pin.Pin.Label);

                    marker.SetSnippet(pin.Pin.Address);

                    marker.SetIcon(BitmapDescriptorFactory.FromResource(Resource.Drawable.pin));

                    _map.AddMarker(marker);
                }

                _isDrawn = true;
            }
        }

        protected override void OnLayout(bool changed, int l, int t, int r, int b)
        {
            base.OnLayout(changed, l, t, r, b);

            if (changed)
            {
                _isDrawn = false;
            }
        }

        void OnInfoWindowClick(object sender, GoogleMap.InfoWindowClickEventArgs e)
        {
            var customPin = GetCustomPin(e.Marker);

            if (customPin == null)
            {
                throw new Exception("Custom pin not found");
            }

            if (!string.IsNullOrWhiteSpace(customPin.Url))
            {
                var url = Android.Net.Uri.Parse(customPin.Url);

                var intent = new Intent(Intent.ActionView, url);

                intent.AddFlags(ActivityFlags.NewTask);

                Android.App.Application.Context.StartActivity(intent);
            }
        }

        public Android.Views.View GetInfoContents(Marker marker)
        {
            var inflater = Android.App.Application.Context.GetSystemService(Context.LayoutInflaterService) as Android.Views.LayoutInflater;

            if (inflater != null)
            {
                Android.Views.View view;

                var customPin = GetCustomPin(marker);
                if (customPin == null)
                {
                    throw new Exception("Custom pin not found");
                }

                view = inflater.Inflate(Resource.Layout.MapInfoWindow, null);

                var infoTitle = view.FindViewById<TextView>(Resource.Id.InfoWindowTitle);
                var infoSubtitle = view.FindViewById<TextView>(Resource.Id.InfoWindowSubtitle);

                if (infoTitle != null)
                {
                    infoTitle.Text = marker.Title;
                }
                if (infoSubtitle != null)
                {
                    infoSubtitle.Text = marker.Snippet;
                }

                return view;
            }
            return null;
        }

        public Android.Views.View GetInfoWindow(Marker marker)
        {
            return null;
        }

        CustomPin GetCustomPin(Marker annotation)
        {
            var position = new Position(annotation.Position.Latitude, annotation.Position.Longitude);

            foreach (var pin in _customPins)
            {
                if (pin.Pin.Position == position)
                {
                    return pin;
                }
            }
            return null;
        }
        public void OnStatusChanged(string provider, [GeneratedEnum] Availability status, Bundle extras)
        {
            throw new NotImplementedException();
        }

        public void OnProviderDisabled(string provider)
        {
            throw new NotImplementedException();
        }

        public void OnProviderEnabled(string provider)
        {
            throw new NotImplementedException();
        }

    }
}