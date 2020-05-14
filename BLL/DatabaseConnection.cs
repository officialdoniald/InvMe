using BLL;
using Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace DAL
{
    public class DatabaseConnection
    {
        public DatabaseConnection()
        {
            
        }

        public string RequestJson(string uri)
        {
            var request = new HttpRequestMessage();
            request.RequestUri = new Uri(GlobalVariables.WebApiURL + uri);
            request.Method = HttpMethod.Get;//Get Put Post Delete
            request.Headers.Add("Accept", "aplication/json");//we would like JSON as response
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
            request.RequestUri = new Uri(GlobalVariables.WebApiURL + uri);
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
            request.RequestUri = new Uri(GlobalVariables.WebApiURL + url);
            request.Method = HttpMethod.Delete;

            if (sendingObject != null)
            {
                request.Content = content;
            }

            var client = new HttpClient();
            return client.SendAsync(request).Result;
        }

        #region DeleteFunctions

        public bool DeleteBlockedPeople(BlockedPeople blockedPeople)
        {
            var message = Delete("Blockedpeople/DeleteBlockedUser", blockedPeople);

            if (message.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool DeleteAttendedByAttended(Attended attend)
        {
            var attended = GetAttendedByUserAnEventID(attend.USERID,attend.EVENTID);

            var message = Delete("Attended/DeleteAttendedByAttended", attended);

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

        public List<BlockedPeople> GetBlockedPeopleByID(int userid)
        {
            try
            {
                return JsonConvert.DeserializeObject<List<BlockedPeople>>(RequestJson("Blockedpeople/BlockedPeopleList/" + userid));
            }
            catch (Exception)
            {
                return null;
            }
        }

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

        public bool InsertBlockedPeople(BlockedPeople blockedPeople)
        {
            var message = PostPut(HttpMethod.Post, blockedPeople, "Blockedpeople/InsertBlockeduser");

            if (message.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

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

        public bool UpdateUserReported(User ID)
        {
            ID.REPORTED = 1;

            var message = PostPut(HttpMethod.Put, ID, "Users/ReportUser");

            if (message.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool UpdateEvent(Events events)
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

        public bool UpdateUser(User user)
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

        public bool ForgotPasswordAsync(User user)
        {
            var message = PostPut(HttpMethod.Put, user, "Users/ForgotPasswordAsync");

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