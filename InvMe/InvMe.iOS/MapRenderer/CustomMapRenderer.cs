using BLL;
using BLL.Xamarin.MapClasses;
using CoreGraphics;
using CoreLocation;
using CustomRenderer.iOS;
using FileStoringWithDependency.iOS.FileStoreAndLoad;
using InvMe_.iOS.MapRenderer;
using MapKit;
using System;
using System.Collections.Generic;
using System.Linq;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Maps.iOS;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(CustomMap), typeof(CustomMapRenderer))]
namespace CustomRenderer.iOS
{
    public class CustomMapRenderer : MapRenderer
    {
        List<BasicMapAnnotation> annotationsThatIAddedToTheMap;
        UIView customPinView;
        List<CustomPin> customPins;
        private bool tappedOnTheElement = false;
        FileStoreAndLoad fileFunctions = new FileStoreAndLoad();
        MKMapView nativeMap = new MKMapView();
        string filename = "";
        private UITapGestureRecognizer _tapRecogniser;

        private void OnTap(UITapGestureRecognizer recognizer)
        {
            if (!tappedOnTheElement)
            {
                var cgPoint = recognizer.LocationInView(Control);

                var location = ((MKMapView)Control).ConvertPoint(cgPoint, Control);
                
                if (filename == "event")
                {
                    GlobalVariables.EventCord = location.Latitude + ";" + location.Longitude + "";
                }
                else
                {
                    GlobalVariables.MeetingCord = location.Latitude + ";" + location.Longitude + "";
                }

                Position position = new Position(location.Latitude, location.Longitude);

                string meetorplace = "Event place";

                if (filename != "event")
                {
                    meetorplace = "Meeting place";
                }

                nativeMap.ClearsContextBeforeDrawing = true;

                var annotation = new BasicMapAnnotation(new CLLocationCoordinate2D(location.Latitude, location.Longitude), meetorplace);

                CustomPin customPin = new CustomPin()
                {
                    Pin = new Pin()
                    {
                        Label = meetorplace,
                        Position = position,
                        Type = PinType.Place
                    }
                };
                if (annotationsThatIAddedToTheMap.Count() != 0)
                {
                    nativeMap.RemoveAnnotation(annotationsThatIAddedToTheMap[0]);
                    annotationsThatIAddedToTheMap = new List<BasicMapAnnotation>();
                }
                customPins = new List<CustomPin>();
                customPins.Add(customPin);
                annotationsThatIAddedToTheMap.Add(annotation);
                nativeMap.AddAnnotation(annotation);
            }
            else
            {
                tappedOnTheElement = false;
            }
        }

        protected override void OnElementChanged(ElementChangedEventArgs<View> e)
        {
            base.OnElementChanged(e);

            annotationsThatIAddedToTheMap = new List<BasicMapAnnotation>();
            if (e.OldElement != null)
            {
                nativeMap = Control as MKMapView;
                nativeMap.GetViewForAnnotation = null;
                
                nativeMap.CalloutAccessoryControlTapped -= OnCalloutAccessoryControlTapped;
                nativeMap.DidSelectAnnotationView -= OnDidSelectAnnotationView;
                nativeMap.DidDeselectAnnotationView -= OnDidDeselectAnnotationView;
            }
            if (e.NewElement != null)
            {
                var formsMap = (CustomMap)e.NewElement;
                nativeMap = Control as MKMapView;
                
                if (formsMap.kind == "event")
                {
                    filename = "event";
                }
                else
                {
                    filename = "meet";
                }
                

                if (!formsMap.isJustShow) {

                    _tapRecogniser = new UITapGestureRecognizer(OnTap)
                    {
                        NumberOfTapsRequired = 1,
                        NumberOfTouchesRequired = 1
                    };

                    if (Control != null)
                        Control.RemoveGestureRecognizer(_tapRecogniser);

                    if (Control != null)
                        Control.AddGestureRecognizer(_tapRecogniser);

                    nativeMap.DidUpdateUserLocation += (sender, d) => {
                        if (nativeMap.UserLocation != null)
                        {
                            CLLocationCoordinate2D coords = nativeMap.UserLocation.Coordinate;
                            MKCoordinateSpan span = new MKCoordinateSpan(MilesToLatitudeDegrees(2), MilesToLongitudeDegrees(2, coords.Latitude));
                            nativeMap.Region = new MKCoordinateRegion(coords, span);
                        }
                    };
                }
                else
                {
                    customPins = formsMap.CustomPins;
                }
            }


        }

        public double MilesToLongitudeDegrees(double miles, double atLatitude)
        {
            double earthRadius = 3960.0; //in miles
            double degreesToRadians = Math.PI / 180.0;
            double radiansToDegrees = 180.0 / Math.PI;
            //derive the earth's radius at theat point in latitude
            double radiusAtLatitude = earthRadius * Math.Cos(atLatitude * degreesToRadians);
            return (miles / radiusAtLatitude) * radiansToDegrees;
        }

        public double MilesToLatitudeDegrees(double miles)
        {
            double earthRadius = 3960.0; //in miles
            double radiansToDegrees = 180.0 / Math.PI;
            return (miles / earthRadius) * radiansToDegrees;
        }

        MKAnnotationView GetViewForAnnotation(MKMapView mapView, IMKAnnotation annotation)
        {
                  MKAnnotationView annotationView = null;
            if (annotation is MKUserLocation)
                return null;
            var anno = annotation as MKPointAnnotation;
            CustomPin customPin = new CustomPin();
            customPin = GetCustomPin(anno);
            if (customPin == null)
            {
                throw new Exception("Custom pin not found");
            }
            annotationView = mapView.DequeueReusableAnnotation(customPin.Id);
            if (annotationView == null)
            {
                annotationView = new CustomMKAnnotationView(annotation, customPin.Id);
                annotationView.Image = UIImage.FromFile("pin.png");
                annotationView.CalloutOffset = new CGPoint(0, 0);
            //    annotationView.LeftCalloutAccessoryView = new UIImageView(UIImage.FromFile
            //("monkey.png"));
            //    annotationView.RightCalloutAccessoryView = UIButton.FromType
            //(UIButtonType.DetailDisclosure);
                ((CustomMKAnnotationView)annotationView).Id = customPin.Id;
                ((CustomMKAnnotationView)annotationView).Url = customPin.Url;
            }
            annotationView.CanShowCallout = true;
            return annotationView;
        }

        void OnDidSelectAnnotationView(object sender, MKAnnotationViewEventArgs e)
        {
            var customView = e.View as CustomMKAnnotationView;
            customPinView = new UIView();
            if (customView.Id == "Xamarin")
            {
                customPinView.Frame = new CGRect(0, 0, 200, 84);
                var image = new UIImageView(new CGRect(0, 0, 200, 84));
                image.Image = UIImage.FromFile("xamarin.png");
                customPinView.AddSubview(image);
                customPinView.Center = new CGPoint(0, -(e.View.Frame.Height + 75));
                e.View.AddSubview(customPinView);
            }
        }
        void OnCalloutAccessoryControlTapped(object sender, MKMapViewAccessoryTappedEventArgs e)
        {
            var customView = e.View as CustomMKAnnotationView;
            if (!string.IsNullOrWhiteSpace(customView.Url))
            {
                UIApplication.SharedApplication.OpenUrl(new Foundation.NSUrl
            (customView.Url));
            }
        }

        void OnDidDeselectAnnotationView(object sender, MKAnnotationViewEventArgs e)
        {
            if (!e.View.Selected)
            {
                customPinView.RemoveFromSuperview();
                customPinView.Dispose();
                customPinView = null;
            }
        }

        CustomPin GetCustomPin(MKPointAnnotation annotation)
        {
            var position = new Position(annotation.Coordinate.Latitude, annotation.Coordinate.Longitude);
            return customPins.FirstOrDefault(pin => pin.Pin.Position == position);
        }
    }
}