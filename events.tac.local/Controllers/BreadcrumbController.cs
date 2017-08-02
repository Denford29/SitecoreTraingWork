using events.tac.local.Models;
using Sitecore.Data.Items;
using Sitecore.Links;
using Sitecore.Mvc.Presentation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace events.tac.local.Controllers
{
    public class BreadcrumbController : Controller
    {
        // GET: Breadcrumb
        public ActionResult Index()
        {
            return View(CreateModel());
        }

        public IEnumerable<NavigationItem> CreateModel()
        {
            // create the default navigation list to populate later
            IEnumerable<NavigationItem> navigationList = new List<NavigationItem>();
            // get the current item
            var currentItem = RenderingContext.Current.ContextItem;

            // get the home item from the database using the sitecore context site start path
            var homeItem = Sitecore.Context.Database.GetItem(Sitecore.Context.Site.StartPath);

            if(homeItem != null && currentItem != null)
            {
                //get the parent items up to home
                var breadcrumbItems = RenderingContext.Current.ContextItem.Axes.GetAncestors().
                                                         Where(item => item.Axes.IsDescendantOf(homeItem)).
                                                         Concat(new Item[] { currentItem }).
                                                         ToList();

                if(breadcrumbItems.Any())
                {
                    navigationList = breadcrumbItems.Select(
                        item => new NavigationItem
                        {
                            Title = item.DisplayName,
                            Url = LinkManager.GetItemUrl(item),
                            Active = (item.ID == currentItem.ID)
                        });
                }
            }

            return navigationList;
        } 
    }
}