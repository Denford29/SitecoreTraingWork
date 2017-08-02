using events.tac.local.Models;
using Sitecore.ContentSearch;
using Sitecore.Mvc.Presentation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Sitecore.ContentSearch.Linq;
using Sitecore.Data.Items;

namespace events.tac.local.Controllers
{
    public class EventsListController : Controller
    {
        private const int PageSize = 5;
        // GET: EventsList
        public ActionResult Index(int page = 1)
        {
            // get the current context item
            var contextItem = RenderingContext.Current.ContextItem;
            //create a new model
            var model = new EventsList();
            //get the database name, should be lower anyway but just in case
            var databaseName = contextItem.Database.Name.ToLower();
            //get the index name
            var indexName = string.Format("events_{0}_index", databaseName);
            //get the index to use
            var index = ContentSearchManager.GetIndex(indexName);
            //check the index
            if(index != null)
                {
                //use the indexes search 
                using (var context = index.CreateSearchContext())
                {
                    // get the results
                    var results = context.GetQueryable<EventDetails>()
                                         .Where(item => item.Paths.Contains(contextItem.ID)
                                            && item.Language == contextItem.Language.Name)
                                            .Page(page - 1, PageSize)
                                            .GetResults();
                    // assing them to our model
                    model.Events = results.Hits.Select(hit => hit.Document).ToList();
                    model.TotalResultCount = results.TotalSearchResults;
                    model.PageSize = PageSize;
                }
            }
            //retunr the results in the model
            return View(model);
        }
    }
}