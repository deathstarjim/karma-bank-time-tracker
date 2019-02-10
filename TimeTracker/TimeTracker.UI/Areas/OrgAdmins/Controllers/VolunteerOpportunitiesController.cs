using System;
using System.Web.Mvc;
using System.Linq;
using TimeTracker.Core.Contracts;
using TimeTracker.Core.Models;
using TimeTracker.UI.Areas.OrgAdmins.ViewModels;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Web;

namespace TimeTracker.UI.Areas.OrgAdmins.Controllers
{
    [Authorize]
    public class VolunteerOpportunitiesController : Controller
    {
        private IVolunteerOpportunity _volunteerOpportunity;
        private IAdministrator _admins;

        public VolunteerOpportunitiesController(IVolunteerOpportunity volunteerOpportunity, IAdministrator admins)
        {
            _volunteerOpportunity = volunteerOpportunity;
            _admins = admins;
        }

        public ActionResult Index()
        {
            VolunteerOpportunityViewModel model = new VolunteerOpportunityViewModel();

            model.VolunteerOpportunities = _volunteerOpportunity.GetVolunteerOpportunities();
            Guid currentAdminId = new Guid();

            if (Session["CurrentUserId"] == null)
                return RedirectToAction("Index", "Login", new { Area = "OrgAdmins" });

            if (Session["CurrentUserId"] != null)
                currentAdminId = (Guid)Session["CurrentUserId"];

            model.CurrentAdministrator = _admins.GetAdministrators().Where(a => a.Id == currentAdminId).FirstOrDefault();

            return View(model);
        }

        [HttpPost]
        public ActionResult Index(VolunteerOpportunityViewModel model)
        {

            model.CurrentOpportunity.Image = UI.Tools.FileTools.ConvertImageToBytes(model.CurrentOpportunity.PostedFile);

            _volunteerOpportunity.CreateVolunteerOpportunity(model.CurrentOpportunity);

            model.VolunteerOpportunities = _volunteerOpportunity.GetVolunteerOpportunities();

            model.CurrentOpportunity = new VolunteerOpportunity();

            ModelState.Clear();

            Session["CurrentMessage"] = UI.Tools.Messages.CreateMessage("Volunteer Opportunity Created!", "You have successfully created a new Volunteer Opportunity.", 
                Models.MessageConstants.Success);

            return View(model);
        }

        public ActionResult EditVolunteerOpportunity(Guid volunteerOpportunityId)
        {
            if (volunteerOpportunityId == Guid.Empty)
                return RedirectToAction("Index");

            VolunteerOpportunityViewModel model = new VolunteerOpportunityViewModel();

            model.CurrentOpportunity = _volunteerOpportunity.GetVolunteerOpportunities().Where(v => v.Id == volunteerOpportunityId).FirstOrDefault();

            return View(model);
        }

        [HttpPost]
        public ActionResult EditVolunteerOpportunity(VolunteerOpportunityViewModel model)
        {
            try
            {
                if(model.CurrentOpportunity.PostedFile != null && model.CurrentOpportunity.PostedFile.ContentLength > 0)
                {
                    model.CurrentOpportunity.Image = UI.Tools.FileTools.ConvertImageToBytes(model.CurrentOpportunity.PostedFile);
                    _volunteerOpportunity.UpdateVolunteerOpportunityImage(model.CurrentOpportunity);
                }

                _volunteerOpportunity.UpdateVolunteerOpportunity(model.CurrentOpportunity);

                model.CurrentOpportunity = _volunteerOpportunity.GetVolunteerOpportunityById(model.CurrentOpportunity.Id);

                Session["CurrentMessage"] = UI.Tools.Messages.CreateMessage("Volunteer Opportunity Updated!", "You have successfully updated the Volunteer Opportunity.", 
                    Models.MessageConstants.Success);

                return View(model);

            }
            catch (Exception ex)
            {
                return View(model);
            }
        }
    }
}