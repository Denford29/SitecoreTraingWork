using Sitecore.Analytics.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TAC.Utils.Helpers;
using Sitecore.Analytics;
using Sitecore.Analytics.Outcome.Extensions;
using Sitecore.Analytics.Outcome.Model;
using Sitecore.Data;

namespace events.tac.local.Controllers
{
    public class SubscribeFormController : Controller
    {
        // GET: SubscribeForm
        public ActionResult Index()
        {
            return View();
        }

        // recieve the posted form
        [HttpPost]
        [ValidateFormHandler]
        public ActionResult Index(string email)
        {
            // save the  email to the current tracked user if it doesnt exsist
            Tracker.Current.Session.Identify(email);
            var contact = Tracker.Current.Contact;
            var emails = contact.GetFacet<IContactEmailAddresses>("Emails");
            if(!emails.Entries.Contains("personal"))
            {
                emails.Preferred = "personal";
                var personalEmail = emails.Entries.Create("personal");
                personalEmail.SmtpAddress = email;
            }

            // create a new outcome object
            var subscribeOutcome = new ContactOutcome
            (
                 ID.NewID,
                 new ID("{CF6C658B-1343-4905-8F4D-6E22594B5702}"),
                 new ID(Tracker.Current.Contact.ContactId)
            );

            Tracker.Current.RegisterContactOutcome(subscribeOutcome);

            return View("Confirmation");
        }
    }
}