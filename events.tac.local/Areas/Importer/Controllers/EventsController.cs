using events.tac.local.Areas.Importer.Models;
using Newtonsoft.Json;
using SATC.SC.Framework.SitecoreHelpers;
using Sitecore;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.SecurityModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace events.tac.local.Areas.Importer.Controllers
{
    public class EventsController : Controller
    {

        /// <summary>
        /// inittiate the field for the standard helper
        /// </summary>
        private readonly StandardHelpers _standardHelper;

        /// <summary>
        /// create the constructor and assing any fields
        /// </summary>
        /// <param name="standardHelper"></param>
        public EventsController
        (
            StandardHelpers standardHelper
        )
        {
            _standardHelper = standardHelper;
        }

        // GET: Importer/Events
        public ActionResult Index()
        {
            var model = new EventsImporterModel();
            return View(model);
        }

        // recieve the posted form
        [HttpPost]
        public ActionResult Index(HttpPostedFileBase file, string parentPath, EventsImporterModel model)
        {
            IEnumerable<Event> events;

            string message = null;

            using (var reader = new System.IO.StreamReader(file.InputStream))
            {
                var contents = reader.ReadToEnd();
                try
                {
                    events = JsonConvert.DeserializeObject<IEnumerable<Event>>(contents);
                }
                catch (Exception ex)
                {
                    //catch the error and return it in the model
                    model.ErrorMessage = ex.Message;
                    return View(model);
                }
            }

            //get the id's to use from the config file by passing in the name of the setting, since we need all of them if one fails we wont import

            var eventDetailsTemplateId = _standardHelper.GetItemIdFromConfig("EventsDetailsTemplateID");
            var eventListingTemplateId = _standardHelper.GetItemIdFromConfig("EventsListingTemplateID");
            var eventWorkflowId = _standardHelper.GetItemIdFromConfig("EIWorkflowID");
            var eventWorkflowCreatedId = _standardHelper.GetItemIdFromConfig("EIWorkflowStateCreatedID");
            var eventWorkflowFinalId = _standardHelper.GetItemIdFromConfig("EIWorkflowStateFinalID");

            if(eventDetailsTemplateId == ID.Null ||
               eventListingTemplateId == ID.Null ||
               eventWorkflowId == ID.Null ||
               eventWorkflowCreatedId == ID.Null ||
               eventWorkflowFinalId == ID.Null)
            {
                //catch the error and return it in the model
                model.ErrorMessage = "One of the id's used in the importer is missing.";
                return View(model);
            }

            // create the sitecore items
            var database = Sitecore.Configuration.Factory.GetDatabase("master");
            var parentItem = database.GetItem(parentPath);
            var templateId = new TemplateID(eventDetailsTemplateId);
            //check if the parent item from the path is an events listing
            if(parentItem.TemplateID != eventListingTemplateId)
            {
                //catch the error and return it in the model
                model.ErrorMessage = "The selected parent item is not a Events listing page, please check the path and try again";
                return View(model);
            }

            if (events != null)
            {
                using (new SecurityDisabler())
                {
                    foreach (var eventItem in events)
                    {
                        var name = ItemUtil.ProposeValidItemName(eventItem.Name);
                        // check if the parent has already got the same event using the name, then update it instead of creating a new one
                        if (parentItem.HasChildren && parentItem.Children.Any(eventChild => eventChild.Name == name))
                        {
                            var childEvent = parentItem.Children.FirstOrDefault(eventChild => eventChild.Name == name);
                            if (childEvent != null)
                            {
                                var eventToUpdateId = childEvent.ID;
                                var eventToUpdate = database.GetItem(eventToUpdateId);
                                if (eventToUpdate != null)
                                {
                                    // if the event is in the final state then it cant be 
                                    if (eventToUpdate.State.GetWorkflowState() != null &&
                                        eventToUpdate.State.GetWorkflowState().FinalState)
                                    {
                                        //catch the error and return it in the model
                                        model.ErrorMessage =
                                            "The event: " + name +
                                            ", to be updated is in the final state of the work flow and cant be updated until published.";
                                        return View(model);
                                    }
                                    // update the version
                                    eventToUpdate.Versions.AddVersion();
                                    // then update the events details
                                    try
                                    {
                                        eventToUpdate.Editing.BeginEdit();

                                        eventToUpdate["ContentHeading"] = eventItem.ContentHeading;
                                        eventToUpdate["ContentIntro"] = eventItem.ContentIntro;
                                        eventToUpdate["Highlights"] = eventItem.Highlights;
                                        eventToUpdate["Date"] = DateUtil.ToIsoDate(eventItem.StartDate);
                                        eventToUpdate["Duration"] = eventItem.Duration.ToString();
                                        eventToUpdate["Difficulty-Level"] = eventItem.Difficulty.ToString();

                                        // add it to the final review state of the workflow 
                                        eventToUpdate[FieldIDs.Workflow] = eventWorkflowId.ToString();
                                        eventToUpdate[FieldIDs.WorkflowState] = eventWorkflowFinalId.ToString();

                                        eventToUpdate.Editing.EndEdit();
                                    }
                                    catch (Exception ex)
                                    {
                                        eventToUpdate.Editing.CancelEdit();
                                        //catch the error and return it in the model
                                        model.ErrorMessage = ex.Message;
                                        return View(model);
                                    }
                                }
                                else
                                {
                                    //catch the error and return it in the model
                                    model.ErrorMessage =
                                        "The event: " + name + ", to be updated is not in the current database";
                                    return View(model);
                                }
                            }
                        }
                        //if we are not updating then add a new one
                        else
                        {
                            Item newEventItem = parentItem.Add(name, templateId);
                            try
                            {
                                newEventItem.Editing.BeginEdit();

                                newEventItem["ContentHeading"] = eventItem.ContentHeading;
                                newEventItem["ContentIntro"] = eventItem.ContentIntro;
                                newEventItem["Highlights"] = eventItem.Highlights;
                                newEventItem["Date"] = DateUtil.ToIsoDate(eventItem.StartDate);
                                newEventItem["Duration"] = eventItem.Duration.ToString();
                                newEventItem["Difficulty-Level"] = eventItem.Difficulty.ToString();

                                newEventItem[FieldIDs.Workflow] = eventWorkflowId.ToString();
                                newEventItem[FieldIDs.WorkflowState] = eventWorkflowCreatedId.ToString();

                                newEventItem.Editing.EndEdit();
                            }
                            catch (Exception ex)
                            {
                                newEventItem.Editing.CancelEdit();
                                //catch the error and return it in the model
                                model.ErrorMessage = ex.Message;
                                return View(model);
                            }
                        }


                    }
                    message = "All the events have been imported, pending reviewing";
                }
            }

            //set the success message and return it in the view
            model.SuccessMessage = message;
            return View(model);
        }
    }
}