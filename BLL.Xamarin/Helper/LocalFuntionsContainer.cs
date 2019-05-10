using BLL.Xamarin.FileStoreAndLoad;
using Model;
using System;
using Xamarin.Forms;

namespace BLL.Xamarin.Helper
{
    public class LocalFuntionsContainer
    {
        /// <summary>
        /// Initialize the userloc.txt file.
        /// </summary>
        public static void InitFirstUserLocationRequestFile()
        {
            try
            {
                GlobalVariables.AutomaticUserLocation = DependencyService.Get<IFileStoreAndLoad>().LoadText(LocalVariablesContainer.userlocationfile) == "0" ? false : true;
            }
            catch (Exception)
            {
                FileStoreAndLoading.InsertToFile(LocalVariablesContainer.userlocationfile, 0.ToString());
            }
        }

        /// <summary>
        /// Initializes the users email.
        /// </summary>
        public static void InitializeUsersEmail()
        {
            try
            {
                GlobalVariables.ActualUsersEmail = DependencyService.Get<IFileStoreAndLoad>().LoadText(LocalVariablesContainer.logintxt);

                if (!string.IsNullOrEmpty(GlobalVariables.ActualUsersEmail))
                {
                    User user = GlobalVariables.DatabaseConnection.GetUserByEMAIL(GlobalVariables.ActualUsersEmail);

                    if (user is null)
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
        public static void InitializeUsersEmailVariable()
        {
            GlobalVariables.ActualUsersEmail = DependencyService.Get<IFileStoreAndLoad>().LoadText(LocalVariablesContainer.logintxt);
        }


    }
}
