using BLL.Helper;
using Model;
using System;
using System.Net;
using System.Threading.Tasks;

namespace BLL.ViewModel
{
    public class ForgotPasswordPageViewModel
    {
        public string SendEmailAsync(string EMAIL)
        {
            if (string.IsNullOrEmpty(EMAIL))
            {
                return GlobalVariables.Language.EmailEntryIsEmpty();
            }

            EMAIL = EMAIL.ToLower();

            User user = GlobalVariables.DatabaseConnection.GetUserByEMAIL(EMAIL);

            if (user.EMAIL is null)
            {
                return GlobalVariables.Language.NoAccountFoundWithThisEmil();
            }

            bool success = GlobalVariables.DatabaseConnection.ForgotPasswordAsync(user);
            
            if (!success)
            {
                return GlobalVariables.Language.SomethingWentWrong();
            }

            return string.Empty;
        }
    }
}
