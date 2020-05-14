using BLL.Helper;
using Model;
using System;
using System.Net;
using System.Threading.Tasks;

namespace BLL.ViewModel
{
    public class UpdateProfilePageViewModel
    {
        private string UpdateUser(User user)
        {
            if (GlobalVariables.DatabaseConnection.UpdateUser(user))
            {
                GlobalVariables.ActualUser = user;

                return string.Empty;
            }
            else return GlobalVariables.Language.SomethingWentWrong();
        }

        public async Task<string> UpdateEmailAsync(string newEmail)
        {
            if (GlobalVariables.ActualUser.EMAIL == newEmail)
            {
                return GlobalVariables.Language.ThisIsYourActualEmail();
            }
            if (!string.IsNullOrEmpty(newEmail))
            {
                GlobalVariables.ActualUser.EMAIL = newEmail;

                User checkEmailExist = GlobalVariables.DatabaseConnection.GetUserByEMAIL(newEmail);

                if (!string.IsNullOrEmpty(checkEmailExist.EMAIL))
                {
                    return GlobalVariables.Language.ThisEmailIsAlreadyExist();
                }
                else
                {
                    GlobalVariables.ActualUsersEmail = GlobalVariables.ActualUser.EMAIL;

                    return UpdateUser(GlobalVariables.ActualUser);
                }
            }

            return GlobalVariables.Language.SomethingWentWrong();
        }

        public string UpdateProfile(string firstname, string lastname, DateTime bornDate)
        {
            if (!string.IsNullOrEmpty(firstname))
            {
                GlobalVariables.ActualUser.FIRSTNAME = firstname;
            }
            if (!string.IsNullOrEmpty(lastname))
            {
                GlobalVariables.ActualUser.LASTNAME = lastname;
            }

            GlobalVariables.ActualUser.BORNDATE = bornDate;

            return UpdateUser(GlobalVariables.ActualUser);
        }

        public string UpdateProfilePicture(bool addedPhoto)
        {
            if (addedPhoto)
            {
                GlobalVariables.ActualUser.PROFILEPICTURE = GlobalFunctionsContainer.ReadFully(GlobalVariables.SelectedImageFromGallery);
            }
            else GlobalVariables.ActualUser.PROFILEPICTURE = null;

            return UpdateUser(GlobalVariables.ActualUser);
        }

        public string UpdatePassword(string oldpassword, string newPassword)
        {
            if (string.IsNullOrEmpty(oldpassword) || string.IsNullOrEmpty(newPassword))
            {
                return GlobalVariables.Language.PasswordEntryIsEmpty();
            }

            if (GlobalVariables.ActualUser.PASSWORD != GlobalFunctionsContainer.EncryptPassword(oldpassword))
            {
                return GlobalVariables.Language.BadOldPassword();
            }

            if (newPassword.Length < 6 && newPassword.Length > 16)
            {
                return GlobalVariables.Language.BadPasswordLength();
            }

            GlobalVariables.ActualUser.PASSWORD = GlobalFunctionsContainer.EncryptPassword(newPassword);

            return UpdateUser(GlobalVariables.ActualUser);
        }
    }
}