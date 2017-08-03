using System.Collections.Generic;

namespace events.tac.local.Models
{
    /// <summary>
    /// class for a single menu item with children, inherits the navigation items properties
    /// </summary>
    public class MenuItemModel : NavigationItem
    {
        /// <summary>
        /// get or set a list of child navigation items
        /// </summary>
        public IList<NavigationItem> ChildItems
        {
            get;
            set;
        }
        = new List<NavigationItem>();
    }
}