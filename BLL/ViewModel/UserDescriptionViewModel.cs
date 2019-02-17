using Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace BLL.ViewModel
{
    public class UserDescriptionViewModel
    {
        public bool ReportUser(User userid)
        {
            bool reported = GlobalVariables.DatabaseConnection.UpdateUserReported(userid);

            if (reported)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool IsItABlockedUser(int userid)
        {
            var blockedPeopleList = GlobalVariables.DatabaseConnection.GetBlockedPeopleByID(GlobalVariables.ActualUser.ID);

            if (blockedPeopleList != null && blockedPeopleList.Count > 0)
            {
                foreach (var item in blockedPeopleList)
                {
                    if (item.BlockedUserID == userid)
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}