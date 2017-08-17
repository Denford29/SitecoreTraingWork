using Sitecore.Data.Items;
using Sitecore.Mvc.Presentation;
using System.Linq;
using System.Web.Mvc;
using SATC.SC.Framework.Navigation;

namespace events.tac.local.Controllers
{
    public class NavigationController : Controller
    {


        /// <summary>
        /// initiate the field for the navigation helpers from the SATC SC Framework
        /// </summary>
        private readonly NavigationHelpers _navigationHelpers;

        /// <summary>
        /// initiate the field to use for the rendering context
        /// </summary>
        private readonly RenderingContext _renderingContext;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="navigationHelpers"></param>
        /// <param name="renderingContext"></param>
        public NavigationController
            (
            NavigationHelpers navigationHelpers,
            RenderingContext renderingContext
            )
        {
            _navigationHelpers = navigationHelpers;
            _renderingContext = renderingContext;
        }

        /// <summary>
        /// get the initial display for the index with any defaults set
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            Item currentItem = _renderingContext.ContextItem;

            Item section = currentItem.Axes.GetAncestors().FirstOrDefault(item => item.TemplateName == "Events-Section");

            if (!string.IsNullOrWhiteSpace(RenderingContext.Current.Rendering.DataSource))
            {
                var datasourceItem = RenderingContext.Current.ContextItem.Database.GetItem(RenderingContext.Current.Rendering.DataSource);
                if (datasourceItem != null)
                {
                    section = datasourceItem;
                }
            }

            var model = _navigationHelpers.CreateNavigationMenu(section, currentItem);
            return View(model);
        }

    }
}