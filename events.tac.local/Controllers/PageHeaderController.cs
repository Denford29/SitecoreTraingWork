using events.tac.local.Models;
using SATC.SC.Framework.SitecoreHelpers;
using Sitecore.Data;
using System.Web.Mvc;

namespace events.tac.local.Controllers
{
    public class PageHeaderController : Controller
    {
        /// <summary>
        /// Get the default menu items and other settings
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            //get the master database to use to get items from
            var database = Sitecore.Configuration.Factory.GetDatabase("master");

            //create a new model
            var model = new PageHeaderModel();

            // get the default home page to use to get the menu items
            var homePage = Sitecore.Context.Database.GetItem(Sitecore.Context.Site.StartPath);

            // get the homepage id set in the config
            var homePageId = StandardHelper.GetItemIDFromConfig("HomePageID");
            if(homePageId != ID.Null)
            {
                var databaseHomePage = database.GetItem(homePageId);
                if(databaseHomePage != null)
                {
                    homePage = databaseHomePage;
                }
            }

            //

            return View();
        }
    }
}