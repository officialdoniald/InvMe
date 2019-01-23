using Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace BLL.Helper
{
    public class GlobalFunctionsContainer
    {
        public static void InitGlobalSettings()
        {
            GlobalVariables.GlobalPassword = Properties.Resources.ResourceManager.GetString("GlobalPassword");
            GlobalVariables.ConnectionString = Properties.Resources.ResourceManager.GetString("ConnectionString");
            GlobalVariables.DatabaseConnection = new DAL.DatabaseConnection(GlobalVariables.ConnectionString);
        }

        public static bool IsValidEmailAddress(string emailaddress)
        {
            try
            {
                Regex rx = new Regex(
            @"^[-!#$%&'*+/0-9=?A-Z^_a-z{|}~](\.?[-!#$%&'*+/0-9=?A-Z^_a-z{|}~])*@[a-zA-Z](-?[a-zA-Z0-9])*(\.[a-zA-Z](-?[a-zA-Z0-9])*)+$");
                return rx.IsMatch(emailaddress);
            }
            catch (FormatException)
            {
                return false;
            }
        }

        public static byte[] ReadFully(Stream input)
        {
            try
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    input.CopyTo(ms);
                    return ms.ToArray();
                }
            }
            catch (Exception)
            {
                try
                {
                    using (FileStream SourceStream = File.Open(GlobalVariables.SourceSelectedImageFromGallery, FileMode.Open))
                    using (MemoryStream ms = new MemoryStream())
                    {
                        SourceStream.CopyTo(ms);
                        return ms.ToArray();
                    }
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }

        public static string EncryptPassword(string password)
        {
            using (MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider())
            {
                UTF8Encoding uTF8Encoding = new UTF8Encoding();
                byte[] data = md5.ComputeHash(uTF8Encoding.GetBytes(password));
                return Convert.ToBase64String(data);
            }
        }

        public static string RandomString(int size, bool lowerCase)
        {
            StringBuilder builder = new StringBuilder();
            Random random = new Random();
            char ch;
            for (int i = 0; i < size; i++)
            {
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                builder.Append(ch);
            }
            if (lowerCase)
                return builder.ToString().ToLower();
            return builder.ToString();
        }
        
        public static async void SendEmail(string url, string email, string firstname, string lastname, string eventname, string from, string to, string town, string place)
        {
            string urls = "";

            if (url == "eventcreate" || url == "joinedevent" || url == "sendmailtosubscribeduser")
            {
                string timef = "";
                string timet = "";
                string finallytown = "";

                DateTimeOffset dto_begin = new DateTimeOffset();
                DateTimeOffset.TryParse(from, out dto_begin);
                dto_begin = dto_begin.ToLocalTime();

                timef = dto_begin.ToString(GlobalVariables.DateFormatForEventsList);

                if (from == to)
                {
                    timet = "Dosn't matter";
                }
                else
                {
                    DateTimeOffset dto_end = new DateTimeOffset();
                    DateTimeOffset.TryParse(to, out dto_end);
                    dto_end = dto_end.ToLocalTime();
                    timet = dto_end.ToString(GlobalVariables.DateFormatForEventsList);
                }

                if (String.IsNullOrEmpty(town) || String.IsNullOrEmpty(place))
                    finallytown = "Online";
                else finallytown = town + ", " + place;

                urls = String.Format("http://invme.hu/invmeapp/{0}.php?emaill={1}&nev={2}&eventname={3}&town={4}&from={5}&to={6}", url, email, firstname + "_" + lastname, eventname, finallytown, timef, timet);
            }
            else
            {
                urls = String.Format("http://invme.hu/invmeapp/{0}.php?emaill={1}&nev={2}", url, email, firstname + "_" + lastname);
            }
            try
            {
                Uri uri = new Uri(urls);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(urls);
                request.Method = "GET";
                WebResponse res = await request.GetResponseAsync();
            }
            catch (Exception)
            {

            }
        }
    }
}