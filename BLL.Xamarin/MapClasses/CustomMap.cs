using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms.Maps;

namespace BLL.Xamarin.MapClasses
{
    public class CustomMap : Map
    {
        public List<CustomPin> CustomPins { get; set; }

        public string kind { get; set; }

        public bool isJustShow { get; set; }

        public double longitude { get; set; }

        public double latitude { get; set; }
    }
}
