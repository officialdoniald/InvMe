using Model;
using System.Collections.Generic;

namespace BLL.ViewModel
{
    public class HomePageViewModel
    {
        public List<User> GetUser()
        {
            return GlobalVariables.DatabaseConnection.GetUser();
        }

        public List<Events> GetEvent()
        {
            return GlobalVariables.DatabaseConnection.GetEvent();
        }
    }
}