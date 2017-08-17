using System.Web;
using events.tac.local.Models;
using SATC.SC.Framework.SitecoreHelpers;
using Sitecore.Data;
using System.Web.Mvc;
using SATC.SC.Framework.Navigation;
using Sitecore.Data.Items;
using Sitecore.Links;
using Sitecore.Mvc.Presentation;
using Sitecore.Web.UI.WebControls;

namespace events.tac.local.Controllers
{
    public class PageHeaderController : Controller
    {
        /// <summary>
        /// inittiate the field for the standard helpers from the SATC SC Framework
        /// </summary>
        private readonly StandardHelpers _standardHelpers;

        /// <summary>
        /// initiate the field for the navigation helpers from the SATC SC Framework
        /// </summary>
        private readonly NavigationHelpers _navigationHelpers;

        /// <summary>
        /// inittiate the field for the language helpers from the SATC SC Framework
        /// </summary>
        private readonly LanguageHelpers _languageHelpers;

        /// <summary>
        /// create the constructor and assing any fields
        /// </summary>
        /// <param name="standardHelpers"></param>
        /// <param name="navigationHelpers"></param>
        /// <param name="languageHelpers"></param>
        public PageHeaderController
        (
            StandardHelpers standardHelpers,
            NavigationHelpers navigationHelpers ,
            LanguageHelpers languageHelpers
        )
        {
            _standardHelpers = standardHelpers;
            _navigationHelpers = navigationHelpers;
            _languageHelpers = languageHelpers;
        }

        /// <summary>
        /// Get the default menu items and other settings
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            //get the current context
            var contextItem = RenderingContext.Current.ContextItem;

            //get the master database to use to get items from
            var database = contextItem.Database;

            //create a new model
            var model = new PageHeaderModel();

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
                        model.LogoImage = new HtmlString(FieldRenderer.Render(projectSettingsPage, "Project_Logo", "mw=400"));
                    }
                }
            }

            // get the homepage id set in the config to use to get the menu items
            var homePageId = _standardHelpers.GetItemIdFromConfig("HomePageID", contextItem);
            if (homePageId != ID.Null)
            {
                //get the home item from the config id
                var homePage = database.GetItem(homePageId);
                //check if the home item in not null
                if (homePage != null)
                {
                    //check if the homepage's template is the same as the one set in our config
                    var homeTemplateId = _standardHelpers.GetTemplateIdFromConfig("HomeTemplateID", contextItem);
                    var isHomePageTemplate = homePage.TemplateID == homeTemplateId;
                    //if the homepage template matches then we can now use it to get the navigation items
                    if (isHomePageTemplate)
                    {
                        //add the verified home page to the model
                        model.HomePageUrl = LinkManager.GetItemUrl(homePage);

                        //get the home page item and add it to the menu items from the verified home page   
                        model.MenuItems.Add(_navigationHelpers.CreateSiteMenuModel(homePage, false));

                        //get the home page children and add them to the menu items
                        if (homePage.HasChildren)
                        {
                            foreach (Item childPage in homePage.Children)
                            {
                                //get the child page items from the verified home page   
                                model.MenuItems.Add(_navigationHelpers.CreateSiteMenuModel(childPage, true));
                            }
                        }
                    }
                }
            }

            //get the site's languages for the dropdown language selector
            model.CurrentLanguage = _languageHelpers.GetActiveLanguageModel();
            model.Languages = _languageHelpers.GetAllLanguages();

            return View(model);
        }
    }
}