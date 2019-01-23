using BLL.Helper;
using Model;
using System;
using System.Net;
using System.Threading.Tasks;

namespace BLL.ViewModel
{
    public class ForgotPasswordPageViewModel
    {
        public async Task<string> SendEmailAsync(string EMAIL)
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

            string passwordWithoutEncrypt = GlobalFunctionsContainer.RandomString(8, false);

            user.PASSWORD = GlobalFunctionsContainer.EncryptPassword(passwordWithoutEncrypt);

            bool success = GlobalVariables.DatabaseConnection.UpdateUser(user.ID, user);

            if (!success)
            {
                return GlobalVariables.Language.SomethingWentWrong();
            }

            try
            {
                string url = String.Format("http://invme.hu/invmeapp/forgotpassword.php?emaill={0}&nev={1}&pwdd={2}", user.EMAIL, user.FIRSTNAME + "_" + user.LASTNAME, passwordWithoutEncrypt);

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
}
