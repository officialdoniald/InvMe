using Model;
using System.Collections.Generic;

namespace BLL.ViewModel
{
    public class EventDescriptionPageViewModel
    {
        public bool GetAttended(int eventid)
        {
            GlobalVariables.Attended = GlobalVariables.DatabaseConnection.GetAttendedByUserAnEventID(GlobalVariables.ActualUser.ID, eventid);

            if (GlobalVariables.Attended != null && GlobalVariables.Attended.ID != 0)
            {
                return true;
            }

            return false;
        }

        public List<Attended> GetAttendedByEventID(int id)
        {
            return GlobalVariables.DatabaseConnection.GetAttendedByEventID(id);
        }

        public bool DeleteAttended(Attended attended)
        {
            return GlobalVariables.DatabaseConnection.DeleteAttendedByAttended(attended);
        }

        public bool InsertAttended(Attended attended)
        {
            return GlobalVariables.DatabaseConnection.InsertAttended(attended);
        }

        public User GetUserByID(int id)
        {
            return GlobalVariables.DatabaseConnection.GetUserByID(id);
        }

        public bool ReportEvent(Events events)
        {
            events.REPORTED = 1;

            bool reported = GlobalVariables.DatabaseConnection.UpdateEvent(events);

            if (reported)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}