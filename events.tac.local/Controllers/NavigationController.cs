using events.tac.local.Business.Navigation;
using events.tac.local.Models;
using Sitecore.Data.Items;
using Sitecore.Links;
using Sitecore.Mvc.Presentation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TAC.Utils.SitecoreModels;

namespace events.tac.local.Controllers
{
    public class NavigationController : Controller
    {

        // Fields
        private readonly NavigationModelBuilder _navigationModelBuilder;
        private readonly RenderingContext _renderingContext;

        //Constructor
        public NavigationController
            (
            NavigationModelBuilder navigationModelBuilder,
            RenderingContext renderingContext
            )
        {
            _navigationModelBuilder = navigationModelBuilder;
            _renderingContext = renderingContext;
        }

        // GET: Navigation
        public ActionResult Index()
        {
            //Item currentItem = RenderingContext.Current.ContextItem;
            Item currentItem = _renderingContext.ContextItem;

            Item Section = currentItem.Axes.GetAncestors().FirstOrDefault(item => item.TemplateName == "Events-Section");

            if (!string.IsNullOrWhiteSpace(RenderingContext.Current.Rendering.DataSource))
            {
                var datasourceItem = RenderingContext.Current.ContextItem.Database.GetItem(RenderingContext.Current.Rendering.DataSource);
                if(datasourceItem != null)
                {
                    Section = datasourceItem;
                }
            }

            //var model = _navigationModelBuilder.CreateNavigationMenu(Section, currentItem);
            var model = _navigationModelBuilder.CreateNavigationMenu(new SitecoreItem(Section), new SitecoreItem(currentItem));
            return View(model);
        }

    }
}