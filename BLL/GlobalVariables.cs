using BLL.Languages;
using DAL;
using Model;
using System.IO;

namespace BLL
{
    public static class GlobalVariables
    {
        public static string ConnectionString { get; set; }

        public static string GlobalPassword { get; set; }

        public static string SourceSelectedImageFromGallery { get; set; }

        public static User ActualUser { get; set; }

        public static Attended Attended { get; set; }

        public static DatabaseConnection DatabaseConnection { get; set; }

        public static byte[] GlobalCasualImage { get; set; }

        public static string ActualUsersEmail { get; set; }

        public static bool HaveToLogin { get; set; }

        public static Stream SelectedImageFromGallery { get; set; }

        public static bool IsUpdatedMyProfile { get; set; }

        public static string EventCord { get; set; }

        public static string MeetingCord { get; set; }

        public static ILanguage Language { get; set; }

        public static string DateFormatForEventsList { get; set; } = "MMM dd. HH:mm";

        public static string DateFormatForEventsAddAndDescription { get; set; } = "dddd, MMM dd yyyy HH:mm zzz";

        public static string DateFormatForBornDate { get; set; } = "dd. MM. yyyy";
    }
}
