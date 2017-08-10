using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using events.tac.local.Models;
using SATC.SC.Framework.SitecoreHelpers;
using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Links;
using Sitecore.Mvc.Presentation;
using Sitecore.Web.UI.WebControls;

namespace events.tac.local.Controllers
{
    public class CarouselController : Controller
    {

        /// <summary>
        /// inittiate the field for the standard helpers from the SATC SC Framework
        /// </summary>
        private readonly StandardHelpers _standardHelpers;

        /// <summary>
        /// create the constructor and assing any fields
        /// </summary>
        /// <param name="standardHelpers"></param>
        public CarouselController
        (
            StandardHelpers standardHelpers
        )
        {
            _standardHelpers = standardHelpers;
        }

        // GET: Carousel
        public ActionResult Index()
        {
            //get the master database to use to get items from
            //var database = RenderingContext.Current.ContextItem.Database;
            var database = Sitecore.Context.Database;

            //create the  default model
            var model = new SiteCarouselModel();

            //get the site settings id from the config, to use to get the carousel items
            var siteSettingsPageId = _standardHelpers.GetItemIdFromConfig("SiteSettingsPageID");
            if (siteSettingsPageId != ID.Null)
            {
                var siteSettingsPage = database.GetItem(siteSettingsPageId);
                if (siteSettingsPage != null)
                {
                    //get the site settings template id from the config
                    var siteSettingsTemplateId = _standardHelpers.GetTemplateIdFromConfig("SiteSettingsTemplateID");
                    //check if the site settings page uses that template
                    var isSiteSettingsTemplate = siteSettingsPage.TemplateID == siteSettingsTemplateId;
                    if (isSiteSettingsTemplate)
                    {
                        //model.LogoImage = new HtmlString(FieldRenderer.Render(siteSettingsPage, "Project_Logo", "mw=400"));

                        //get the tree list of carousel images
                        MultilistField carouselImagesList = siteSettingsPage.Fields["Carousel"];

                        if (carouselImagesList != null)
                        {
                            //get the carousel images from the tree list
                            var carouselImages = carouselImagesList.GetItems();
                            //create the carousel models if we have any images
                            if (carouselImages.Any())
                            {
                                foreach (var image in carouselImages)
                                {
                                    MediaItem carouselMedia = image;
                                    var carouselImageModel = new CarouselImageModel()
                                    {
                                        CarouselImage = Sitecore.Resources.Media.MediaManager.GetMediaUrl(carouselMedia),
                                        CarouselHeading = image.Fields["Title"].ToString(),
                                        CarouselIntro = image.Fields["Description"].ToString(),
                                        CarouselLink = image.Fields["ContentLink"].ToString()
                                    };

                                    //get the content link
                                    var contentLinkField = image.Fields["ContentLink"].ToString();
                                    if (!string.IsNullOrWhiteSpace(contentLinkField))
                                    {
                                        //var contentLinkId = new ID(contentLinkField);
                                        var contentLink = database.GetItem(contentLinkField);
                                        if (contentLink != null)
                                        {
                                            carouselImageModel.CarouselLink = LinkManager.GetItemUrl(contentLink);
                                        }
                                    }

                                    //add it to the model items
                                    model.CarouselItems.Add(carouselImageModel);
                                }
                            }
                        }
                    }
                }
            }
            return View(model);
        }
    }
}