using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TimeTracker.Core.Contracts;
using TimeTracker.Core.Models;
using TimeTracker.UI.Areas.OrgAdmins.Tools;
using TimeTracker.UI.Areas.OrgAdmins.ViewModels;
using TimeTracker.UI.Models;

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
            try
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
            catch (Exception ex)
            {
                Error error = new Error
                {
                    Message = ex.Message,
                    InnerException = (ex.InnerException != null) ? ex.InnerException.Message : "",
                    ControllerName = "Volunteer Opportunities",
                    ActionName = "Index"
                };

                TempData["Error"] = error;

                return RedirectToAction("Index", "Errors", new { Area = "" });
            }
        }

        [HttpPost]
        public ActionResult Index(VolunteerOpportunityViewModel model)
        {

            try
            {
                model.CurrentOpportunity.Image = UI.Tools.FileTools.ConvertImageToBytes(model.CurrentOpportunity.PostedFile);

                var volOppDates = OrgAdminTools.GetDateRange(model.CurrentOpportunity.DateRange);

                if(volOppDates != null && volOppDates.Count > 0)
                {
                    model.CurrentOpportunity.StartDateTime = (DateTime)volOppDates.FirstOrDefault();
                    model.CurrentOpportunity.EndDateTime = (DateTime)volOppDates.LastOrDefault();
                }

                _volunteerOpportunity.CreateVolunteerOpportunity(model.CurrentOpportunity);

                model.VolunteerOpportunities = _volunteerOpportunity.GetVolunteerOpportunities();

                model.CurrentOpportunity = new VolunteerOpportunity();

                ModelState.Clear();

                Session["CurrentMessage"] = UI.Tools.Messages.CreateMessage("Volunteer Opportunity Created!", "You have successfully created a new Volunteer Opportunity.",
                    MessageConstants.Success);

                return View(model);

            }
            catch (Exception ex)
            {
                Error error = new Error
                {
                    Message = ex.Message,
                    InnerException = (ex.InnerException != null) ? ex.InnerException.Message : "",
                    ControllerName = "Volunteer Opportunities",
                    ActionName = "Index - Post"
                };

                TempData["Error"] = error;

                return RedirectToAction("Index", "Errors", new { Area = "" });
            }
        }

        public ActionResult EditVolunteerOpportunity(Guid volunteerOpportunityId)
        {
            try
            {
                if (volunteerOpportunityId == Guid.Empty)
                    return RedirectToAction("Index");

                VolunteerOpportunityViewModel model = new VolunteerOpportunityViewModel();

                model.CurrentOpportunity = _volunteerOpportunity.GetVolunteerOpportunityById(volunteerOpportunityId);

                model.CurrentOpportunity.DateRange = string.Format("{0} - {1}",  model.CurrentOpportunity.StartDateTime.ToString(),
                    model.CurrentOpportunity.EndDateTime.ToString());

                return View(model);

            }
            catch (Exception ex)
            {
                Error error = new Error
                {
                    Message = ex.Message,
                    InnerException = (ex.InnerException != null) ? ex.InnerException.Message : "",
                    ControllerName = "Volunteer Opportunities",
                    ActionName = "EditVolunteerOpportunity"
                };

                TempData["Error"] = error;

                return RedirectToAction("Index", "Errors", new { Area = "" });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditVolunteerOpportunity(VolunteerOpportunityViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (model.CurrentOpportunity.PostedFile != null)
                    {
                        bool fileExtCheck = CheckFileExtension(model.CurrentOpportunity.PostedFile);

                        if (fileExtCheck)
                        {
                            model.CurrentOpportunity.Image = UI.Tools.FileTools.ConvertImageToBytes(model.CurrentOpportunity.PostedFile);
                            _volunteerOpportunity.UpdateVolunteerOpportunityImage(model.CurrentOpportunity);
                        }

                        if (!fileExtCheck)
                        {
                            ViewBag.FileExtError = "Please select a valid file: .png, .jpg, .jpeg or .gif.";
                            return View(model);
                        }
                    }

                    var volOppDates = OrgAdminTools.GetDateRange(model.CurrentOpportunity.DateRange);

                    if(volOppDates != null && volOppDates.Count > 0)
                    {
                        model.CurrentOpportunity.StartDateTime = (DateTime)volOppDates.FirstOrDefault();
                        model.CurrentOpportunity.EndDateTime = (DateTime)volOppDates.LastOrDefault();
                    }

                    _volunteerOpportunity.UpdateVolunteerOpportunity(model.CurrentOpportunity);

                    model.CurrentOpportunity = _volunteerOpportunity.GetVolunteerOpportunityById(model.CurrentOpportunity.Id);

                    Session["CurrentMessage"] = UI.Tools.Messages.CreateMessage("Volunteer Opportunity Updated!", "You have successfully updated the Volunteer Opportunity.",
                        MessageConstants.Success);

                    return View(model); 
                }

                if (!ModelState.IsValid)
                {
                    model.Message = model.CurrentOpportunity.PostedFile.FileName + " exceeds the maximum file size of 4 MB.";
                    model.IsValid = false;
                }

                return View(model);

            }
            catch (Exception ex)
            {
                Error error = new Error
                {
                    Message = ex.Message,
                    InnerException = (ex.InnerException != null) ? ex.InnerException.Message : "",
                    ControllerName = "Volunteer Opportunities",
                    ActionName = "EditVolunteerOpportunity - Post"
                };

                TempData["Error"] = error;

                return RedirectToAction("Index", "Errors", new { Area = "" });
            }
        }

        public ActionResult RemoveVolOppImage(Guid volOppId)
        {
            try
            {
                VolunteerOpportunityViewModel model = new VolunteerOpportunityViewModel();

                model.CurrentOpportunity = _volunteerOpportunity.GetVolunteerOpportunityById(volOppId);

                model.CurrentOpportunity.Image = null;

                _volunteerOpportunity.UpdateVolunteerOpportunityImage(model.CurrentOpportunity);

                return RedirectToAction("EditVolunteerOpportunity", new { volunteerOpportunityId = model.CurrentOpportunity.Id });

            }
            catch (Exception ex)
            {
                Error error = new Error
                {
                    Message = ex.Message,
                    InnerException = (ex.InnerException != null) ? ex.InnerException.Message : "",
                    ControllerName = "Volunteer Opportunities",
                    ActionName = "RemoveVolOppImage"
                };

                TempData["Error"] = error;

                return RedirectToAction("Index", "Errors", new { Area = "" });
            }
        }

        private bool CheckFileExtension(HttpPostedFileBase uploadedFile)
        {
            var supportedTypes = new[] { "jpg", "gif", "png", "jpeg"};
            var fileExt = System.IO.Path.GetExtension(uploadedFile.FileName).Substring(1).ToLower();
            return supportedTypes.Contains(fileExt);
        }
    }
}