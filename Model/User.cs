using System;

namespace Model
{
    /// <summary>
    /// User
    /// </summary>
    public class User
    {
        /// <summary>
        /// ID
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// EMAIL
        /// </summary>
        public string EMAIL { get; set; }

        /// <summary>
        /// FIRSTNAME
        /// </summary>
        public string FIRSTNAME { get; set; }

        /// <summary>
        /// LASTNAME
        /// </summary>
        public string LASTNAME { get; set; }

        /// <summary>
        /// BORNDATE
        /// </summary>
        public DateTime BORNDATE { get; set; }

        /// <summary>
        /// PROFILEPICTURE
        /// </summary>
        public byte[] PROFILEPICTURE { get; set; }

        /// <summary>
        /// PASSWORD
        /// </summary>
        public string PASSWORD { get; set; }
    }
}