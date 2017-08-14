using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using events.tac.local.Models;
using SATC.SC.Framework.SitecoreHelpers;
using Sitecore.Data;
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
            //get the master database to use to get items from
            var database = RenderingContext.Current.ContextItem.Database;

            //create the intial model
            var model = new PageFooterModel();

            //get the project settings id from the config, to use to get the logo image from
            var projectSettingsPageId = _standardHelpers.GetItemIdFromConfig("ProjectSettingsPageID");
            if (projectSettingsPageId != ID.Null)
            {
                var projectSettingsPage = database.GetItem(projectSettingsPageId);
                if (projectSettingsPage != null)
                {
                    //get the project settings template id from the config
                    var projectSettingsTemplateId = _standardHelpers.GetTemplateIdFromConfig("ProjectSettingsTemplateID");
                    //check if the project settings page uses that template
                    var isProjectSettingsTemplate = projectSettingsPage.TemplateID == projectSettingsTemplateId;
                    if (isProjectSettingsTemplate)
                    {
                        //get the copyright text
                        model.CopyrightText = new HtmlString(FieldRenderer.Render(projectSettingsPage, "Copyright_Text"));

                        //get the page set for the details
                        var detailsPageId = projectSettingsPage.Fields["Info_Page"];
                        if (detailsPageId != null)
                        {
                           // var 
                        }
                    }
                }
            }

            return View(model);
        }
    }
}