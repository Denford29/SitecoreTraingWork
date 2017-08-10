using System.Collections.Generic;
using System.Web;
using SATC.SC.Framework.Navigation.Models;
using SATC.SC.Framework.SitecoreHelpers.Models;

namespace events.tac.local.Models
{
    public class PageHeaderModel
    {

        /// <summary>
        /// get or set the home page item
        /// </summary>
        public string HomePageUrl
        {
            get;
            set;
        }

        /// <summary>
        /// get or set a list of child navigation items
        /// </summary>
        public List<SiteMenuModel> MenuItems
        {
            get;
            set;
        }
        = new List<SiteMenuModel>();

        /// <summary>
        /// get or set the logo image html string
        /// </summary>
        public HtmlString LogoImage
        {
            get;
            set;
        }

        /// <summary>
        /// get or set the list of languages available
        /// </summary>
        public List<LanguageModel> Languages
        {
            get;
            set;
        }
        = new List<LanguageModel>();

        /// <summary>
        /// get or set the current language's model
        /// </summary>
        public LanguageModel CurrentLanguage
        {
            get;
            set;
        }

    }
}