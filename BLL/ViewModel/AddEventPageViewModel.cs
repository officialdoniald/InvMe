using BLL.Helper;
using Model;
using System;
using System.Collections.Generic;

namespace BLL.ViewModel
{
    public class AddEventPageViewModel
    {
        public string AddEvent(Events events, bool onlineEvent, bool noMatterHowManyPerson, string howmanyPersonEntrytext)
        {
            try
            {
                if (string.IsNullOrEmpty(events.EVENTNAME))
                {
                    return GlobalVariables.Language.FillAllEntries();
                }

                if (!onlineEvent && (string.IsNullOrEmpty(events.TOWN) || string.IsNullOrEmpty(events.PLACE)))
                {
                    return GlobalVariables.Language.FillAllEntries();
                }
                else if (!onlineEvent && (GlobalVariables.EventCord == "" || GlobalVariables.MeetingCord == ""))
                {
                    return GlobalVariables.Language.YouHaveToPickThePlaceAndMeetingOnTheMap();
                }

                if (!noMatterHowManyPerson && string.IsNullOrEmpty(howmanyPersonEntrytext))
                {
                    return GlobalVariables.Language.FillAllEntries();
                }
                else if (!noMatterHowManyPerson)
                {
                    try
                    {
                        events.HOWMANY = Convert.ToInt32(howmanyPersonEntrytext);
                    }
                    catch (Exception)
                    {
                        return GlobalVariables.Language.EnterValidNumber();
                    }
                }

                DateTimeOffset dto_begin = events.FROM.ToLocalTime();

                DateTimeOffset dto_end = events.TO.ToLocalTime();

                if (dto_begin < DateTimeOffset.Now.ToLocalTime())
                {
                    return GlobalVariables.Language.HaveToBiggerBeginDateThanCurrentDate();
                }

                if (dto_begin > dto_end)
                {
                    return GlobalVariables.Language.HaveToBiggerBeginDateThanEndDate();
                }

                if (dto_end < DateTimeOffset.Now.ToLocalTime())
                {
                    return GlobalVariables.Language.HaveToBiggerEndDateThanCurrentDate();
                }

                events.MDESCRIPTION = "";
                events.DESCRIPTION = "";
                events.CREATEUID = GlobalVariables.ActualUser.ID;

                int success = GlobalVariables.DatabaseConnection.InsertEventAsync(events);

                if (success == 0)
                {
                    return GlobalVariables.Language.SomethingWentWrong();
                }
                else
                {
                    return string.Empty;
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}