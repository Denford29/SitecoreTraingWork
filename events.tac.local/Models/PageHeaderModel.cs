using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace events.tac.local.Models
{
    public class PageHeaderModel
    {
        /// <summary>
        /// get or set a list of child navigation items
        /// </summary>
        public IList<MenuItemModel> MenuItems
        {
            get;
            set;
        }
        = new List<MenuItemModel>();

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
        public IList<SelectListItem> Languages
        {
            get;
            set;
        }
    }
}