using System;
using events.tac.local.Models;
using Sitecore.ContentSearch;
using Sitecore.Mvc.Presentation;
using System.Linq;
using System.Web.Mvc;
using SATC.SC.Framework.SitecoreHelpers;
using Sitecore.ContentSearch.Linq;
using Sitecore.Data;
using Sitecore.Links;

namespace events.tac.local.Controllers
{
    public class EventsListController : Controller
    {
        /// <summary>
        /// set a constant to use for the page size
        /// </summary>
        private const int PageSize = 5;

        /// <summary>
        /// initiate a database to use
        /// </summary>
        private readonly Database _database;

        /// <summary>
        /// inittiate the field for the standard helpers from the SATC SC Framework
        /// </summary>
        private readonly StandardHelpers _standardHelpers;

        /// <summary>
        ///  Create the constructor and assing any fields
        /// </summary>
        /// <param name="standardHelpers"></param>
        public EventsListController
        (
            StandardHelpers standardHelpers
        )
        {
            _standardHelpers = standardHelpers;
            _database = RenderingContext.Current.ContextItem.Database;
        }

        /// <summary>
        /// Get the events using the events index
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        public ActionResult Index(int page = 1)
        {
            // get the current context item
            var contextItem = RenderingContext.Current.ContextItem;

            //create a new model
            var model = new EventsList();

            //get the index to use
            var index = GetEventsSearchIndex();
            //check the index
            if (index != null)
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

        /// <summary>
        /// get a list of upcoming events
        /// </summary>
        /// <returns></returns>
        public ActionResult GetUpcomingEvents()
        {
            //creae the initial model
            var model = new UpcomingEventsListModel();

            //get the current context
            var contextItem = RenderingContext.Current.ContextItem;

            //get the events landing page id from the config, to use to get the logo image from
            var eventsLandingPageId = _standardHelpers.GetItemIdFromConfig("EventsLandingPageID", contextItem);
            if (eventsLandingPageId != ID.Null)
            {
                var eventsLandingPage = _database.GetItem(eventsLandingPageId);
                if (eventsLandingPage != null)
                {
                    //get the events landing template id from the config
                    var eventsLandingTemplateId = _standardHelpers.GetTemplateIdFromConfig("EventsLandingTemplateID", contextItem);
                    //check if the events landing page uses that template
                    var isEventsLandingTemplate = eventsLandingPage.TemplateID == eventsLandingTemplateId;
                    if (isEventsLandingTemplate)
                    {
                        //set the url for the events landing page
                        model.EventsLandingUrl = LinkManager.GetItemUrl(eventsLandingPage);

                        //get the index to use
                        var index = GetEventsSearchIndex();
                        //check the index
                        if (index != null)
                        {
                            //use the indexes search 
                            using (var context = index.CreateSearchContext())
                            {
                                // get the results
                                var results = context.GetQueryable<EventDetails>()
                                    .Where(eventDetailItem => eventDetailItem.Paths.Contains(eventsLandingPage.ID)
                                                   && eventDetailItem.Date != DateTime.MinValue
                                                   && eventDetailItem.Date >= DateTime.Now)
                                    .OrderBy(eventDetailItem => eventDetailItem.Date)
                                    .Take(PageSize)
                                    .GetResults();
                                // assing them to our model
                                model.Events = results.Hits.Select(hit => hit.Document).ToList();
                            }
                        }
                    }
                }
            }
            return View(model);

        }

        public ISearchIndex GetEventsSearchIndex()
        {
            //get the database name, should be lower anyway but just in case
            var databaseName = _database.Name.ToLower();
            //get the index name
            var indexName = string.Format("events_{0}_index", databaseName);
            //get the index to use
            var eventsIndex = ContentSearchManager.GetIndex(indexName);
            //return the index
            return eventsIndex;
        }

    }
}