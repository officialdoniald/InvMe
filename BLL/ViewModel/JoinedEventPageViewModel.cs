using Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BLL.ViewModel
{
    public class JoinedEventPageViewModel
    {
        public List<BindableEvent> GetAttendedEvents()
        {
            List<Attended> attended = new List<Attended>();
            List<BindableEvent> bindableEventList = new List<BindableEvent>();
            List<Events> eventsFromDB = new List<Events>();

            attended = GlobalVariables.DatabaseConnection.GetAttendedByID(GlobalVariables.ActualUser.ID);

            if (attended is null || attended.Count == 0)
            {
                return new List<BindableEvent>();
            }

            foreach (var item in attended)
            {
                Events events = GlobalVariables.DatabaseConnection.GetEventByID(item.EVENTID);

                if (events != null)
                {
                    eventsFromDB.Add(events);
                }
            }

            eventsFromDB = eventsFromDB.OrderBy(d => d.FROM).ToList();

            DateTime dts = DateTime.Now.ToLocalTime();

            foreach (var item in eventsFromDB)
            {
                item.FROM = item.FROM.ToLocalTime();
                item.TO = item.TO.ToLocalTime();

                if ((dts >= item.FROM && dts < item.TO) || (dts < item.TO || (item.FROM == item.TO && dts < item.FROM)) || (dts <= item.FROM && dts > item.TO))
                {
                    BindableEvent bindableEvent = new BindableEvent()
                    {
                        Event = item,
                        EVENTNAME = item.EVENTNAME,
                        FROM = item.FROM.ToString(GlobalVariables.DateFormatForEventsList),
                        DAY = item.FROM.ToString("dd"),
                        MONTH = item.FROM.ToString("MMM"),
                        TIME = item.FROM.ToString("h:mm tt")
                    };
                    
                    if (item.ONLINE == 1) bindableEvent.TOWN = "Online";
                    else bindableEvent.TOWN = item.TOWN + ", " + item.PLACE;

                    if (item.FROM == item.TO) bindableEvent.TO = "No matter";
                    else bindableEvent.TO = item.TO.ToString(GlobalVariables.DateFormatForEventsList);

                    bindableEventList.Add(bindableEvent);
                }
            }

            return bindableEventList;
        }
    }
}