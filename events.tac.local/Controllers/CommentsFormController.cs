using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TAC.Utils.Helpers;

namespace events.tac.local.Controllers
{
    public class CommentsFormController : Controller
    {
        // GET: CommentsForm
        public ActionResult Index()
        {
            return View();
        }

        // recieve the posted form
        //[HttpPost]
        //[ValidateFormHandler]
        //public ActionResult Index(string email)
        //{
        //    return View("Confirmation");
        //}
    }
}