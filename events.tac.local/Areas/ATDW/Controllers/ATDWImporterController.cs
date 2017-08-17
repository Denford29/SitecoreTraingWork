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
using Sitecore.SecurityModel;

namespace events.tac.local.Areas.ATDW.Controllers
{
    public class AtdwImporterController : Controller
    {

        /// <summary>
        /// inittiate the field for the standard helper
        /// </summary>
        private readonly StandardHelpers _standardHelper;

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
        /// <param name="attributeType"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ViewResult> AttributesImporter(string attributeType, AttributesModel model)
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
            IEnumerable<AttributesJsonModel> responseAttributes;

            // create the api response
            var response =
                await httpClient.GetAsync(
                    "http://atlas.atdw-online.com.au/api/atlas/attributes?key=996325374125&types="+ selectedAttributeType + "&out=json");

            //check if we get a successs status back
            if (response.IsSuccessStatusCode)
            {
                //trya and read the contents and convert them into a json object
                try
                {
                    var responseStream = response.Content.ReadAsStringAsync();
                    //assign the json response to our attributes model
                    responseAttributes = JsonConvert
                        .DeserializeObject<IEnumerable<AttributesJsonModel>>(responseStream.Result).ToList();
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
            if (responseAttributes.Any())
            {
                //get the attribute templates to use late
                var atdwSettingsItemId = _standardHelper.GetItemIdFromConfig("ATDWSettingsItemID");
                var atdwAttributeTemplateId = _standardHelper.GetTemplateIdFromConfig("ATDWAttributeTemplateID");
                var atdwAttributeParentTemplateId =
                    _standardHelper.GetTemplateIdFromConfig("ATDWAttributeParentTemplateID");

                //check if the templates are valid
                if (atdwAttributeTemplateId.ID != ID.Null && atdwAttributeParentTemplateId.ID != ID.Null)
                {
                    var database = Sitecore.Configuration.Factory.GetDatabase("master");
                    //get the atdw settings item to look for the parent with
                    var atdwSettingsItem = database.GetItem(atdwSettingsItemId);
                    if (atdwSettingsItem != null)
                    {
                        var atdwAttributeParentItem = atdwSettingsItem.Children
                            .FirstOrDefault(item => item["AttributeTypeId"].ToString() == selectedAttributeType &&
                                                    item.TemplateID == atdwAttributeParentTemplateId);

                        //check if the parent to insert on is not null
                        if (atdwAttributeParentItem != null)
                        {
                            var attributeTemplateId = new TemplateID(atdwAttributeTemplateId);
                            // use the security disabler to creae or update items
                            using (new SecurityDisabler())
                            {
                                //go through the attributes to add
                                foreach (var attributeTypeItem in responseAttributes)
                                {
                                    try
                                    {
                                        //initiate the editing
                                        atdwAttributeParentItem.Editing.BeginEdit();
                                        // fill in the parents type and description
                                        atdwAttributeParentItem["AttributeTypeId"] = attributeTypeItem.AttributeTypeId;
                                        atdwAttributeParentItem["Description"] = attributeTypeItem.Description;
                                        //terminate the editing
                                        atdwAttributeParentItem.Editing.EndEdit();

                                        //get the attribute items from the parent
                                        if (attributeTypeItem.Attributes.Any())
                                        {
                                            foreach (var attribute in attributeTypeItem.Attributes)
                                            {
                                                //check if the parent doesnt alread have the attribute
                                                var attributeCode = attribute.AttributeId;
                                                var hasAttribute = atdwAttributeParentItem.Children
                                                    .Any(attributeItem => attributeItem.Fields["Attribute-Id"].Value ==
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
                                                    attributeItem["Attribute-Type-Id"] =
                                                        attributeTypeItem.AttributeTypeId;
                                                    attributeItem["Attribute-Id"] = attribute.AttributeId;
                                                    attributeItem["Attribute-Description"] = attribute.Description;
                                                    attributeItem["Attribute-ATDW-Id"] = attribute.AttributeId;
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
            }

            PrepareAttributesModel(model);
            model.SuccessMessage = attributeCounter + " attributes for the type: " + model.SelectedAttributeType +
                                   " have been imported successfully";
            return View(model);
        }

        public AttributesModel PrepareAttributesModel(AttributesModel model)
        {
            //get the database to use
            var database = Sitecore.Configuration.Factory.GetDatabase("master");

            //get the settings item id
            var atdwSettingsItemId = _standardHelper.GetItemIdFromConfig("ATDWSettingsItemID");
            var atdwAttributeTypeTemplateId = _standardHelper.GetTemplateIdFromConfig("ATDWAttributeParentTemplateID");
            if (atdwSettingsItemId != ID.Null)
            {
                var atdwSettingsItem = database.GetItem(atdwSettingsItemId);
                if (atdwSettingsItem != null)
                {
                    //get the attribute types list from the settings children
                    var attributeTypesList = atdwSettingsItem.Children
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

        [HttpGet]
        public ActionResult AttributesTypesImporter()
        {
            return View();
        }

        /// <summary>
        /// import the attributes types from a specific attribute type
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
            else
            {
                //get the response status code
                //var errorStatusCode = response.StatusCode;
                //var responseError = errorStatusCode.ToString();
            }

            //now get the attribute types from the model
            if (responseAttributeTypesList.Any())
            {
                //get the attribute type templates to use late
                var atdwSettingsItemId = _standardHelper.GetItemIdFromConfig("ATDWSettingsItemID");
                var atdwAttributeTypeTemplateId = _standardHelper.GetTemplateIdFromConfig("ATDWAttributeParentTemplateID");

                //check if the template are valid
                if (atdwAttributeTypeTemplateId.ID != ID.Null)
                {
                    var database = Sitecore.Configuration.Factory.GetDatabase("master");
                    //get the atdw settings item to use as the parent to insert the types
                    var atdwSettingsItem = database.GetItem(atdwSettingsItemId);
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
    }
}