using System.Collections.Generic;

namespace SATC.SC.Framework.Navigation.Models
{
    /// <summary>
    /// This is the core model used to create a general navigation menu with cascading children of type navigation menu as well
    /// This also inherits from the mavigation so each navigation menu item has got properties like url and title.
    /// </summary>
    public class NavigationMenuModel : NavigationItemModel
    {
        /// <summary>
        /// Create a list children items for the navigation menu item models which are of the same type as current model i.e. navigation menu
        /// </summary>
        public IEnumerable<NavigationMenuModel> Children
        {
            get;
            set;
        }
    }
}
