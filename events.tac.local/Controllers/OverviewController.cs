using events.tac.local.Models;
using Sitecore.Links;
using Sitecore.Mvc.Presentation;
using Sitecore.Web.UI.WebControls;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace events.tac.local.Controllers
{
    public class OverviewController : Controller
    {
        // GET: Overview
        public ActionResult Index()
        {
            // create a new model
            var model = new OverviewList()
            {
                ReadMore = Sitecore.Globalization.Translate.Text("Read More")
            };

            // add the over view items
            model.AddRange(RenderingContext.Current.ContextItem.GetChildren(Sitecore.Collections.ChildListOptions.SkipSorting)
                .OrderByDescending(item => item.Created)
                .Select(item => new OverviewItem()
                {
                    Url = LinkManager.GetItemUrl(item),
                    Title = new HtmlString(FieldRenderer.Render(item, "ContentHeading")),
                    Image = new HtmlString(FieldRenderer.Render(item, "DecorationBanner", "w=450&h=300"))
                }
                ));

            return View(model);
        }
    }
}