using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using UIKit;
using MapKit;
using CoreLocation;

namespace InvMe_.iOS.MapRenderer
{
    class BasicMapAnnotation : MKAnnotation
    {
        CLLocationCoordinate2D coord;
        string title, subtitle;

        public override CLLocationCoordinate2D Coordinate { get { return coord; } }
        public override void SetCoordinate(CLLocationCoordinate2D value)
        {
            coord = value;
        }
        public override string Title { get { return title; } }
        public override string Subtitle { get { return subtitle; } }
        public BasicMapAnnotation(CLLocationCoordinate2D coordinate, string title)
        {
            this.coord = coordinate;
            this.title = title;
        }

        public BasicMapAnnotation()
        {
        }
    }
}