using Model;
using Xamarin.Forms;

namespace BLL.Xamarin
{
    public class UserWithProfilePictureAndName
    {
        public int ID { get; set; }

        public string FIRSTNAME { get; set; }

        public ImageSource PROFILEPICTURE { get; set; }

        public BlockedPeople blockedPeople { get; set; }

        public User user { get; set; }
    }
}