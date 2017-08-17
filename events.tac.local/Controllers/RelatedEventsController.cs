using events.tac.local.Models;
using Sitecore.Data.Fields;
using Sitecore.Links;
using Sitecore.Mvc.Presentation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SATC.SC.Framework.Navigation.Models;

namespace events.tac.local.Controllers
{
    public class RelatedEventsController : Controller
    {
        // GET: RelatedEvents
        public ActionResult Index()
        {
            //get the current item using rendering item, this can also bring back the data source item and if not set defaults back to the current item
            var currentItem = RenderingContext.Current.Rendering.Item;
            if (currentItem == null)
            {
                return new EmptyResult();
            }

            //get the related events list
            MultilistField relatedEvents = currentItem.Fields["Related-Events"];
            if (relatedEvents == null)
            {
                return new EmptyResult();
            }

            //get the set items
            var setEvents = relatedEvents.GetItems();
            if (!setEvents.Any() && !Sitecore.Context.PageMode.IsExperienceEditorEditing)
            {
                return new EmptyResult();
            }

            var displayEvents = setEvents.Select(eventItem => new NavigationItemModel()
            {
                Title = eventItem.DisplayName,
                Url = LinkManager.GetItemUrl(eventItem)
            });

            // return the view with the events
            return View(displayEvents);
        }
    }
}