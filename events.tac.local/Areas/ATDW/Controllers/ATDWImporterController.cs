using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Mvc;
using events.tac.local.Areas.ATDW.Models;
using Newtonsoft.Json;
using SATC.SC.Framework.SitecoreHelpers;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Data.Managers;
using Sitecore.Publishing;
using Sitecore.SecurityModel;

namespace events.tac.local.Areas.ATDW.Controllers
{
    public class AtdwImporterController : Controller
    {

        /// <summary>
        /// inittiate the field for the standard helper
        /// </summary>
        private readonly StandardHelpers _standardHelper;

        /// <inheritdoc />
        /// <summary>
        /// create the constructor and assing any fields
        /// </summary>
        /// <param name="standardHelper"></param>
        public AtdwImporterController
        (
            StandardHelpers standardHelper
        )
        {
            _standardHelper = standardHelper;
        }

        /// <summary>
        /// get the main view for the ATDW Importer
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View();
        }

        #region Product Attribute types and Attributes

        /// <summary>
        /// get the initial display for the attributes types importer
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult AttributesTypesImporter()
        {
            return View();
        }

        /// <summary>
        /// import the available attributes types from ATDW
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<ViewResult> ImportAttributesTypes()
        {
            //create the http client to request the json with
            var httpClient = new HttpClient();

            //create the initial response attribute types
            var responseAttributeTypesList = new List<AttributesTypesModel>();

            // create the api response
            var response =
                await httpClient.GetAsync("http://atlas.atdw-online.com.au/api/atlas/attributetypes?key=996325374125&out=json");

            //check if we get a successs status back
            if (response.IsSuccessStatusCode)
            {
                //try and read the contents and convert them into a json object
                try
                {
                    var responseStream = response.Content.ReadAsStringAsync();
                    //assign the json response to our attributes model
                    responseAttributeTypesList = JsonConvert.DeserializeObject<List<AttributesTypesModel>>(responseStream.Result);
                }
                catch (Exception)
                {
                    //catch the error
                    return View("AttributesTypesImporter");
                }
            }

            //now get the attribute types from the model
            if (responseAttributeTypesList.Any())
            {
                //get the attribute type templates to use late
                var atdwAttributesSettingsItemId = _standardHelper.GetItemIdFromConfig("ATDWAttributesSettingsItemID");
                var atdwAttributeTypeTemplateId = _standardHelper.GetTemplateIdFromConfig("ATDWAttributeParentTemplateID");

                //check if the template are valid
                if (atdwAttributeTypeTemplateId.ID != ID.Null)
                {
                    var database = Sitecore.Configuration.Factory.GetDatabase("master");
                    //get the atdw settings item to use as the parent to insert the types
                    var atdwSettingsItem = database.GetItem(atdwAttributesSettingsItemId);
                    if (atdwSettingsItem != null)
                    {
                        var attributeTypeTemplateId = new TemplateID(atdwAttributeTypeTemplateId);
                        // use the security disabler to create or update items
                        using (new SecurityDisabler())
                        {
                            //go through the attribute type to add
                            foreach (var attributeType in responseAttributeTypesList)
                            {
                                var attributeTypeCode = attributeType.AttributeTypeId;
                                var hasAttributeType = atdwSettingsItem.Children
                                    .Any(typeItem => typeItem.Fields["AttributeTypeId"].Value == attributeTypeCode);

                                if (!hasAttributeType)
                                {
                                    var name = ItemUtil.ProposeValidItemName(attributeType.Description);
                                    Item attributeTypeItem = atdwSettingsItem.Add(name, attributeTypeTemplateId);
                                    try
                                    {
                                        //initiate the editing
                                        attributeTypeItem.Editing.BeginEdit();
                                        // fill in the parents type and description
                                        attributeTypeItem["AttributeTypeId"] = attributeType.AttributeTypeId;
                                        attributeTypeItem["Description"] = attributeType.Description;
                                        // set our custom flag for items in use to false 
                                        attributeTypeItem["InUse"] = "0";
                                        //terminate the editing
                                        attributeTypeItem.Editing.EndEdit();

                                    }
                                    catch (Exception)
                                    {
                                        attributeTypeItem.Editing.CancelEdit();
                                        //catch the error and return it in the model
                                        return View("AttributesTypesImporter");
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return View("AttributesTypesImporter");
        }

        /// <summary>
        /// get the initial view for the attributes importer
        /// </summary>
        /// <returns></returns>
        public ActionResult AttributesImporter()
        {
            //create the initial model
            var model = new AttributesModel();

            //prefill the model with dropdown items
            PrepareAttributesModel(model);

            //return the view with the model
            return View(model);
        }

        /// <summary>
        /// imporrt the attributes from a specific attribute type
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ViewResult> AttributesImporter(AttributesModel model)
        {
            //create a the counter for the imported attributes
            var attributeCounter = 0;

            //check if we have a selected attribute type
            if (string.IsNullOrWhiteSpace(model.SelectedAttributeType))
            {
                //prefill the model with dropdown items
                PrepareAttributesModel(model);
                model.ErrorMessage = "Please select the attribute type to import";
                return View(model);
            }

            //get the selected attribute type
            var selectedAttributeType = model.SelectedAttributeType;

            //create the http client to request the json with
            var httpClient = new HttpClient();

            //create the initial response attributes
            AttributesJsonModel responseAttributes;

            // create the api response
            var response =
                await httpClient.GetAsync(
                    "http://atlas.atdw-online.com.au/api/atlas/attributes?key=996325374125&types=" + selectedAttributeType + "&out=json");

            //check if we get a successs status back
            if (response.IsSuccessStatusCode)
            {
                //try and read the contents and convert them into a json object
                try
                {
                    //read the contents of the response
                    var responseStream = response.Content.ReadAsStringAsync();
                    // get the result string
                    var responseResult = responseStream.Result;
                    //check the json result string, some tags will start with the sq brackets which will break the convertor
                    if (responseResult.StartsWith("[") || responseResult.EndsWith("]"))
                    {
                        responseResult = responseResult.Trim('[', ']');
                    }

                    //assign the json response to our attributes model
                    responseAttributes = JsonConvert.DeserializeObject<AttributesJsonModel>(responseResult);
                }
                catch (Exception ex)
                {
                    //catch the error
                    //prefill the model with dropdown items
                    PrepareAttributesModel(model);
                    model.ErrorMessage = "There was an error reading the data from ATDW, please try again, see :" + ex.Message;
                    return View(model);
                }
            }
            else
            {
                //get the response status code
                var errorStatusCode = response.StatusCode;
                var responseError = errorStatusCode.ToString();

                PrepareAttributesModel(model);
                model.ErrorMessage = "There was an error reading the data from ATDW, please try again, see :" + responseError;
                return View(model);
            }

            //now get the attributes from the model
            if (responseAttributes.Attributes.Any())
            {
                //get the attribute templates to use late
                var atdwAttributesSettingsItemId = _standardHelper.GetItemIdFromConfig("ATDWAttributesSettingsItemID");
                var atdwAttributeTemplateId = _standardHelper.GetTemplateIdFromConfig("ATDWAttributeTemplateID");
                var atdwAttributeParentTemplateId = _standardHelper.GetTemplateIdFromConfig("ATDWAttributeParentTemplateID");

                //check if the templates are valid
                if (atdwAttributeTemplateId.ID != ID.Null && atdwAttributeParentTemplateId.ID != ID.Null)
                {
                    var database = Sitecore.Configuration.Factory.GetDatabase("master");
                    //get the atdw settings item to look for the parent with
                    var atdwAttributesSettingsItem = database.GetItem(atdwAttributesSettingsItemId);
                    if (atdwAttributesSettingsItem != null && atdwAttributesSettingsItem.Children.Any())
                    {
                        var atdwAttributeParentItem = atdwAttributesSettingsItem.Children
                            .FirstOrDefault(item =>
                                                                item["AttributeTypeId"].ToString() == selectedAttributeType &&
                                                                item.TemplateID == atdwAttributeParentTemplateId);

                        //check if the parent to insert on is not null
                        if (atdwAttributeParentItem != null)
                        {
                            var attributeTemplateId = new TemplateID(atdwAttributeTemplateId);
                            // use the security disabler to creae or update items
                            using (new SecurityDisabler())
                            {
                                try
                                {
                                    //initiate the editing
                                    atdwAttributeParentItem.Editing.BeginEdit();
                                    // fill in the parents type and description
                                    atdwAttributeParentItem["AttributeTypeId"] = responseAttributes.AttributeTypeId;
                                    atdwAttributeParentItem["Description"] = responseAttributes.Description;
                                    //terminate the editing
                                    atdwAttributeParentItem.Editing.EndEdit();

                                    //get the attribute items from the parent
                                    if (responseAttributes.Attributes.Any())
                                    {
                                        foreach (var attribute in responseAttributes.Attributes)
                                        {
                                            //check if the parent doesnt alread have the attribute
                                            var attributeCode = attribute.AttributeId;
                                            var hasAttribute = atdwAttributeParentItem.Children
                                                .Any(attributeItem => attributeItem.Fields["AttributeId"].Value ==
                                                                      attributeCode);
                                            if (!hasAttribute)
                                            {
                                                //get a valid name from the description
                                                var name = ItemUtil.ProposeValidItemName(attribute.Description);
                                                //create the new attribute to add
                                                Item attributeItem =
                                                    atdwAttributeParentItem.Add(name, attributeTemplateId);

                                                //initiate the editing
                                                attributeItem.Editing.BeginEdit();
                                                // fill in the attribute properties
                                                attributeItem["AttributeTypeId"] = responseAttributes.AttributeTypeId;
                                                attributeItem["AttributeId"] = attribute.AttributeId;
                                                attributeItem["AttributeDescription"] = attribute.Description;
                                                attributeItem["AttributeATDWId"] = attribute.AttributeId;
                                                // set our custom flag for items in use to false 
                                                attributeItem["InUse"] = "0";
                                                //initiate the editing
                                                attributeItem.Editing.EndEdit();
                                                attributeCounter++;
                                            }
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    atdwAttributeParentItem.Editing.CancelEdit();
                                    //prefill the model with dropdown items
                                    PrepareAttributesModel(model);
                                    model.ErrorMessage = "There was an error reading the data from ATDW, please try again, see :" + ex.Message;
                                    return View(model);
                                }
                            }
                        }
                    }
                }
            }

            PrepareAttributesModel(model);
            model.SuccessMessage = attributeCounter + " attributes for the type: " + model.SelectedAttributeType +
                                   " have been imported successfully";
            return View(model);
        }

        /// <summary>
        /// Prepare the default required properties for an attribute model
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public AttributesModel PrepareAttributesModel(AttributesModel model)
        {
            //get the database to use
            var database = Sitecore.Configuration.Factory.GetDatabase("master");

            //get the settings item id
            var atdwAttributesSettingsItemId = _standardHelper.GetItemIdFromConfig("ATDWAttributesSettingsItemID");
            var atdwAttributeTypeTemplateId = _standardHelper.GetTemplateIdFromConfig("ATDWAttributeParentTemplateID");
            if (atdwAttributesSettingsItemId != ID.Null)
            {
                var atdwAttributeSettingsItem = database.GetItem(atdwAttributesSettingsItemId);
                if (atdwAttributeSettingsItem != null)
                {
                    //get the attribute types list from the settings children
                    var attributeTypesList = atdwAttributeSettingsItem.Children
                        .Where(item => item.TemplateID == atdwAttributeTypeTemplateId).ToList();
                    //check if we have any types
                    if (attributeTypesList.Any())
                    {
                        // add these to our select list
                        foreach (var attributeType in attributeTypesList)
                        {
                            model.AttributeTypesAvailable.Add(
                                new SelectListItem
                                {
                                    Text = attributeType.Name,
                                    Value = attributeType.Fields["AttributeTypeId"].ToString()
                                }
                            );
                        }
                    }
                }
            }

            return model;
        }

        #endregion

        #region Product Categories and Category Classifications

        /// <summary>
        /// get the default view of the categories importer
        /// </summary>
        /// <returns></returns>
        public ActionResult CategoriesImporter()
        {
            return View();
        }

        /// <summary>
        /// mport the available categories types from ATDW
        /// </summary>
        /// <returns></returns>
        public async Task<ViewResult> ImportCategories()
        {
            //create the http client to request the json with
            var httpClient = new HttpClient();

            //create the initial response categories list
            var responseCategoiresList = new List<ProductCategoriesModel>();

            // create the api response
            var response =
                await httpClient.GetAsync("http://atlas.atdw-online.com.au/api/atlas/categories?key=996325374125&out=json");

            //check if we get a successs status back
            if (response.IsSuccessStatusCode)
            {
                //try and read the contents and convert them into a json object
                try
                {
                    var responseStream = response.Content.ReadAsStringAsync();
                    //assign the json response to our attributes model
                    responseCategoiresList = JsonConvert.DeserializeObject<List<ProductCategoriesModel>>(responseStream.Result);
                }
                catch (Exception)
                {
                    //catch the error
                    return View("CategoriesImporter");
                }
            }
            else
            {
                //get the response status code
                //var errorStatusCode = response.StatusCode;
                //var responseError = errorStatusCode.ToString();
            }

            //now get the categories list from the model
            if (responseCategoiresList.Any())
            {
                //get the categories item templates to use late
                var projectSettingsItemId = _standardHelper.GetItemIdFromConfig("ProjectSettingsItemID");
                var atdwCategoriesParentTemplateId = _standardHelper.GetTemplateIdFromConfig("ATDWCategoriesParentTemplateID");
                var atdwCategoryTemplateId = _standardHelper.GetTemplateIdFromConfig("ATDWCategoryTemplateID");

                //check if the template are valid
                if (atdwCategoryTemplateId.ID != ID.Null)
                {
                    var database = Sitecore.Configuration.Factory.GetDatabase("master");
                    //get the project settings item to use  to find the categories parent to insert the types
                    var projectSettingsItem = database.GetItem(projectSettingsItemId);
                    if (projectSettingsItem != null)
                    {
                        //get the categories arent to use to import categories under
                        var categoriesParentContainer = projectSettingsItem.GetChildren()
                                                                          .FirstOrDefault(settingChild => settingChild.TemplateID == atdwCategoriesParentTemplateId);

                        //check if we have the categories parent container
                        if (categoriesParentContainer != null)
                        {
                            var categoryTemplateId = new TemplateID(atdwCategoryTemplateId);
                            // use the security disabler to create or update items
                            using (new SecurityDisabler())
                            {
                                //go through the categories to add
                                foreach (var productCategory in responseCategoiresList)
                                {
                                    var categoryCode = productCategory.CategoryId;
                                    var hasCategory = categoriesParentContainer.Children
                                                                .Any(typeItem => typeItem.Fields["CategoryId"].Value == categoryCode);

                                    if (!hasCategory)
                                    {
                                        var name = ItemUtil.ProposeValidItemName(productCategory.Description);
                                        Item categoryItem = categoriesParentContainer.Add(name, categoryTemplateId);
                                        try
                                        {
                                            //initiate the editing
                                            categoryItem.Editing.BeginEdit();
                                            // fill in the category properties
                                            categoryItem["CategoryId"] = productCategory.CategoryId;
                                            categoryItem["CategoryDescription"] = productCategory.Description;
                                            categoryItem["CategoryATDWId"] = productCategory.CategoryId;

                                            // set our custom flag for items in use to false
                                            categoryItem["InUse"] = "0";

                                            //terminate the editing
                                            categoryItem.Editing.EndEdit();

                                        }
                                        catch (Exception)
                                        {
                                            categoryItem.Editing.CancelEdit();
                                            //catch the error and return it in the model
                                            return View("CategoriesImporter");
                                        }
                                    }
                                }
                            }
                        }

                    }
                }
            }

            return View("CategoriesImporter");
        }

        /// <summary>
        /// get the initial view of category classifications view
        /// </summary>
        /// <returns></returns>
        public ActionResult CategoryClassificationsImporter()
        {
            // create the initial model
            var model = new CategoryClassificationsModel();

            // prepare the model
            PrepareCategoryClassificationsModel(model);

            //return the model to the view
            return View(model);
        }

        /// <summary>
        /// import the classifications using a post async method
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ViewResult> CategoryClassificationsImporter(CategoryClassificationsModel model)
        {
            //create a the counter for the imported classifications
            var classificationCounter = 0;

            //check if we have a selected category
            if (string.IsNullOrWhiteSpace(model.SelectedCategory))
            {
                //prefill the model with dropdown items
                PrepareCategoryClassificationsModel(model);
                model.ErrorMessage = "Please select the category to import classifications for";
                return View(model);
            }

            //get the selected category
            var selectedCategory = model.SelectedCategory;

            //create the http client to request the json with
            var httpClient = new HttpClient();

            //create the initial response list of classifications
            List<CategoryClassificationsJsonModel> responseClassifications;

            // create the api response
            var response =
                await httpClient.GetAsync(
                    "http://atlas.atdw-online.com.au/api/atlas/producttypes?key=996325374125&cats=" + selectedCategory + "&out=json");

            //check if we get a successs status back
            if (response.IsSuccessStatusCode)
            {
                //try and read the contents and convert them into a json object
                try
                {
                    //read the contents of the response
                    var responseStream = response.Content.ReadAsStringAsync();
                    // get the result string
                    var responseResult = responseStream.Result;

                    //assign the json response to our classifications models list
                    responseClassifications = JsonConvert.DeserializeObject<List<CategoryClassificationsJsonModel>>(responseResult);
                }
                catch (Exception ex)
                {
                    //catch the error and prefill the model with dropdown items
                    PrepareCategoryClassificationsModel(model);
                    model.ErrorMessage = "There was an error reading the data from ATDW, please try again, see :" + ex.Message;
                    return View(model);
                }
            }
            else
            {
                //get the response status code
                var errorStatusCode = response.StatusCode;
                var responseError = errorStatusCode.ToString();

                PrepareCategoryClassificationsModel(model);
                model.ErrorMessage = "There was an error reading the data from ATDW, please try again, see :" + responseError;
                return View(model);
            }

            //now get check if our list contains any classifications
            if (responseClassifications.Any())
            {
                //get the attribute templates to use late
                var atdwCategorySettingsItemId = _standardHelper.GetItemIdFromConfig("ATDWCategorySettingsItemID");
                var atdwClassificationTemplateId = _standardHelper.GetTemplateIdFromConfig("ATDWClassificationTemplateID");
                var atdwClassificationParentTemplateId = _standardHelper.GetTemplateIdFromConfig("ATDWCategoryTemplateID");

                //check if the templates are valid
                if (atdwClassificationTemplateId.ID != ID.Null && atdwClassificationParentTemplateId.ID != ID.Null)
                {
                    var database = Sitecore.Configuration.Factory.GetDatabase("master");
                    //get the atdw category settings item to look for the parent with
                    var atdwCategorySettingsItem = database.GetItem(atdwCategorySettingsItemId);
                    if (atdwCategorySettingsItem != null && atdwCategorySettingsItem.Children.Any())
                    {
                        var atdwClassificationParentItem = atdwCategorySettingsItem.Children
                            .FirstOrDefault(item => item["CategoryId"].ToString() == selectedCategory &&
                                                                item.TemplateID == atdwClassificationParentTemplateId);

                        //check if the parent to insert on is not null
                        if (atdwClassificationParentItem != null)
                        {
                            var classificationTemplateId = new TemplateID(atdwClassificationTemplateId);
                            // use the security disabler to creae or update items
                            using (new SecurityDisabler())
                            {
                                try
                                {
                                    //get the attribute items from the parent
                                    if (responseClassifications.Any())
                                    {
                                        foreach (var classification in responseClassifications)
                                        {
                                            //check if the parent doesnt alread have the attribute
                                            var classificationId = classification.ProductTypeId;
                                            var hasclassification = atdwClassificationParentItem.Children
                                                .Any(classificationItem => classificationItem.Fields["ProductTypeId"].Value == classificationId);
                                            if (!hasclassification)
                                            {
                                                //get a valid name from the description
                                                var name = ItemUtil.ProposeValidItemName(classification.Description);
                                                //create the new classification to add
                                                Item classificationItem = atdwClassificationParentItem.Add(name, classificationTemplateId);

                                                //initiate the editing
                                                classificationItem.Editing.BeginEdit();
                                                // fill in the attribute properties
                                                classificationItem["ProductTypeId"] = classification.ProductTypeId;
                                                classificationItem["Description"] = classification.Description;
                                                classificationItem["ProductCategory"] = classification.ProductCategory;

                                                // set our custom flag for items in use to false
                                                classificationItem["InUse"] = "0";

                                                //initiate the editing
                                                classificationItem.Editing.EndEdit();
                                                classificationCounter++;
                                            }
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    atdwClassificationParentItem.Editing.CancelEdit();
                                    //prefill the model with dropdown items
                                    PrepareCategoryClassificationsModel(model);
                                    model.ErrorMessage = "There was an error reading the data from ATDW, please try again, see :" + ex.Message;
                                    return View(model);
                                }

                                //}
                            }
                        }
                    }
                }
            }

            // return a success message with a prefilled model
            PrepareCategoryClassificationsModel(model);
            model.SuccessMessage = classificationCounter + " attributes for the type: " + model.SelectedCategory +
                                   " have been imported successfully";
            return View(model);
        }

        /// <summary>
        /// prepare the category classifications model
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public CategoryClassificationsModel PrepareCategoryClassificationsModel(CategoryClassificationsModel model)
        {
            //get the database to use
            var database = Sitecore.Configuration.Factory.GetDatabase("master");

            //get the category settings item id, and category template id
            var atdwCategorySettingsItemId = _standardHelper.GetItemIdFromConfig("ATDWCategorySettingsItemID");
            var atdwCategoryTemplateId = _standardHelper.GetTemplateIdFromConfig("ATDWCategoryTemplateID");
            //check if the id is not null
            if (atdwCategorySettingsItemId != ID.Null)
            {
                //get the category settings item
                var atdwCategorySettingsItem = database.GetItem(atdwCategorySettingsItemId);
                if (atdwCategorySettingsItem != null)
                {
                    //get the categories list from the category settings children
                    var categoriesList = atdwCategorySettingsItem.Children
                                                        .Where(item => item.TemplateID == atdwCategoryTemplateId).ToList();
                    //check if we have any categories
                    if (categoriesList.Any())
                    {
                        // add these to our select list
                        foreach (var category in categoriesList)
                        {
                            model.CategoriesAvailable.Add(
                                new SelectListItem
                                {
                                    Text = category.Name,
                                    Value = category.Fields["CategoryId"].ToString()
                                }
                            );
                        }
                    }
                }
            }

            //return the prepared model
            return model;
        }

        #endregion

        #region Products Auto Publishing

        /// <summary>
        /// get the initial view of the auto publish page, this will automatically start the publish logic on the initial view
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult AutoPublishProducts(AutoPublishModel model)
        {
            if (string.IsNullOrWhiteSpace(model.DomainUrl))
            {
                model = new AutoPublishModel()
                {
                    InitiatePublish = true
                };
            }

            //return the view with the model
            return View(model);
        }

        /// <summary>
        /// run the auto publish call and return the view with a success or failure message after
        /// </summary>
        /// <param name="model"></param>
        /// <param name="publishForm"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AutoPublishProducts(AutoPublishModel model, string publishForm)
        {
            if (!string.IsNullOrWhiteSpace(publishForm))
            {
                try
                {
                    // run the publish on page load
                    PublishOptions publishOptions = new PublishOptions(
                                                                                    Database.GetDatabase("master"),
                                                                                    Database.GetDatabase("web"),
                                                                                    PublishMode.Smart,
                                                                                    LanguageManager.DefaultLanguage,
                                                                                    DateTime.Now)
                    {
                        Deep = true,
                        RootItem = Sitecore.Context.Database.GetItem("/sitecore/media library/ATDW-Products")
                    };

                    // Create a publisher with the publish options and run the initial publish of images
                    Publisher medialPublisher = new Publisher(publishOptions);
                    medialPublisher.Publish();

                    //trigger the second publish for the content products
                    publishOptions.RootItem = Sitecore.Context.Database.GetItem("/sitecore/content/Project-Settings/ATDW-Products");
                    Publisher contentPublisher = new Publisher(publishOptions);
                    contentPublisher.Publish();

                    // set a success message
                    model.SuccessMessage = "Auto Publish complete";
                }
                catch (Exception ex)
                {
                    model.ErrorMessage = ex.Message;
                }

            }
            // set the flag to trigger publish to false
            model.InitiatePublish = false;

            //return the model
            return View(model);
        }

        #endregion
    }
}