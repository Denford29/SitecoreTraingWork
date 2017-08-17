using System.Collections.Generic;

namespace SATC.SC.Framework.Navigation.Models
{

    /// <summary>
    /// This is the base namespace for all SATC SC Framework's navigation related models
    /// </summary>
    [System.Runtime.CompilerServices.CompilerGenerated]
    class NamespaceDoc
    {

    }

    /// <summary>
    /// This is the core site menu model which can be used to create menu items with the core
    /// navigation properties as well as a list of imeediate children.
    /// </summary>
    public class SiteMenuModel
    {
        /// <summary>
        /// Get or set the navigation item title, used as the display for the navigation item link
        /// </summary>
        public string Title
        {
            get;
            set;
        }

        /// <summary>
        /// Get or set the navigation item url, used as the hrefs link for the navigation item link
        /// </summary>
        public string Url
        {
            get;
            set;
        }

        /// <summary>
        /// Get or set an active flag for the navigation item url, can be used to set the current item as the active one i.e. 
        /// the currently selected one or the one that matches the current page you are on.
        /// </summary>
        public bool Active
        {
            get;
            set;
        }

        /// <summary>
        /// Create a list children items for the site menu item models which uses the navigation item model for the children
        /// </summary>
        public List<NavigationItemModel> Children
        {
            get;
            set;
        }
            = new List<NavigationItemModel>();
    }
}
