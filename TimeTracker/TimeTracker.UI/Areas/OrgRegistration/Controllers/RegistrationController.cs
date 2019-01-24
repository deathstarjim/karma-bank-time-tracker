using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TimeTracker.Core.Contracts;
using TimeTracker.Core.Models;
using TimeTracker.UI.Areas.OrgRegistration.ViewModels;

namespace TimeTracker.UI.Areas.OrgRegistration.Controllers
{
    public class RegistrationController : Controller
    {
        private IOrganization _org;
        private IAdministrator _admin;

        public RegistrationController(IOrganization org, IAdministrator admin)
        {
            _org = org;
            _admin = admin;
        }
        
        public ActionResult Index()
        {
            RegistrationViewModel model = new RegistrationViewModel();

            if (Session["CurrentOrg"] != null)
                model.CurrentOrg = (Organization)Session["CurrentOrg"];

            if (Session["CurrentAdmin"] != null)
                model.CurrentAdmin = (Administrator)Session["CurrentAdmin"];

            return View(model);
        }

        [HttpPost]
        public ActionResult CreateOrg(RegistrationViewModel model)
        {
            model.CurrentOrg.TaxExemptionFile = Tools.FileTools.ConvertImageToBytes(model.CurrentOrg.PostedFile);

            model.CurrentOrg = _org.CreateOrganization(model.CurrentOrg);

            Session["CurrentOrg"] = model.CurrentOrg;

            return RedirectToAction("Index");
        }
    }
}