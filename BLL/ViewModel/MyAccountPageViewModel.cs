namespace BLL.ViewModel
{
    public class MyAccountPageViewModel
    {
        public string DeleteAccount()
        {
            if (!GlobalVariables.DatabaseConnection.DeleteUser(GlobalVariables.ActualUser))
            {
                return GlobalVariables.Language.SomethingWentWrong();
            }
            else
            {
                return string.Empty;
            }
        }
    }
}