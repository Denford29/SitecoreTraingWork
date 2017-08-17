using System.Collections.Generic;
using SATC.SC.Framework.Navigation.Models;
using System.Web;

namespace events.tac.local.Models
{
    public class PageFooterModel
    {
        public List<NavigationItemModel> FooterLinks
        {
            get;
            set;
        }
        = new List<NavigationItemModel>();

        public HtmlString FooterHeading
        {
            get;
            set;
        }

        public HtmlString FooterIntro
        {
            get;
            set;
        }

        public HtmlString CopyrightText
        {
            get;
            set;
        }

        public string EditorDatasourcePath
        {
            get;
            set;
        }
    }
}