using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using events.tac.local.Models;
using SATC.SC.Framework.Navigation.Models;
using SATC.SC.Framework.SitecoreHelpers;
using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Links;
using Sitecore.Mvc.Presentation;
using Sitecore.Shell.Framework.Commands;
using Sitecore.Web.UI.WebControls;

namespace events.tac.local.Controllers
{
    public class PageFooterController : Controller
    {

        /// <summary>
        /// inittiate the field for the standard helpers from the SATC SC Framework
        /// </summary>
        private readonly StandardHelpers _standardHelpers;

        /// <summary>
        ///  create the constructor and assing any fields
        /// </summary>
        /// <param name="standardHelpers"></param>
        public PageFooterController
        (
            StandardHelpers standardHelpers
        )
        {
            _standardHelpers = standardHelpers;
        }

        /// <summary>
        /// get the page footer with the model populated
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            //get the current context
            var contextItem = RenderingContext.Current.ContextItem;

            //get the master database to use to get items from
            var database = contextItem.Database;

            //create the intial model
            var model = new PageFooterModel();

            //get the project settings id from the config, to use to get the logo image from
            var projectSettingsPageId = _standardHelpers.GetItemIdFromConfig("ProjectSettingsPageID", contextItem);
            if (projectSettingsPageId != ID.Null)
            {
                var projectSettingsPage = database.GetItem(projectSettingsPageId);
                if (projectSettingsPage != null)
                {
                    //get the project settings template id from the config
                    var projectSettingsTemplateId = _standardHelpers.GetTemplateIdFromConfig("ProjectSettingsTemplateID", contextItem);
                    //check if the project settings page uses that template
                    var isProjectSettingsTemplate = projectSettingsPage.TemplateID == projectSettingsTemplateId;
                    if (isProjectSettingsTemplate)
                    {
                        //save the project settings path to use for the editor
                        model.EditorDatasourcePath = projectSettingsPage.Paths.FullPath;
                        //get the copyright text
                        model.CopyrightText = new HtmlString(FieldRenderer.Render(projectSettingsPage, "Copyright_Text"));

                        //get the page set for the details
                        var detailsPageId = projectSettingsPage.Fields["Info_Page"];
                        if (detailsPageId != null)
                        {
                            //get the details page
                            var detailsPage = database.GetItem(detailsPageId.ToString());
                            if (detailsPage != null)
                            {
                                model.FooterHeading = new HtmlString(FieldRenderer.Render(detailsPage, "ContentHeading"));
                                model.FooterIntro = new HtmlString(FieldRenderer.Render(detailsPage, "ContentIntro"));
                            }
                        }

                        //get the footer links
                        MultilistField footerLinks = projectSettingsPage.Fields["Page-Links"];
                        if (footerLinks != null)
                        {
                            var footerLinksPages = footerLinks.GetItems();
                            if (footerLinksPages.Any())
                            {
                                model.FooterLinks = footerLinksPages.Select(linkPage => new NavigationItemModel()
                                {
                                            Title = linkPage.DisplayName,
                                            Url = LinkManager.GetItemUrl(linkPage)
                                })
                                .ToList();
                            }
                        }
                    }
                }
            }

            // return the view with the model
            return View(model);
        }
    }
}