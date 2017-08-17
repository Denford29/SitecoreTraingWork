using Sitecore.Data.Items;
using Sitecore.Links;
using Sitecore.Mvc.Presentation;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using SATC.SC.Framework.Navigation;
using SATC.SC.Framework.Navigation.Models;
using SATC.SC.Framework.SitecoreHelpers;
using Sitecore.Data;

namespace events.tac.local.Controllers
{
    public class BreadcrumbController : Controller
    {

        /// <summary>
        /// initiate the field for the navigation helpers from the SATC SC Framework
        /// </summary>
        private readonly NavigationHelpers _navigationHelpers;

        /// <summary>
        /// inittiate the field for the standard helper from the SATC SC Framework
        /// </summary>
        private readonly StandardHelpers _standardHelper;

        /// <summary>
        /// create the constructor and assign any fields
        /// </summary>
        /// <param name="navigationHelpers"></param>
        /// <param name="standardHelper"></param>
        public BreadcrumbController
        (
            NavigationHelpers navigationHelpers,
            StandardHelpers standardHelper
        )
        {
            _navigationHelpers = navigationHelpers;
            _standardHelper = standardHelper;
        }

        /// <summary>
        /// get the initial display with the model to use in the view
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {

            // create the inital list of items
            var breadcrumbList = new List<NavigationItemModel>();
            // get the current item
            var currentItem = RenderingContext.Current.ContextItem;

            //get the master database to use to get items from
            var database = currentItem.Database;

            // get the homepage id set in the config to use to get the breadcrumb items with
            var homePageId = _standardHelper.GetItemIdFromConfig("HomePageID", currentItem);
            if (homePageId != ID.Null)
            {
                //get the home item from the config id
                var homePage = database.GetItem(homePageId);
                //check if the home item in not null
                if (homePage != null)
                {
                    //get the breadcrumb list from our framework helper
                    breadcrumbList = _navigationHelpers.CreateBreadcrumbMenu(currentItem, homePage);
                }
            }

            //retunr the list
            return View(breadcrumbList);
        }

    }
}