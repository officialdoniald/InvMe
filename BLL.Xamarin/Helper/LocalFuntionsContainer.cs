using Model;
using System;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace BLL.Xamarin.Helper
{
    public class LocalFuntionsContainer
    {
        /// <summary>
        /// Initialize the userloc.txt file.
        /// </summary>
        public async static void InitFirstUserLocationRequestFile()
        {
            try
            {
                GlobalVariables.AutomaticUserLocation = await SecureStorage.GetAsync(LocalVariablesContainer.userlocationfile) == "0" ? false : true;
            }
            catch (Exception)
            {
                await SecureStorage.SetAsync(LocalVariablesContainer.userlocationfile, 0.ToString());
            }
        }

        /// <summary>
        /// Initializes the users email.
        /// </summary>
        public async static void InitializeUsersEmail()
        {
            try
            {
                GlobalVariables.ActualUsersEmail = await SecureStorage.GetAsync(LocalVariablesContainer.logintxt);

                if (!string.IsNullOrEmpty(GlobalVariables.ActualUsersEmail))
                {
                    User user = GlobalVariables.DatabaseConnection.GetUserByEMAIL(GlobalVariables.ActualUsersEmail);

                    if (user is null || string.IsNullOrEmpty(user.EMAIL))
                    {
                        GlobalVariables.HaveToLogin = true;
                    }
                    else
                    {
                        GlobalVariables.HaveToLogin = false;
                    }

                }
                else
                {
                    GlobalVariables.HaveToLogin = true;
                }
            }
            catch (Exception)
            {
                GlobalVariables.HaveToLogin = true;
            }
        }

        /// <summary>
        /// Initializes the user.
        /// </summary>
        public static void InitializeUser()
        {
            GlobalVariables.ActualUser = GlobalVariables.DatabaseConnection.GetUserByEMAIL(GlobalVariables.ActualUsersEmail);
        }

        /// <summary>
        /// Initializes the users email variable.
        /// </summary>
        public async static void InitializeUsersEmailVariable()
        {
            GlobalVariables.ActualUsersEmail = await SecureStorage.GetAsync(LocalVariablesContainer.logintxt);
        }


    }
}
