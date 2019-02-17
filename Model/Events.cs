using System;

namespace Model
{
    /// <summary>
    /// Events
    /// </summary>
    public class Events
    {
        /// <summary>
        /// ID
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// DESCRIPTION
        /// </summary>
        public string DESCRIPTION { get; set; }

        /// <summary>
        /// EVENTNAME
        /// </summary>
        public string EVENTNAME { get; set; }

        /// <summary>
        /// FROM
        /// </summary>
        public DateTimeOffset FROM { get; set; }

        /// <summary>
        /// TO
        /// </summary>
        public DateTimeOffset TO { get; set; }

        /// <summary>
        /// TOWN
        /// </summary>
        public string TOWN { get; set; }

        /// <summary>
        /// PLACE
        /// </summary>
        public string PLACE { get; set; }

        /// <summary>
        /// MDESCRIPTION
        /// </summary>
        public string MDESCRIPTION { get; set; }

        /// <summary>
        /// HOWMANY
        /// </summary>
        public int HOWMANY { get; set; }

        /// <summary>
        /// ONLINE
        /// </summary>
        public int ONLINE { get; set; }

        /// <summary>
        /// MEETINGCORD
        /// </summary>
        public string MEETINGCORD { get; set; }

        /// <summary>
        /// PLACECORD
        /// </summary>
        public string PLACECORD { get; set; }

        /// <summary>
        /// REPORTED
        /// </summary>
        public int REPORTED { get; set; }
    }
}