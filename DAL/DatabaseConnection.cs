using Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.IO;
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
            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(
                        DELETE_EVENTS_SQL
                        , conn))
                    {
                        cmd.Parameters.AddWithValue("@id", events.ID);
                        if (cmd.ExecuteNonQuery() == 1) return true;
                        else return false;
                    }
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool DeleteFriend(Friends friend)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(
                       DELETE_FRIENDS_SQL
                        , conn))
                    {
                        cmd.Parameters.AddWithValue("@id", friend.ID);
                        if (cmd.ExecuteNonQuery() == 1) return true;
                        else return false;
                    }
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool DeleteAttended(Attended attend)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(
                       DELETE_ATTENDED_SQL
                        , conn))
                    {
                        cmd.Parameters.AddWithValue("@id", attend.ID);
                        if (cmd.ExecuteNonQuery() == 1) return true;
                        else return false;
                    }
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool DeleteAttendedbyuseridandeventid(Attended attend)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(
                       DELETE_ATTENDEDuserideventid_SQL
                        , conn))
                    {
                        cmd.Parameters.AddWithValue("@USER_ID", attend.USER_ID);
                        cmd.Parameters.AddWithValue("@EVENT_ID", attend.EVENT_ID);
                        if (cmd.ExecuteNonQuery() == 1) return true;
                        else return false;
                    }
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool DeleteHashtag(Hashtags hastag)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(
                        DELETE_HASHTAG_SQL
                        , conn))
                    {
                        cmd.Parameters.AddWithValue("@id", hastag.ID);
                        if (cmd.ExecuteNonQuery() == 1) return true;
                        else return false;
                    }
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool DeleteUser(User user)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(DELETE_USER_SQL, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", user.ID);
                        if (cmd.ExecuteNonQuery() >= 1) return true;
                        else return false;
                    }
                }
            }
            catch (Exception)
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
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                using (SqlCommand cmd = new SqlCommand(GET_GlobalCasualImage_SQL, conn))
                {
                    conn.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader != null)
                        {
                            while (reader.Read())
                            {
                                System.IO.Stream stream = reader.GetStream(reader.GetOrdinal("CasualImage"));

                                if (stream.Length != 0)
                                    return ReadFully(stream);
                                else return null;
                            }
                        }
                    }
                }
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public List<Events> GetEvent()
        {
            List<Events> events = new List<Events>();

            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                using (SqlCommand cmd = new SqlCommand(GET_EVENTS_SQL, conn))
                {
                    conn.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {

                        if (reader != null)
                        {
                            while (reader.Read())
                            {
                                Events even = new Events();

                                even.ID = reader.GetInt32(reader.GetOrdinal("id"));
                                even.EVENTNAME = reader.GetString(reader.GetOrdinal("EVENTNAME"));
                                try
                                {
                                    even.DESCRIPTION = reader.GetString(reader.GetOrdinal("DESCRIPTION"));
                                }
                                catch (Exception)
                                {
                                    even.DESCRIPTION = null;
                                }
                                try
                                {
                                    even.FROM = reader.GetDateTimeOffset(reader.GetOrdinal("FROM"));
                                }
                                catch (Exception)
                                {
                                    even.FROM = new DateTime();
                                }
                                try
                                {
                                    even.TO = reader.GetDateTimeOffset(reader.GetOrdinal("TO"));
                                }
                                catch (Exception)
                                {
                                    even.TO = new DateTime();
                                }
                                try
                                {
                                    even.ONLINE = reader.GetInt32(reader.GetOrdinal("ONLINE"));
                                }
                                catch (Exception)
                                {
                                    even.ONLINE = 0;
                                }
                                try
                                {
                                    even.TOWN = reader.GetString(reader.GetOrdinal("TOWN"));
                                }
                                catch (Exception)
                                {
                                    even.TOWN = null;
                                }
                                try
                                {
                                    even.PLACE = reader.GetString(reader.GetOrdinal("PLACE"));
                                }
                                catch (Exception)
                                {
                                    even.PLACE = null;
                                }
                                try
                                {
                                    even.MDESCRIPTION = reader.GetString(reader.GetOrdinal("MDESCRIPTION"));
                                }
                                catch (Exception)
                                {
                                    even.MDESCRIPTION = null;
                                }
                                try
                                {
                                    even.HOWMANY = reader.GetInt32(reader.GetOrdinal("HOWMANY"));
                                }
                                catch (Exception)
                                {
                                    even.HOWMANY = 1;
                                }
                                try
                                {
                                    even.MEETINGCORD = reader.GetString(reader.GetOrdinal("MEETINGCORD"));
                                }
                                catch (Exception)
                                {
                                    even.MEETINGCORD = null;
                                }
                                try
                                {
                                    even.PLACECORD = reader.GetString(reader.GetOrdinal("PLACECORD"));
                                }
                                catch (Exception)
                                {
                                    even.PLACECORD = null;
                                }

                                events.Add(even);
                            }
                        }
                    }
                }
                return events;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public List<Friends> GetFriend()
        {
            List<Friends> friends = new List<Friends>();

            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                using (SqlCommand cmd = new SqlCommand(GET_FRIENDS_SQL, conn))
                {
                    conn.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {

                        if (reader != null)
                        {
                            while (reader.Read())
                            {
                                Friends friend = new Friends();

                                friend.ID = reader.GetInt32(reader.GetOrdinal("id"));
                                friend.SUID = reader.GetInt32(reader.GetOrdinal("SUID"));
                                friend.GUID = reader.GetInt32(reader.GetOrdinal("GUID"));

                                friends.Add(friend);
                            }
                        }
                    }
                }
                return friends;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public List<Attended> GetAttended()
        {
            List<Attended> attended = new List<Attended>();

            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                using (SqlCommand cmd = new SqlCommand(GET_ATTENDED_SQL, conn))
                {
                    conn.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {

                        if (reader != null)
                        {
                            while (reader.Read())
                            {
                                Attended attend = new Attended();

                                attend.ID = reader.GetInt32(reader.GetOrdinal("id"));
                                attend.USER_ID = reader.GetInt32(reader.GetOrdinal("user_id"));
                                attend.EVENT_ID = reader.GetInt32(reader.GetOrdinal("event_id"));

                                attended.Add(attend);
                            }
                        }
                    }
                }
                return attended;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public List<Hashtags> GetHashtag()
        {
            List<Hashtags> hashtags = new List<Hashtags>();

            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                using (SqlCommand cmd = new SqlCommand(GET_HASHTAGS_SQL, conn))
                {
                    conn.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {

                        if (reader != null)
                        {
                            while (reader.Read())
                            {
                                Hashtags hashtag = new Hashtags();

                                hashtag.ID = reader.GetInt32(reader.GetOrdinal("id"));
                                hashtag.UID = reader.GetInt32(reader.GetOrdinal("UID"));
                                hashtag.HASHTAG = reader.GetString(reader.GetOrdinal("HASHTAG"));
                                hashtag.TOWN = reader.GetString(reader.GetOrdinal("TOWN"));

                                hashtags.Add(hashtag);
                            }
                        }
                    }
                }
                return hashtags;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public List<User> GetUser()
        {
            List<User> users = new List<User>();

            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                using (SqlCommand cmd = new SqlCommand(GET_USER_SQL, conn))
                {
                    conn.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader != null)
                        {
                            while (reader.Read())
                            {
                                User user = new User();

                                user.ID = reader.GetInt32(reader.GetOrdinal("id"));
                                user.EMAIL = reader.GetString(reader.GetOrdinal("EMAIL"));
                                user.FIRSTNAME = reader.GetString(reader.GetOrdinal("FIRSTNAME"));
                                user.LASTNAME = reader.GetString(reader.GetOrdinal("LASTNAME"));
                                user.BORNDATE = reader.GetDateTime(reader.GetOrdinal("BORNDATE"));
                                user.PROFILEPICTURE = ReadFully(reader.GetStream(reader.GetOrdinal("PROFILEPICTURE")));
                                user.PASSWORD = reader.GetString(reader.GetOrdinal("PASSWORD"));

                                users.Add(user);
                            }
                        }
                    }
                }
                return users;
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
            Events even = new Events();

            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                using (SqlCommand cmd = new SqlCommand(
                    GETBYID_EVENTS_SQL
                    , conn))
                {
                    conn.Open();
                    cmd.Parameters.AddWithValue("@id", ID);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {

                        if (reader != null)
                        {
                            while (reader.Read())
                            {
                                even.ID = reader.GetInt32(reader.GetOrdinal("id"));
                                even.EVENTNAME = reader.GetString(reader.GetOrdinal("EVENTNAME"));
                                try
                                {
                                    even.DESCRIPTION = reader.GetString(reader.GetOrdinal("DESCRIPTION"));
                                }
                                catch (Exception)
                                {
                                    even.DESCRIPTION = null;
                                }
                                try
                                {
                                    even.FROM = reader.GetDateTimeOffset(reader.GetOrdinal("FROM"));
                                }
                                catch (Exception)
                                {
                                    even.FROM = new DateTime();
                                }
                                try
                                {
                                    even.TO = reader.GetDateTimeOffset(reader.GetOrdinal("TO"));
                                }
                                catch (Exception)
                                {
                                    even.TO = new DateTime();
                                }
                                try
                                {
                                    even.ONLINE = reader.GetInt32(reader.GetOrdinal("ONLINE"));
                                }
                                catch (Exception)
                                {
                                    even.ONLINE = 0;
                                }
                                try
                                {
                                    even.TOWN = reader.GetString(reader.GetOrdinal("TOWN"));
                                }
                                catch (Exception)
                                {
                                    even.TOWN = null;
                                }
                                try
                                {
                                    even.PLACE = reader.GetString(reader.GetOrdinal("PLACE"));
                                }
                                catch (Exception)
                                {
                                    even.PLACE = null;
                                }
                                try
                                {
                                    even.MDESCRIPTION = reader.GetString(reader.GetOrdinal("MDESCRIPTION"));
                                }
                                catch (Exception)
                                {
                                    even.MDESCRIPTION = null;
                                }
                                try
                                {
                                    even.HOWMANY = reader.GetInt32(reader.GetOrdinal("HOWMANY"));
                                }
                                catch (Exception)
                                {
                                    even.HOWMANY = 1;
                                }
                                try
                                {
                                    even.MEETINGCORD = reader.GetString(reader.GetOrdinal("MEETINGCORD"));
                                }
                                catch (Exception)
                                {
                                    even.MEETINGCORD = null;
                                }
                                try
                                {
                                    even.PLACECORD = reader.GetString(reader.GetOrdinal("PLACECORD"));
                                }
                                catch (Exception)
                                {
                                    even.PLACECORD = null;
                                }
                            }
                        }
                    }
                }
                return even;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public Friends GetFriendByID(int ID)
        {
            Friends friend = new Friends();

            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                using (SqlCommand cmd = new SqlCommand(
                    GETBYID_FRIENDS_SQL
                    , conn))
                {
                    conn.Open();
                    cmd.Parameters.AddWithValue("@id", ID);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {

                        if (reader != null)
                        {
                            while (reader.Read())
                            {
                                friend.ID = reader.GetInt32(reader.GetOrdinal("id"));
                                friend.SUID = reader.GetInt32(reader.GetOrdinal("SUID"));
                                friend.GUID = reader.GetInt32(reader.GetOrdinal("GUID"));
                            }
                        }
                    }
                }
                return friend;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public List<Hashtags> GetHashtagByUSERID(int ID)
        {
            List<Hashtags> hashtags = new List<Hashtags>();

            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                using (SqlCommand cmd = new SqlCommand(
                    GETBYID_HASHTAGS_SQL
                    , conn))
                {
                    conn.Open();
                    cmd.Parameters.AddWithValue("@id", ID);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader != null)
                        {
                            while (reader.Read())
                            {
                                Hashtags hashtag = new Hashtags();

                                hashtag.ID = reader.GetInt32(reader.GetOrdinal("id"));
                                hashtag.UID = reader.GetInt32(reader.GetOrdinal("UID"));
                                hashtag.HASHTAG = reader.GetString(reader.GetOrdinal("HASHTAG"));
                                hashtag.TOWN = reader.GetString(reader.GetOrdinal("TOWN"));

                                hashtags.Add(hashtag);
                            }
                        }
                    }
                }
                return hashtags;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public User GetUserByID(int ID)
        {
            User user = new User();

            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                using (SqlCommand cmd = new SqlCommand(
                    GETBYID_USER_SQL
                    , conn))
                {
                    conn.Open();

                    cmd.Parameters.AddWithValue("@id", ID);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {

                        if (reader != null)
                        {
                            while (reader.Read())
                            {
                                user.ID = reader.GetInt32(reader.GetOrdinal("id"));
                                user.EMAIL = reader.GetString(reader.GetOrdinal("EMAIL"));
                                user.FIRSTNAME = reader.GetString(reader.GetOrdinal("FIRSTNAME"));
                                user.LASTNAME = reader.GetString(reader.GetOrdinal("LASTNAME"));
                                user.BORNDATE = reader.GetDateTime(reader.GetOrdinal("BORNDATE"));
                                user.PROFILEPICTURE = ReadFully(reader.GetStream(reader.GetOrdinal("PROFILEPICTURE")));
                                user.PASSWORD = reader.GetString(reader.GetOrdinal("PASSWORD"));
                            }
                        }
                    }
                }
                return user;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public User GetUserByEMAIL(string EMAIL)
        {
            User user = new User();

            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                using (SqlCommand cmd = new SqlCommand(
                    GETBYEMAIL_USER_SQL
                    , conn))
                {
                    conn.Open();

                    cmd.Parameters.AddWithValue("@EMAIL", EMAIL);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader != null)
                        {
                            while (reader.Read())
                            {
                                user.ID = reader.GetInt32(reader.GetOrdinal("id"));
                                user.EMAIL = reader.GetString(reader.GetOrdinal("EMAIL"));
                                user.FIRSTNAME = reader.GetString(reader.GetOrdinal("FIRSTNAME"));
                                user.LASTNAME = reader.GetString(reader.GetOrdinal("LASTNAME"));
                                user.BORNDATE = reader.GetDateTime(reader.GetOrdinal("BORNDATE"));
                                user.PROFILEPICTURE = ReadFully(reader.GetStream(reader.GetOrdinal("PROFILEPICTURE")));
                                user.PASSWORD = reader.GetString(reader.GetOrdinal("PASSWORD"));
                            }
                        }
                    }
                }
                return user;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public Attended GetAttendedByUserAnEventID(int USERID, int EVENTID)
        {
            Attended attended = new Attended();

            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                using (SqlCommand cmd = new SqlCommand(GET_ATTENDEDBYUSERIDANDEVENTID_SQL, conn))
                {
                    cmd.Parameters.AddWithValue("@USERID", USERID);
                    cmd.Parameters.AddWithValue("@EVENTID", EVENTID);
                    conn.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader != null)
                        {
                            while (reader.Read())
                            {
                                attended.ID = reader.GetInt32(reader.GetOrdinal("id"));
                                attended.USER_ID = reader.GetInt32(reader.GetOrdinal("user_id"));
                                attended.EVENT_ID = reader.GetInt32(reader.GetOrdinal("event_id"));

                                break;
                            }
                        }
                    }
                }
                return attended;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public List<Attended> GetAttendedByID(int ID)
        {
            List<Attended> attend = new List<Attended>();

            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                using (SqlCommand cmd = new SqlCommand(GET_ATTENDEDBYID_SQL, conn))
                {
                    cmd.Parameters.AddWithValue("@id", ID);
                    conn.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader != null)
                        {
                            while (reader.Read())
                            {
                                Attended attended = new Attended();
                                attended.ID = reader.GetInt32(reader.GetOrdinal("id"));
                                attended.USER_ID = reader.GetInt32(reader.GetOrdinal("user_id"));
                                attended.EVENT_ID = reader.GetInt32(reader.GetOrdinal("event_id"));

                                attend.Add(attended);
                            }
                        }
                    }
                }
                return attend;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public List<Attended> GetAttendedByEventID(int ID)
        {
            List<Attended> attended = new List<Attended>();

            try
            {

                using (SqlConnection conn = new SqlConnection(ConnectionString))
                using (SqlCommand cmd = new SqlCommand(GET_ATTENDEDBYEVENTID_SQL, conn))
                {
                    cmd.Parameters.AddWithValue("@id", ID);
                    conn.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {

                        if (reader != null)
                        {
                            while (reader.Read())
                            {
                                Attended attend = new Attended();

                                attend.ID = reader.GetInt32(reader.GetOrdinal("id"));
                                attend.USER_ID = reader.GetInt32(reader.GetOrdinal("user_id"));
                                attend.EVENT_ID = reader.GetInt32(reader.GetOrdinal("event_id"));

                                attended.Add(attend);
                            }
                        }
                    }
                }
                return attended;
            }
            catch (Exception)
            {
                return null;
            }
        }

        #endregion

        #region InsertFunctions

        public bool InsertUser(User user)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand(INSERT_USER_SQL, conn);

                    cmd.Parameters.Add(
                        new SqlParameter("@EMAIL", user.EMAIL)
                        {
                            SqlDbType = System.Data.SqlDbType.NVarChar
                        }
                     );
                    cmd.Parameters.Add(
                        new SqlParameter("@FIRSTNAME", user.FIRSTNAME)
                        {
                            SqlDbType = System.Data.SqlDbType.NVarChar
                        }
                     );
                    cmd.Parameters.Add(
                        new SqlParameter("@LASTNAME", user.LASTNAME)
                        {
                            SqlDbType = System.Data.SqlDbType.NVarChar
                        }
                     );
                    cmd.Parameters.Add(
                        new SqlParameter("@BORNDATE", (object)user.BORNDATE ?? DBNull.Value)
                        {
                            SqlDbType = System.Data.SqlDbType.DateTime
                        }
                     );
                    cmd.Parameters.Add(
                        new SqlParameter("@PROFILEPICTURE", (object)user.PROFILEPICTURE ?? DBNull.Value)
                        {
                            SqlDbType = System.Data.SqlDbType.Image
                        }
                     );
                    cmd.Parameters.Add(
                        new SqlParameter("@PASSWORD", EncryptPassword(user.PASSWORD))
                        {
                            SqlDbType = System.Data.SqlDbType.VarChar
                        }
                     );

                    cmd.ExecuteNonQuery();

                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        public int InsertEventAsync(Events events)
        {
            try
            {
                SqlConnection conn = new SqlConnection(ConnectionString);

                conn.Open();

                SqlCommand cmd = new SqlCommand(INSERT_EVENTS_SQL, conn);

                cmd.Parameters.Add(
                    new SqlParameter("@EVENTNAME", events.EVENTNAME)
                    {
                        SqlDbType = System.Data.SqlDbType.NVarChar
                    }
                 );
                cmd.Parameters.Add(
                    new SqlParameter("@HOWMANY", events.HOWMANY)
                    {
                        SqlDbType = System.Data.SqlDbType.Int
                    }
                 );
                cmd.Parameters.Add(
                    new SqlParameter("@ONLINE", events.ONLINE)
                    {
                        SqlDbType = System.Data.SqlDbType.Int
                    }
                 );
                cmd.Parameters.Add(
                    new SqlParameter("@FROM", (object)events.FROM ?? DBNull.Value)
                    {
                        SqlDbType = System.Data.SqlDbType.DateTimeOffset
                    }
                 );
                cmd.Parameters.Add(
                    new SqlParameter("@TO", (object)events.TO ?? DBNull.Value)
                    {
                        SqlDbType = System.Data.SqlDbType.DateTimeOffset
                    }
                 );
                cmd.Parameters.Add(
                    new SqlParameter("@TOWN", (object)events.TOWN ?? DBNull.Value)
                    {
                        SqlDbType = System.Data.SqlDbType.NVarChar
                    }
                 );
                cmd.Parameters.Add(
                    new SqlParameter("@PLACE", (object)events.PLACE ?? DBNull.Value)
                    {
                        SqlDbType = System.Data.SqlDbType.NVarChar
                    }
                 );
                cmd.Parameters.Add(
                    new SqlParameter("@MEETINGCORD", (object)events.MEETINGCORD ?? DBNull.Value)
                    {
                        SqlDbType = System.Data.SqlDbType.NVarChar
                    }
                 );
                cmd.Parameters.Add(
                    new SqlParameter("@PLACECORD", (object)events.PLACECORD ?? DBNull.Value)
                    {
                        SqlDbType = System.Data.SqlDbType.NVarChar
                    }
                 );

                var idpar = new SqlParameter("@id", DBNull.Value)
                {
                    SqlDbType = System.Data.SqlDbType.Int,
                    Direction = System.Data.ParameterDirection.Output
                };
                cmd.Parameters.Add(idpar);
                cmd.CommandTimeout = 600;
                cmd.ExecuteNonQuery();

                events.ID = (int)idpar.Value;

                return events.ID;
            }
            catch (Exception)
            {
                return -1;
            }
        }

        public string InsertFriend(Friends friends)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand(INSERT_FRIENDS_SQL, conn);

                    cmd.Parameters.Add(
                        new SqlParameter("@SUID", friends.SUID)
                        {
                            SqlDbType = System.Data.SqlDbType.NVarChar
                        }
                     );
                    cmd.Parameters.Add(
                        new SqlParameter("@GUID", friends.GUID)
                        {
                            SqlDbType = System.Data.SqlDbType.NVarChar
                        }
                     );

                    cmd.ExecuteNonQuery();

                    return "";
                }
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        public bool InsertAttended(Attended attended)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand(INSERT_ATTENDED_SQL, conn);

                    cmd.Parameters.Add(
                        new SqlParameter("@user_id", attended.USER_ID)
                        {
                            SqlDbType = System.Data.SqlDbType.Int
                        }
                     );
                    cmd.Parameters.Add(
                        new SqlParameter("@event_id", attended.EVENT_ID)
                        {
                            SqlDbType = System.Data.SqlDbType.Int
                        }
                     );

                    cmd.ExecuteNonQuery();

                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool InsertHashtag(Hashtags hashtags)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand(INSERT_HASHTAGS_SQL, conn);

                    cmd.Parameters.Add(
                        new SqlParameter("@HASHTAG", hashtags.HASHTAG)
                        {
                            SqlDbType = System.Data.SqlDbType.NVarChar
                        }
                     );
                    cmd.Parameters.Add(
                        new SqlParameter("@UID", hashtags.UID)
                        {
                            SqlDbType = System.Data.SqlDbType.Int
                        }
                     );
                    cmd.Parameters.Add(
                        new SqlParameter("@TOWN", hashtags.TOWN)
                        {
                            SqlDbType = System.Data.SqlDbType.NVarChar
                        }
                     );

                    cmd.ExecuteNonQuery();

                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        #endregion

        #region UpdateFunctions

        public bool UpdateEvent(int ID, Events events)
        {
            try
            {
                using (SqlConnection conn =
                    new SqlConnection(ConnectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(
                            UPDATE_EVENTS_SQL, conn))
                    {
                        cmd.Parameters.AddWithValue("@ID", ID);
                        cmd.Parameters.AddWithValue("@EVENTNAME", events.EVENTNAME);
                        cmd.Parameters.AddWithValue("@DESCRIPTION", events.DESCRIPTION);
                        cmd.Parameters.AddWithValue("@FROM", events.FROM);
                        cmd.Parameters.AddWithValue("@TO", events.TO);
                        cmd.Parameters.AddWithValue("@ONLINE", events.ONLINE);
                        cmd.Parameters.AddWithValue("@TOWN", events.TOWN);
                        cmd.Parameters.AddWithValue("@PLACE", events.PLACE);
                        cmd.Parameters.AddWithValue("@MDESCRIPTION", events.MDESCRIPTION);
                        cmd.Parameters.AddWithValue("@HOWMANY", events.HOWMANY);
                        cmd.Parameters.AddWithValue("@MEETINGCORD", events.HOWMANY);
                        cmd.Parameters.AddWithValue("@PLACECORD", events.HOWMANY);

                        int rows = cmd.ExecuteNonQuery();

                        if (rows == 1) return true; else return false;
                    }
                }
            }
            catch (SqlException)
            {
                return false;
            }
        }

        public bool UpdateFriend(int ID, Friends friend)
        {
            try
            {
                using (SqlConnection conn =
                    new SqlConnection(ConnectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(
                            UPDATE_FRIENDS_SQL, conn))
                    {
                        cmd.Parameters.AddWithValue("@ID", ID);
                        cmd.Parameters.AddWithValue("@SUID", friend.SUID);
                        cmd.Parameters.AddWithValue("@GUID", friend.GUID);

                        int rows = cmd.ExecuteNonQuery();

                        if (rows == 1) return true; else return false;
                    }
                }
            }
            catch (SqlException)
            {
                return false;
            }
        }

        public bool UpdateHashtag(int ID, Hashtags hastag)
        {
            try
            {
                using (SqlConnection conn =
                    new SqlConnection(ConnectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(
                            UPDATE_HASHTAGS_SQL, conn))
                    {
                        cmd.Parameters.AddWithValue("@ID", ID);
                        cmd.Parameters.AddWithValue("@UID", hastag.UID);
                        cmd.Parameters.AddWithValue("@HASHTAG", hastag.HASHTAG);
                        cmd.Parameters.AddWithValue("@TOWN", hastag.TOWN);

                        int rows = cmd.ExecuteNonQuery();

                        if (rows == 1) return true; else return false;
                    }
                }
            }
            catch (SqlException)
            {
                return false;
            }
        }

        public bool UpdateUser(int ID, User user)
        {
            try
            {
                using (SqlConnection conn =
                    new SqlConnection(ConnectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(
                            UPDATE_USER_SQL, conn))
                    {
                        cmd.Parameters.AddWithValue("@ID", ID);
                        cmd.Parameters.AddWithValue("@EMAIL", user.EMAIL);
                        cmd.Parameters.AddWithValue("@FIRSTNAME", user.FIRSTNAME);
                        cmd.Parameters.AddWithValue("@LASTNAME", user.LASTNAME);
                        cmd.Parameters.AddWithValue("@BORNDATE", user.BORNDATE);
                        cmd.Parameters.AddWithValue("@PROFILEPICTURE", user.PROFILEPICTURE);
                        cmd.Parameters.AddWithValue("@PASSWORD", user.PASSWORD);

                        int rows = cmd.ExecuteNonQuery();
                        
                        if (rows == 1) return true;
                        return false;
                    }
                }
            }
            catch (SqlException)
            {
                return false;
            }
        }

        #endregion
    }
}