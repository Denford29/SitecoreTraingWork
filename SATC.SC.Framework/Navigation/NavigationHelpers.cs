using System.Collections.Generic;
using System.Linq;
using SATC.SC.Framework.Navigation.Models;
using Sitecore.Data.Items;
using Sitecore.Links;
using Sitecore.Mvc.Presentation;

namespace SATC.SC.Framework.Navigation
{
    /// <summary>
    /// This is the base namespace for all SATC SC Framework's navigation related methods
    /// </summary>
    [System.Runtime.CompilerServices.CompilerGenerated]
    class NamespaceDoc
    {

    }

    /// <summary>
    /// This class contains all the helpers we use to build different kinds of navigation items e.g. menus, breadcrumbs and links in general
    /// </summary>
    public class NavigationHelpers
    {
        /// <summary>
        /// Create a navigation menu item with all the nested children of the the same type i.e. navigation menu item
        /// which will also have children items of the same type i.e. navigation menu item. This type of navigation item will
        /// be handy for menus like a side navigation which will list all the child pages of that current page.
        /// </summary>
        /// <param name="root">
        /// This defines the top level item to use for the menu
        /// </param>
        /// <param name="current">
        /// This defines the current item for the menu
        /// </param>
        /// <returns>
        /// Returns the navigation menu model with the children
        /// </returns>
        public NavigationMenuModel CreateNavigationMenu(Item root, Item current)
        {
            // set the navigation menu item's properties, with children if need be
            var menu = new NavigationMenuModel()
            {
                Title = root.DisplayName,
                Url = LinkManager.GetItemUrl(root),
                Children = root.Axes.IsAncestorOf(current) ?
                                  root.GetChildren().Select
                                  (item => CreateNavigationMenu(item, current))
                                  : null
            };

            //return the menu item
            return menu;
        }

        /// <summary>
        /// create a sitem menu item model, used in the main site menu with just the immediate children list.
        /// The model itself has got the core navigation items i.e. url and title and uses the navigation item for the
        /// children. This will be handy to use for the main site navigation.
        /// </summary>
        /// <param name="currentItem">
        /// This defines the current item used by the menu
        /// </param>
        /// <param name="getChildren">
        /// This flag indicates whether you need to get the children if that current item 
        /// </param>
        /// <returns>
        /// Returns the Sitem menu model with or without children set
        /// </returns>
        public SiteMenuModel CreateSiteMenuModel(Item currentItem, bool getChildren)
        {
            // check if the current item to use in the menu is not null
            if (currentItem == null)
            {
                return null;
            }

            //create the standard menu item properties
            var menuItem = new SiteMenuModel()
            {
                Title = currentItem.DisplayName,
                Url = LinkManager.GetItemUrl(currentItem)
            };

            //check if the current item has got children and add them
            if (getChildren && currentItem.Children.Any())
            {
                menuItem.Children = currentItem.GetChildren().Select(item => new NavigationItemModel()
                {
                    Title = item.DisplayName,
                    Url = LinkManager.GetItemUrl(item),
                    Active = (item.ID == currentItem.ID)
                })
                    .ToList();
            }

            //return the menu item
            return menuItem;
        }

        /// <summary>
        /// Create a list of navigation items starting with the start item until the current item.
        /// Ideally used for a breadcrumb menu where you will list all the parent links up to the start item
        /// which is normally the home page link.
        /// </summary>
        /// <param name="currentItem">
        /// This defines the current item of the breadcrumb i.e. the last item which is the page you are on
        /// </param>
        /// <param name="startItem">
        /// This defines the start item of the breadcrumb e.g. the homepage or which ever parent item you want the breadcrumb to end on
        /// </param>
        /// <returns>
        /// Returns a list of the navigation items 
        /// </returns>
        public List<NavigationItemModel> CreateBreadcrumbMenu(Item currentItem, Item startItem)
        {
            // create the default navigation list to populate later
            var navigationList = new List<NavigationItemModel>();

            //check if the start item and current item are not null
            if (startItem != null && currentItem != null)
            {
                //get the parent items up to home
                //var allAncestors = currentItem.Axes.GetAncestors();
                //var allHomeAncestors = allAncestors.Where(item => item.Axes.IsDescendantOf(startItem));
                //var breadcrumbItems = allHomeAncestors
                //    .Concat(new Item[] { currentItem }).
                //    ToList();

                //get the parent items up to home
                var breadcrumbItems = currentItem.Axes.GetAncestors().
                    Where(item => item.Axes.IsDescendantOf(startItem)).
                    Concat(new Item[] { currentItem }).
                    ToList();



                //if we have the list of breadcrumb item, then add them to the return list
                if (breadcrumbItems.Any())
                {
                    navigationList = breadcrumbItems.Select(
                        item => new NavigationItemModel
                        {
                            Title = item.DisplayName,
                            Url = LinkManager.GetItemUrl(item),
                            Active = (item.ID == currentItem.ID)
                        })
                        .ToList();
                }
            }

            //return the list
            return navigationList;
        }

    }
}
