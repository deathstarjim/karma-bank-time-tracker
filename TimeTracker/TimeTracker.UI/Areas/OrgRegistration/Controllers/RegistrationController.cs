using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TimeTracker.UI.Areas.OrgRegistration.ViewModels;

namespace TimeTracker.UI.Areas.OrgRegistration.Controllers
{
    public class RegistrationController : Controller
    {
        // GET: OrgRegistration/Registration
        public ActionResult Index()
        {
            RegistrationViewModel model = new RegistrationViewModel();

            return View(model);
        }
    }
}