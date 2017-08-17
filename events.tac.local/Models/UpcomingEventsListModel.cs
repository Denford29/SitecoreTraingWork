using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace events.tac.local.Models
{
    public class UpcomingEventsListModel
    {
        public List<EventDetails> Events
        {
            get;
            set;
        }
        = new List<EventDetails>();

        public string EventsLandingUrl
        {
            get;
            set;
        }
    }
}