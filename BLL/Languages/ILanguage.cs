namespace BLL.Languages
{
    public interface ILanguage
    {
        string AppName();

        string Warning();

        string OK();

        string FillAllEntries();

        string SomethingWentWrong();

        string Success();

        string SuccessAddedd();

        string EnterValidNumber();

        string HaveToBiggerBeginDateThanEndDate();
        
        string HaveToBiggerBeginDateThanCurrentDate();

        string HaveToBiggerEndDateThanCurrentDate();

        string SuccessFulUnSubscibed();

        string SuccessFulSubsrcibed();

        string FillTheHashtagEntry();

        string FillTheTownEntry();

        string NoPickingFromGallery();

        string SuccessFulUpdatedProfile();

        string AreYouSure();

        string Cancel();

        string Delete();

        string EmailEntryIsEmpty();

        string PasswordEntryIsEmpty();

        string BadEmailFormat();

        string BadPasswordLength();

        string WrongEmailOrPassword();

        string NoAccountFoundWithThisEmil();

        string ThisEmailIsAlreadyExist();

        string ThisIsYourActualEmail();

        string BadOldPassword();

        string YouHaveToPickThePlaceAndMeetingOnTheMap();

        string SuccessReg();

        string BlockedUser();

        string NeedUserLocation();
    }
}