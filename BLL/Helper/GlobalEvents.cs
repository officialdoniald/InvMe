using Model;
using System;

namespace BLL.Helper
{
    public static class GlobalEvents
    {
        public static event EventHandler<object> OnLogin;

        public static void OnLogin_Event(object sender, object args)
        {
            AppConstants.SubscriptionTags[0] = GlobalVariables.ActualUser.EMAIL.ToLower();
            AppConstants.SubscriptionTags[1] = "default";

            OnLogin?.Invoke(sender, args);
        }

        public static event EventHandler<object> OnLogoff;

        public static void OnLogoff_Event(object sender, object args)
        {
            AppConstants.SubscriptionTags[0] = "asd";
            AppConstants.SubscriptionTags[1] = "default";

            OnLogoff?.Invoke(sender, args);
        }

        public static event EventHandler<Events> OnNoificationClicked;

        public static void OnNoificationClicked_Event(object sender, Events args)
        {
            OnNoificationClicked?.Invoke(sender, args);
        }

    }
}