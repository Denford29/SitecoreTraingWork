using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace events.tac.local.Models
{
    public class FeaturedEvent
    {
        public FeaturedEvent()
        {
        }

        public HtmlString EventHeading
        {
            get;
            set;
        }

        public HtmlString EventIntro
        {
            get;
            set;
        }

        public HtmlString EventImage
        {
            get;
            set;
        }

        public string CssClass
        {
            get;
            set;
        }

        public string UrlLink
        {
            get;
            set;
        }
    }
}