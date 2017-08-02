using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Sitecore.Web.UI.WebControls;
using events.tac.local.Models;
using Sitecore.Mvc.Presentation;
using Sitecore.Links;

namespace events.tac.local.Controllers
{
    public class FeaturedEventController : Controller
    {
        // GET: FeaturedEvent
        public ActionResult Index()
        {
            //return the view with the featured event
            return View(CreateModel());
        }

        public static FeaturedEvent CreateModel()
        {
            // get the rendering item, which should be the one defined by the datasource if set, otherwise falls back to the current item
            var item = RenderingContext.Current.Rendering.Item;

            //create and populate the featured event from the item
            var featuredEvent = new FeaturedEvent()
            {
                EventHeading = new HtmlString(FieldRenderer.Render(item, "ContentHeading")),
                EventImage = new HtmlString(FieldRenderer.Render(item, "Event-Image", "mw=400")),
                EventIntro = new HtmlString(FieldRenderer.Render(item, "ContentIntro"))
            };

            //get the link for the item
            featuredEvent.UrlLink = LinkManager.GetItemUrl(item);

            //get the css class from the dropdown list selected if any
            var cssClass = RenderingContext.Current.Rendering.Parameters["CssClass"];
            if (!string.IsNullOrWhiteSpace(cssClass))
            {
                var refItem = Sitecore.Context.Database.GetItem(cssClass);
                if(refItem != null)
                {
                    featuredEvent.CssClass = refItem["class"];
                }
                else
                {
                    featuredEvent.CssClass = cssClass;
                }
                
            }

            //return the featured event
            return featuredEvent;
        }
    }
}