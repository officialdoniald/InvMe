using Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.IO;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;

namespace DAL
{
    public class DatabaseConnection
    {
        #region Properties

        private string ConnectionString;

        #endregion

        #region Snipets
        public static string GET_GlobalCasualImage_SQL { get; } =
                    "SELECT CasualImage FROM [dbo].[CasualImageTable]";
        public string INSERT_USER_SQL { get; } =
            "INSERT INTO [dbo].[USER]" +
            "([EMAIL], [FIRSTNAME], [LASTNAME], [BORNDATE], [PROFILEPICTURE], [PASSWORD]) " +
            "VALUES(" +
            "@EMAIL,@FIRSTNAME,@LASTNAME,@BORNDATE,@PROFILEPICTURE,@PASSWORD)";

        public string INSERT_EVENTS_SQL { get; } =
            "INSERT INTO [dbo].[EVENTS]" +
            "([EVENTNAME], [FROM], [TO], [ONLINE], [TOWN], [PLACE], [HOWMANY], [MEETINGCORD], [PLACECORD]) " +
            "VALUES(" +
            "@EVENTNAME,@FROM,@TO,@ONLINE,@TOWN,@PLACE,@HOWMANY,@MEETINGCORD,@PLACECORD);SET @id = SCOPE_IDENTITY();";

        public string INSERT_HASHTAGS_SQL { get; } =
            "INSERT INTO [dbo].[HASHTAGS]" +
            "([UID], [HASHTAG], [TOWN]) " +
            "VALUES(" +
            "@UID,@HASHTAG,@TOWN)";

        public string INSERT_FRIENDS_SQL { get; } =
            "INSERT INTO [dbo].[FRIENDS]" +
            "([SUID], [GUID]) " +
            "VALUES(" +
            "@SUID,@GUID)";

        public string INSERT_ATTENDED_SQL { get; } =
           "INSERT INTO [dbo].[Attended]" +
           "([user_id], [event_id]) " +
           "VALUES(" +
           "@user_id,@event_id)";

        public string GET_FRIENDS_SQL { get; } =
            "SELECT * FROM [dbo].[FRIENDS]";

        public string GET_ATTENDED_SQL { get; } =
            "SELECT * FROM [dbo].[Attended]";

        public string GET_USER_SQL { get; } =
            "SELECT * FROM [dbo].[USER]";

        public string GET_HASHTAGS_SQL { get; } =
            "SELECT * FROM [dbo].[HASHTAGS]";

        public string GET_EVENTS_SQL { get; } =
            "SELECT * FROM [dbo].[EVENTS] order by [FROM] ASC";

        public string GET_EVENTSBYUSERID_SQL { get; } =
            "SELECT * FROM [dbo].[EVENTS] order by [FROM] ASC";

        public string DELETE_EVENTS_SQL { get; } =
            "DELETE FROM [dbo].[EVENTS] WHERE ID = @id";

        public string DELETE_USER_SQL { get; } =
            "DELETE FROM [dbo].[HASHTAGS] WHERE UID = @id;" +
            "DELETE FROM [dbo].[Attended] WHERE user_id = @id;" +
            "DELETE FROM [dbo].[USER] WHERE ID = @id;";

        public string DELETE_HASHTAG_SQL { get; } =
            "DELETE FROM [dbo].[HASHTAGS] WHERE ID = @id";

        public string DELETE_FRIENDS_SQL { get; } =
            "DELETE FROM [dbo].[FRIENDS] WHERE ID = @id";

        public string DELETE_ATTENDED_SQL { get; } =
            "DELETE FROM [dbo].[Attended] WHERE id = @id";

        public string DELETE_ATTENDEDuserideventid_SQL { get; } =
            "DELETE FROM [dbo].[Attended] WHERE USER_ID = @USER_ID AND EVENT_ID = @EVENT_ID;";
        
        public string GETBYID_FRIENDS_SQL { get; } =
            "SELECT * FROM [dbo].[FRIENDS] WHERE ID=@id";
        public string GET_ATTENDEDBYID_SQL { get; } =
                    "SELECT * FROM [dbo].[ATTENDED] WHERE user_id=@id";

        public string GET_ATTENDEDBYUSERIDANDEVENTID_SQL { get; } =
                    "SELECT * FROM [dbo].[ATTENDED] WHERE user_id=@USERID and event_id=@EVENTID";

        public string GET_ATTENDEDBYEVENTID_SQL { get; } =
                    "SELECT * FROM [dbo].[ATTENDED] WHERE event_id=@id";

        public string GETBYID_USER_SQL { get; } =
            "SELECT * FROM [dbo].[USER] WHERE ID=@id";

        public string GETBYEMAIL_USER_SQL { get; } =
            "SELECT * FROM [dbo].[USER] WHERE EMAIL=@EMAIL";

        public string GETBYID_HASHTAGS_SQL { get; } =
            "SELECT * FROM [dbo].[HASHTAGS] WHERE UID=@id";
        
        public string GETBYID_EVENTS_SQL { get; } =
            "SELECT * FROM [dbo].[EVENTS] WHERE ID=@id";

        public string UPDATE_EVENTS_SQL { get; } =
            "UPDATE EVENTS SET " +
            "EVENTNAME=@EVENTNAME," +
            "DESCRIPTION=@DESCRIPTION," +
            "FROM=@FROM," +
            "TO=@TO," +
            "ONLINE=@ONLINE," +
            "TOWN=@TOWN," +
            "PLACE=@PLACE," +
            "MDESCRIPTION=@MDESCRIPTION," +
            "HOWMANY=@HOWMANY" +
            "MEETINGCORD=@MEETINGCORD," +
            "PLACECORD=@PLACECORD" +
            " WHERE ID=@ID";

        public string UPDATE_USER_SQL { get; } =
            "UPDATE [dbo].[USER] SET " +
            "EMAIL=@EMAIL," +
            "FIRSTNAME=@FIRSTNAME," +
            "LASTNAME=@LASTNAME," +
            "BORNDATE=@BORNDATE," +
            "PROFILEPICTURE=@PROFILEPICTURE," +
            "PASSWORD=@PASSWORD" +
            " WHERE ID=@ID";

        public string UPDATE_HASHTAGS_SQL { get; } =
           "UPDATE HASHTAGS SET " +
           "UID=@UID," +
           "HASHTAG=@HASHTAG," +
           "TOWN=@TOWN" +
           " WHERE ID=@ID";

        public string UPDATE_FRIENDS_SQL { get; } =
           "UPDATE FRIENDS SET " +
           "SUID=@SUID," +
           "GUID=@GUID" +
           " WHERE ID=@ID";

        #endregion

        public DatabaseConnection(string ConnectionString)
        {
            this.ConnectionString = ConnectionString;
        }

        public string RequestJson(string uri)
        {
            var request = new HttpRequestMessage();
            request.RequestUri = new Uri("https://invmewebapp20190213124106.azurewebsites.net/api/" + uri);
            request.Method = HttpMethod.Get;//Get Put Post Delete
            request.Headers.Add("Accept", "aaplication/json");//we would like JSON as response
            var client = new HttpClient();
            HttpResponseMessage response = client.SendAsync(request).Result;

            if (response.StatusCode == System.Net.HttpStatusCode.OK) { }

            HttpContent content = response.Content;
            var json = content.ReadAsStringAsync().Result;

            return json;
        }

        public HttpResponseMessage PostPut(HttpMethod method, object sendingObject, string uri)
        {
            string json = JsonConvert.SerializeObject(sendingObject);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var request = new HttpRequestMessage();
            request.RequestUri = new Uri("https://invmewebapp20190213124106.azurewebsites.net/api/" + uri);
            request.Method = method;
            request.Content = content;

            var client = new HttpClient();
            return client.SendAsync(request).Result;
        }

        public HttpResponseMessage Delete(string url, object sendingObject = null)
        {
            string json = JsonConvert.SerializeObject(sendingObject);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var request = new HttpRequestMessage();
            request.RequestUri = new Uri("https://invmewebapp20190213124106.azurewebsites.net/api/" + url);
            request.Method = HttpMethod.Delete;

            if (sendingObject != null)
            {
                request.Content = content;
            }

            var client = new HttpClient();
            return client.SendAsync(request).Result;
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
                //try
                //{
                //    using (FileStream SourceStream = File.Open(GlobalVariables.SourceSelectedImageFromGallery, FileMode.Open))
                //    using (MemoryStream ms = new MemoryStream())
                //    {
                //        SourceStream.CopyTo(ms);
                //        return ms.ToArray();
                //    }
                //}
                //catch (Exception)
                //{
                //    return null;
                //}

                return null;
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

        #region DeleteFunctions

        public bool DeleteEvent(Events events)
        {
            var message = Delete("Events/DeleteEvents/" + events.ID);

            if (message.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool DeleteFriend(Friends friend)
        {
            var message = Delete("Friends/DeleteFriends/" + friend.ID);

            if (message.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool DeleteAttended(Attended attend)
        {
            var message = Delete("Attended/DeleteAttended/" + attend.ID);

            if (message.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool DeleteAttendedbyuseridandeventid(Attended attend)
        {
            var message = Delete("Attended/DeleteAttendedByAttended", attend);

            if (message.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool DeleteHashtag(Hashtags hastag)
        {
            var message = Delete("Hashtags/DeleteHashtags/" + hastag.ID);

            if (message.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool DeleteUser(User user)
        {
            var message = Delete("Users/DeleteUser/" + user.ID);

            if (message.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        #endregion

        #region GetFunctions

        public byte[] GetGlobalCasualImage()
        {
            try
            {
                return JsonConvert.DeserializeObject<byte[]>(RequestJson("CasualImage/GetCasualImage"));
            }
            catch (Exception)
            {
                return null;
            }
        }

        public List<Events> GetEvent()
        {
            try
            {
                return JsonConvert.DeserializeObject<List<Events>>(RequestJson("Events/GetEvents"));
            }
            catch (Exception)
            {
                return null;
            }
        }

        public List<Friends> GetFriend()
        {
            try
            {
                return JsonConvert.DeserializeObject<List<Friends>>(RequestJson("Friends/GetFriends"));
            }
            catch (Exception)
            {
                return null;
            }
        }

        public List<Attended> GetAttended()
        {
            try
            {
                return JsonConvert.DeserializeObject<List<Attended>>(RequestJson("Attended/GetAttended"));
            }
            catch (Exception)
            {
                return null;
            }
        }

        public List<Hashtags> GetHashtag()
        {
            try
            {
                return JsonConvert.DeserializeObject<List<Hashtags>>(RequestJson("Hashtags/GetHashtags"));
            }
            catch (Exception)
            {
                return null;
            }
        }

        public List<User> GetUser()
        {
            try
            {
                return JsonConvert.DeserializeObject<List<User>>(RequestJson("Users/GetUsers"));
            }
            catch (Exception)
            {
                return null;
            }
        }

        #endregion

        #region GetElementByID

        public Events GetEventByID(int ID)
        {
            try
            {
                return JsonConvert.DeserializeObject<Events>(RequestJson("Events/GetEventsByID/" + ID));
            }
            catch (Exception)
            {
                return null;
            }
        }

        public Friends GetFriendByID(int ID)
        {
            try
            {
                return JsonConvert.DeserializeObject<Friends>(RequestJson("Friends/GetFriendsByID/" + ID));
            }
            catch (Exception)
            {
                return null;
            }
        }

        public List<Hashtags> GetHashtagByUSERID(int ID)
        {
            try
            {
                return JsonConvert.DeserializeObject<List<Hashtags>>(RequestJson("Hashtags/GetHashtagsByUserID/" + ID));
            }
            catch (Exception)
            {
                return new List<Hashtags>();
            }
        }

        public User GetUserByID(int ID)
        {
            try
            {
                return JsonConvert.DeserializeObject<User>(RequestJson("Users/GetUserByID/" + ID));
            }
            catch (Exception)
            {
                return null;
            }
        }

        public User GetUserByEMAIL(string EMAIL)
        {
            try
            {
                return JsonConvert.DeserializeObject<User>(RequestJson("Users/GetUserByEmail/" + EMAIL));
            }
            catch (Exception)
            {
                return null;
            }
        }

        public Attended GetAttendedByUserAnEventID(int USERID, int EVENTID)
        {
            try
            {
                return JsonConvert.DeserializeObject<Attended>(RequestJson("Attended/GetAttendedByUserAnEventID?userid=" + USERID + "&eventid=" + EVENTID));
            }
            catch (Exception)
            {
                return null;
            }
        }

        public List<Attended> GetAttendedByID(int ID)
        {
            try
            {
                return JsonConvert.DeserializeObject<List<Attended>>(RequestJson("Attended/GetAttendedByID/" + ID));
            }
            catch (Exception)
            {
                return new List<Attended>();
            }
        }

        public List<Attended> GetAttendedByEventID(int ID)
        {
            try
            {
                return JsonConvert.DeserializeObject<List<Attended>>(RequestJson("Attended/GetAttendedByEventID/" + ID));
            }
            catch (Exception)
            {
                return new List<Attended>();
            }
        }

        #endregion

        #region InsertFunctions

        public bool InsertUser(User user)
        {
            var message = PostPut(HttpMethod.Post, user, "Users/InsertUser");

            if (message.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public int InsertEventAsync(Events events)
        {
            var message = PostPut(HttpMethod.Post, events, "Events/InsertEvent");

            return JsonConvert.DeserializeObject<int>(message.Content.ReadAsStringAsync().Result);
        }

        public bool InsertFriend(Friends friends)
        {
            var message = PostPut(HttpMethod.Post, friends, "Friends/InsertFriends");

            if (message.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool InsertAttended(Attended attended)
        {
            var message = PostPut(HttpMethod.Post, attended, "Attended/InsertAttended");

            if (message.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool InsertHashtag(Hashtags hashtags)
        {
            var message = PostPut(HttpMethod.Post, hashtags, "Hashtags/InsertHashtags");

            if (message.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        #endregion

        #region UpdateFunctions

        public bool UpdateEvent(int ID, Events events)
        {
            var message = PostPut(HttpMethod.Put, events, "Events/UpdateEvents");

            if (message.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool UpdateFriend(int ID, Friends friend)
        {
            var message = PostPut(HttpMethod.Put, friend, "Friends/UpdateFriends");

            if (message.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool UpdateHashtag(int ID, Hashtags hastag)
        {
            var message = PostPut(HttpMethod.Put, hastag, "Hashtags/UpdateHashtags");

            if (message.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool UpdateUser(int ID, User user)
        {
            var message = PostPut(HttpMethod.Put, user, "Users/UpdateUser");

            if (message.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        #endregion
    }
}