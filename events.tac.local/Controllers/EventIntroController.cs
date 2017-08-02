using events.tac.local.Models;
using Sitecore.Mvc.Presentation;
using Sitecore.Web.UI.WebControls;
using System.Web;
using System.Web.Mvc;

namespace events.tac.local.Controllers
{
    public class EventIntroController : Controller
    {
        // GET: EventIntro
        public ActionResult Index()
        {
            //return the view with the event intor model 
            return View(CreateModel());
        }

        /// <summary>
        /// prepare and return the event intro
        /// </summary>
        /// <returns></returns>
        private static EventIntro CreateModel()
        {
            //get the current context item
            var item = RenderingContext.Current.ContextItem;

            //create and populate the event intro model
            var eventIntro = new EventIntro()
            {
                Heading = new HtmlString(FieldRenderer.Render(item, "ContentHeading")),
                EventImage = new HtmlString(FieldRenderer.Render(item, "Event-Image", "mw=400")),
                Hightlights = new HtmlString(FieldRenderer.Render(item, "Highlights")),
                Intro = new HtmlString(FieldRenderer.Render(item, "ContentIntro")),
                Body = new HtmlString(FieldRenderer.Render(item, "Content-Body")),
                StartDate = new HtmlString(FieldRenderer.Render(item, "Date", "format=dd MMMM yyyy")),
                Duration = new HtmlString(FieldRenderer.Render(item, "Duration")),
                Difficult = new HtmlString(FieldRenderer.Render(item, "Difficulty-Level"))
            };

            //return the event intro
            return eventIntro;
        }
    }
}