using BLL.Helper;
using System;

namespace BLL.ViewModel
{
    public class LoginPageViewModel
    {
        public string LogIn(string EMAIL, string PASSWORD)
        {
            if (String.IsNullOrEmpty(EMAIL) && String.IsNullOrEmpty(PASSWORD))
            {
                return GlobalVariables.Language.FillAllEntries();
            }
            if (String.IsNullOrEmpty(EMAIL))
            {
                return GlobalVariables.Language.EmailEntryIsEmpty();
            }
            if (String.IsNullOrEmpty(PASSWORD))
            {
                return GlobalVariables.Language.PasswordEntryIsEmpty();
            }
            if (!GlobalFunctionsContainer.IsValidEmailAddress(EMAIL.ToLower()))
            {
                return GlobalVariables.Language.BadEmailFormat();
            }
            if (PASSWORD.Length < 6 || PASSWORD.Length > 16)
            {
                return GlobalVariables.Language.BadPasswordLength();
            }

            var user = GlobalVariables.DatabaseConnection.GetUserByEMAIL(EMAIL.ToLower());

            if (user is null || user.PASSWORD != GlobalFunctionsContainer.EncryptPassword(PASSWORD))
            {
                return GlobalVariables.Language.WrongEmailOrPassword();
            }

            return string.Empty;
        }
    }
}