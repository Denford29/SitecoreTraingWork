using System.Collections.Generic;
using SATC.SC.Framework.Navigation.Models;

namespace events.tac.local.Models
{
    /// <summary>
    /// class for a single menu item with children, inherits the navigation items properties
    /// </summary>
    public class MenuItemModel : NavigationItemModel
    {
        /// <summary>
        /// get or set a list of child navigation items
        /// </summary>
        public IList<NavigationItemModel> ChildItems
        {
            get;
            set;
        }
        = new List<NavigationItemModel>();
    }
}