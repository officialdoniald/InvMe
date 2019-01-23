using Model;
using System.Collections.Generic;

namespace BLL.ViewModel
{
    public class HashtagsPageViewModel
    {
        public List<Hashtags> GetHashtagsByUserID()
        {
            return GlobalVariables.DatabaseConnection.GetHashtagByUSERID(GlobalVariables.ActualUser.ID);
        }

        public string DeleteHashtag(Hashtags listItem)
        {
            return GlobalVariables.DatabaseConnection.DeleteHashtag(listItem) ? string.Empty : GlobalVariables.Language.SomethingWentWrong();
        }

        public string InsertHashtag(Hashtags listItem)
        {
            return GlobalVariables.DatabaseConnection.InsertHashtag(listItem) ? string.Empty : GlobalVariables.Language.SomethingWentWrong();
        }
    }
}