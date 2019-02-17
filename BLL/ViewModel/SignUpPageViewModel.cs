using BLL.Helper;
using Model;
using System;
using System.Net;
using System.Threading.Tasks;

namespace BLL.ViewModel
{
    public class SignUpPageViewModel
    {
        public async Task<string> SignUp(User user)
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

            if (isItAUser is null)
            {
                GlobalVariables.GlobalCasualImage = GlobalVariables.DatabaseConnection.GetGlobalCasualImage();

                user.PASSWORD = GlobalFunctionsContainer.EncryptPassword(user.PASSWORD);
                user.PROFILEPICTURE = GlobalVariables.GlobalCasualImage;

                var success = GlobalVariables.DatabaseConnection.InsertUser(user);

                if (success)
                {
                    try
                    {
                        string name = string.Format("{0} {1}", user.FIRSTNAME, user.LASTNAME);

                        string url = String.Format("http://invme.hu/invmeapp/registration.php?emaill={0}&nev={1}", user.EMAIL, name);
                        Uri uri = new Uri(url);
                        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                        request.Method = "GET";
                        WebResponse res = await request.GetResponseAsync();
                    }
                    catch (Exception)
                    {
                    }

                    return string.Empty;
                }
            }
            else
            {
                return GlobalVariables.Language.ThisEmailIsAlreadyExist();
            }

            return GlobalVariables.Language.SomethingWentWrong();
        }
    }
}