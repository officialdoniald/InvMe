using BLL.Helper;
using Model;
using System;
using System.Net;
using System.Threading.Tasks;

namespace BLL.ViewModel
{
    public class SignUpPageViewModel
    {
        public string SignUp(User user)
        {
            if (string.IsNullOrEmpty(user.EMAIL) || string.IsNullOrEmpty(user.FIRSTNAME) ||
                string.IsNullOrEmpty(user.LASTNAME) || string.IsNullOrEmpty(user.PASSWORD))
            {
                return GlobalVariables.Language.FillAllEntries();
            }
            if (user.PASSWORD.Length < 6 || user.PASSWORD.Length > 16)
            {
                return GlobalVariables.Language.BadPasswordLength();
            }

            user.EMAIL = user.EMAIL.ToLower();

            var isItAUser = GlobalVariables.DatabaseConnection.GetUserByEMAIL(user.EMAIL);

            if (isItAUser is null || string.IsNullOrEmpty(isItAUser.EMAIL))
            {
                GlobalVariables.GlobalCasualImage = GlobalVariables.DatabaseConnection.GetGlobalCasualImage();

                user.PASSWORD = GlobalFunctionsContainer.EncryptPassword(user.PASSWORD);
                user.PROFILEPICTURE = GlobalVariables.GlobalCasualImage;

                var success = GlobalVariables.DatabaseConnection.InsertUser(user);

                if (success)
                {
                    return string.Empty;
                }
                else
                {
                    return GlobalVariables.Language.SomethingWentWrong();
                }
            }
            else
            {
                return GlobalVariables.Language.ThisEmailIsAlreadyExist();
            }
        }
    }
}