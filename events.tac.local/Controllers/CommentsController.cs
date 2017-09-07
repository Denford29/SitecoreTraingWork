using System.Linq;
using System.Web.Mvc;
using events.tac.local.Models;
using SATC.SC.Framework.SitecoreHelpers;
using Sitecore.Data;
using Sitecore.Mvc.Presentation;
using TAC.Utils.Helpers;

namespace events.tac.local.Controllers
{
    public class CommentsController : Controller
    {

        /// <summary>
        /// set a constant to use for the page size
        /// </summary>
        //private const int PageSize = 5;

        /// <summary>
        /// initiate a database to use
        /// </summary>
        //private readonly Database _database;

        /// <summary>
        /// inittiate the field for the standard helpers from the SATC SC Framework
        /// </summary>
        private readonly StandardHelpers _standardHelpers;

        /// <summary>
        ///  Create the constructor and assing any fields
        /// </summary>
        /// <param name="standardHelpers"></param>
        public CommentsController
        (
            StandardHelpers standardHelpers
        )
        {
            _standardHelpers = standardHelpers;
            //_database = RenderingContext.Current.ContextItem.Database;
        }

        /// <summary>
        /// Get the comments form
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetPageCommentsList(int page = 1)
        {
            // get the current context item
            var contextItem = RenderingContext.Current.ContextItem;

            //create the default model
            var model = new EventCommentsListModel();

            //get the database name, should be lower anyway but just in case
            //var databaseName = _database.Name.ToLower();

            //get the index name
            //var indexName = string.Format("sitecore_{0}_index", databaseName);
            //var indexName = $"sitecore_{databaseName}_index";

            //get the index to use
            //var sitecoreIndex = ContentSearchManager.GetIndex(indexName);

            //get the comments template id
            var eventCommentsTemplateId = _standardHelpers.GetTemplateIdFromConfig("EventCommentsTemplateID", contextItem);

            //check the index and the template id, testing how to use the standard index
            //if (sitecoreIndex != null && eventCommentsTemplateId != new TemplateID().ID)
            //{
            //    //use the indexes search  to get the comments, need to adjust the index to have the properties we need to populate comment list item model
            //    using (var context = sitecoreIndex.CreateSearchContext())
            //    {
            //        // get the results
            //        var results = context.GetQueryable<CommentListItemModel>()
            //            .Where(item => item.Paths.Contains(contextItem.ID)
            //                         && item.Language == contextItem.Language.Name
            //                         && item.TemplateId == eventCommentsTemplateId.ID)
            //            .Page(page - 1, PageSize)
            //            .GetResults();
            //        // assing them to our model
            //        model.PageComments = results.Hits.Select(hit => hit.Document).ToList();

            //    }
            //}

            //use the standard get children method
            if (eventCommentsTemplateId != new TemplateID().ID)
            {
                //get any child comments
                var eventComments = contextItem.GetChildren()
                    .Where(item => item.TemplateID == eventCommentsTemplateId.ID).ToList();

                // add them to the model
                if (eventComments.Any())
                {
                    model.PageComments = eventComments.Select(
                        comment => new CommentListItemModel()
                        {
                            CommentorName = comment.Fields["Name"].ToString(),
                            CommentText = comment.Fields["Comment"].ToString(),
                           CommentDate = comment.Created
                        }).ToList();
                }
            }

            //retunr the model with any comments
            return View(model);
        }

        /// <summary>
        /// recieve the posted form
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateFormHandler]
        public ActionResult Index(string email)
        {
            //var sitecoreItemService = Sitecore.
            return View("Confirmation");
        }
    }
}