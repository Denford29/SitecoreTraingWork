namespace SATC.SC.Framework.Navigation.Models
{

    /// <summary>
    /// This is the core model used to create a generic navigation item, this can be used individually or 
    /// as an inherited class to provide these core navigation items e.g. ulr and title.
    /// </summary>
    public class NavigationItemModel
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
    }
}
