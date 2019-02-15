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

                int success = GlobalVariables.DatabaseConnection.InsertEventAsync(events);

                if (success == -1)
                {
                    return GlobalVariables.Language.SomethingWentWrong();
                }
                else
                {
                    Attended attend = new Attended();
                    attend.USER_ID = GlobalVariables.ActualUser.ID;
                    attend.EVENT_ID = success;

                    bool successq = GlobalVariables.DatabaseConnection.InsertAttended(attend);

                    if (successq)
                    {
                        GlobalFunctionsContainer.SendEmail("eventcreate", GlobalVariables.ActualUser.EMAIL, GlobalVariables.ActualUser.FIRSTNAME, GlobalVariables.ActualUser.LASTNAME, events.EVENTNAME, events.FROM.ToString(GlobalVariables.DateFormatForEventsAddAndDescription), events.TO.ToString(GlobalVariables.DateFormatForEventsAddAndDescription), events.TOWN, events.PLACE);
                        
                        List<Hashtags> hashtags = new List<Hashtags>();

                        hashtags = GlobalVariables.DatabaseConnection.GetHashtag();

                        List<User> usersHowsOkay = new List<User>();

                        foreach (var item in hashtags)
                        {
                            if (item.UID != GlobalVariables.ActualUser.ID)
                            {
                                if (
                                    (!string.IsNullOrEmpty(item.TOWN.ToLower()) &&
                                    item.TOWN.ToLower().Contains(item.TOWN.ToLower())
                                    ) ||
                                    (!string.IsNullOrEmpty(item.HASHTAG.ToLower()) &&
                                    (events.EVENTNAME.ToLower().Contains(item.HASHTAG.ToLower()) ||
                                    events.DESCRIPTION.ToLower().Contains(item.HASHTAG.ToLower())))
                                    )
                                {
                                    User user = new User();
                                    user = GlobalVariables.DatabaseConnection.GetUserByID(item.UID);

                                    bool wasThat = false;

                                    foreach (var useritem in usersHowsOkay)
                                    {
                                        if (useritem.ID == user.ID)
                                        {
                                            wasThat = true;
                                            break;
                                        }
                                    }

                                    if (!wasThat)
                                    {
                                        usersHowsOkay.Add(user);
                                    }
                                }
                            }
                        }

                        foreach (var item in usersHowsOkay)
                        {
                            if (item.ID != GlobalVariables.ActualUser.ID)
                            {
                                GlobalFunctionsContainer.SendEmail("sendmailtosubscribeduser", item.EMAIL, item.FIRSTNAME, item.LASTNAME, events.EVENTNAME, events.FROM.ToString(GlobalVariables.DateFormatForEventsAddAndDescription), events.TO.ToString(GlobalVariables.DateFormatForEventsAddAndDescription), events.TOWN, events.PLACE);
                            }
                        }
                    }
                }
                return string.Empty;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}