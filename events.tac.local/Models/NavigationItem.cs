using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace events.tac.local.Models
{
    public class NavigationItem
    {
        public string Title
        {
            get;
            set;
        }

        public string Url
        {
            get;
            set;
        }

        public bool Active
        {
            get;
            set;
        }
    }
}