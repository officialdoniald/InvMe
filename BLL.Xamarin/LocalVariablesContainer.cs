using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace BLL.Xamarin
{
    public static class LocalVariablesContainer
    {
        public static Style NormalLabel { get; set; }

        public static Style NavigationPageStyle { get; set; }

        public static string logintxt = "login.txt";

        public static string userlocationfile = "userloc.txt";

        public static string addpng = "add.png";

        public static string calendarpng = "calendar.png";

        public static string hashtagspng = "hashtags.png";

        public static string settingspng = "settings.png";

        public static string homepng = "home.png";

        public static int HowManyTimesUserLocationPopup = 0;
    }
}
