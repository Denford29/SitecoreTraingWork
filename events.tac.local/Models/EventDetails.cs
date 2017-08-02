using Sitecore.ContentSearch.SearchTypes;
using Sitecore.Links;
using Sitecore.Web.UI.WebControls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace events.tac.local.Models
{
    public class EventDetails : SearchResultItem
    {
        public string ContentHeading
        {
            get;
            set;
        }

        public string ContentIntro
        {
            get;
            set;
        }

        public DateTime Date
        {
            get;
            set;
        }

        public HtmlString EventImage
        {
            get
            {
                return new HtmlString(FieldRenderer.Render(GetItem(), "Event-Image", "mw=150"));
            }
        }

        public string urllink
        {
            get;
            set;
        }

        public HtmlString ItemUrl
        {
            get
            {
                return new HtmlString(LinkManager.GetItemUrl(GetItem()));
            }
        }
    }
}