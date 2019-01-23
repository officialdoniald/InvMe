using System;
using System.Collections.Generic;
using Model;

namespace BLL.ViewModel
{
    public class AttendedUsersListPageViewModel
    {
        public List<Attended> GetAttendedByEventID(int id)
        {
            return GlobalVariables.DatabaseConnection.GetAttendedByEventID(id);
        }

        public User GetUserByID(int id)
        {
            return GlobalVariables.DatabaseConnection.GetUserByID(id);
        }
    }
}
