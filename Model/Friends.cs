namespace Model
{
    /// <summary>
    /// Friends
    /// </summary>
    public class Friends
    {
        /// <summary>
        /// ID
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// SUID ->ez az a user, aki bejelölte.
        /// </summary>
        public int SUID { get; set; }

        /// <summary>
        /// GUID ->ez az a user, akit bejelölt.
        /// </summary>
        public int GUID { get; set; }
    }
}