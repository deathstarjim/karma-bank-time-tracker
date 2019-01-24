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
        private ISecurity _security;
        private ISystemRole _roles;

        public RegistrationController(IOrganization org, IAdministrator admin, ISecurity security, ISystemRole roles)
        {
            _org = org;
            _admin = admin;
            _security = security;
            _roles = roles;
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

        [HttpPost]
        public ActionResult CreateAdmin(RegistrationViewModel model)
        {
            string salt = _security.GenerateSalt();
            string securedPassword = _security.HashPassword(model.CurrentAdmin.Password);
            var roles = _roles.GetSystemRoles();

            model.CurrentAdmin.Password = securedPassword;
            model.CurrentAdmin.PasswordSalt = salt;

            model.CurrentAdmin.Role = roles.Where(r => r.Name == "Organizational Administrator").FirstOrDefault();

            if (Session["CurrentOrg"] != null)
                model.CurrentOrg = (Organization)Session["CurrentOrg"];

            model.CurrentAdmin.OrganizationId = model.CurrentOrg.Id;

            _admin.CreateAdministrator(model.CurrentAdmin);

            return RedirectToAction("Index");
        }
    }
}