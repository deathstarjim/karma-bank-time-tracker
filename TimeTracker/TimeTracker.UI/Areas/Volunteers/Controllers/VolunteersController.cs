using System;
using System.Linq;
using System.Web.Mvc;
using TimeTracker.Core.Contracts;
using TimeTracker.Core.Models;
using TimeTracker.UI.Areas.Volunteers.ViewModels;
using TimeTracker.UI.Models;

namespace TimeTracker.UI.Areas.Volunteers.Controllers
{
    public class VolunteersController : Controller
    {
        private IVolunteer _volunteer;
        private ISystemRole _roles;
        private IVolunteerOpportunity _volunteerOpportunity;
        private IVolTimePunch _timePunches;

        public VolunteersController(IVolunteer volunteer, ISystemRole roles, IVolunteerOpportunity volunteerOpportunity, IVolTimePunch timePunches)
        {
            _volunteer = volunteer;
            _volunteerOpportunity = volunteerOpportunity;
            _roles = roles;
            _timePunches = timePunches;
        }

        public ActionResult Index()
        {
            try
            {
                VolunteerViewModel model = new VolunteerViewModel();

                model.Volunteers = _volunteer.GetVolunteers();
                model.OpenTimePunches = _timePunches.GetOpenTimePunches();
                model.ControllerName = "Volunteers";

                return View(model);

            }
            catch (Exception ex)
            {
                Error error = new Error { Message = ex.Message, InnerException = (ex.InnerException != null) ? ex.InnerException.Message : "",
                    ControllerName = "Volunteers", ActionName = "Index" };

                TempData["Error"] = error;

                return RedirectToAction("Index", "Errors", new { Area = "" });
            }
        }

        [HttpPost]
        public ActionResult Index(VolunteerViewModel model)
        {
            try
            {
                model.CurrentVolunteer.Id = _volunteer.CreateVolunteer(model.CurrentVolunteer);
                model.CurrentVolunteer = new Volunteer();
                model.Volunteers = _volunteer.GetVolunteers();

                return View(model);

            }
            catch (Exception ex)
            {
                Error error = new Error
                {
                    Message = ex.Message,
                    InnerException = (ex.InnerException != null) ? ex.InnerException.Message : "",
                    ControllerName = "Volunteers",
                    ActionName = "Index - Post"
                };

                TempData["Error"] = error;

                return RedirectToAction("Index", "Errors", new { Area = "" });
            }
        }

        public ActionResult VolunteerRegistration()
        {
            try
            {
                VolunteerViewModel model = new VolunteerViewModel();

                return View(model);

            }
            catch (Exception ex)
            {
                Error error = new Error
                {
                    Message = ex.Message,
                    InnerException = (ex.InnerException != null) ? ex.InnerException.Message : "",
                    ControllerName = "Volunteers",
                    ActionName = "VolunteerRegistration"
                };

                TempData["Error"] = error;

                return RedirectToAction("Index", "Errors", new { Area = "" });
            }
        }

        [HttpPost]
        public ActionResult VolunteerRegistration(VolunteerViewModel model)
        {
            try
            {
                var roles = _roles.GetSystemRoles();

                model.CurrentVolunteer.Role = roles.Where(r => r.Name == "Volunteer").FirstOrDefault();

                _volunteer.CreateVolunteer(model.CurrentVolunteer);

                Session["CurrentMessage"] = Tools.Messages.CreateMessage("Volunteer Registered!", model.CurrentVolunteer.FullName + " has been successfully registered.",
                    MessageConstants.Success);

                model.CurrentVolunteer = new Volunteer();

                ModelState.Clear();

                return View(model);

            }
            catch (Exception ex)
            {
                Error error = new Error
                {
                    Message = ex.Message,
                    InnerException = (ex.InnerException != null) ? ex.InnerException.Message : "",
                    ControllerName = "Volunteers",
                    ActionName = "VolunteerRegistration - Post"
                };

                TempData["Error"] = error;

                return RedirectToAction("Index", "Errors", new { Area = "" });
            }
        }

        [HttpPost]
        public ActionResult VolunteerNameSearch(VolunteerViewModel model)
        {
            try
            {
                model.Volunteers = _volunteer.SearchVolunteersByName(model.SearchTerm);

                return PartialView("_VolunteerNameSearch", model);

            }
            catch (Exception ex)
            {
                Error error = new Error
                {
                    Message = ex.Message,
                    InnerException = (ex.InnerException != null) ? ex.InnerException.Message : "",
                    ControllerName = "Volunteers",
                    ActionName = "VolunteerNameSearch"
                };

                TempData["Error"] = error;

                return RedirectToAction("Index", "Errors", new { Area = "" });
            }
        }

        public ActionResult OpportunitiesList(Guid volunteerId)
        {
            try
            {
                if (volunteerId == Guid.Empty)
                {
                    Session["CurrentMessage"] = Tools.Messages.CreateMessage("Volunteer Not Found!", " There was an issue with the selected volunteer. Please try again.",
                        MessageConstants.Error, 6000);

                    return RedirectToAction("Index", "Volunteers");
                }

                if (_timePunches.CheckVolunteerClockedIn(volunteerId))
                {
                    GetVolunteerClockedInMessage(volunteerId);

                    return RedirectToAction("Index", "Volunteers");
                }

                VolunteerViewModel model = new VolunteerViewModel();

                model.CurrentVolunteer = _volunteer.GetVolunteerById(volunteerId);

                Session["CurrentVolunteer"] = model.CurrentVolunteer;

                model.VolunteerOpportunities = _volunteerOpportunity.GetVisibleOpportunities();

                return View(model);

            }
            catch (Exception ex)
            {
                Error error = new Error
                {
                    Message = ex.Message,
                    InnerException = (ex.InnerException != null) ? ex.InnerException.Message : "",
                    ControllerName = "Volunteers",
                    ActionName = "OpportunitiesList"
                };

                TempData["Error"] = error;

                return RedirectToAction("Index", "Errors", new { Area = "" });
            }
        }

        public ActionResult ClockVolunteerIn(Guid volunteerId, Guid volOppId, int volOppLimit)
        {
            try
            {
                //Get the number of volunteers currently logged into this opportunity
                int clockedInCount = _volunteerOpportunity.GetClockedInVolunteerCount(volOppId);

                if (volOppLimit == 0)
                {
                    CreateTimePunchRecord(volunteerId, volOppId);
                    GetClockedInSuccessMessage();
                }

                if (volOppLimit > 0 && clockedInCount < volOppLimit)
                {
                    CreateTimePunchRecord(volunteerId, volOppId);
                    GetClockedInSuccessMessage();
                }

                if (volOppLimit > 0 && clockedInCount == volOppLimit)
                    GetClockedInLimitMessage();

                return RedirectToAction("Index", "Volunteers");

            }
            catch (Exception ex)
            {
                Error error = new Error
                {
                    Message = ex.Message,
                    InnerException = (ex.InnerException != null) ? ex.InnerException.Message : "",
                    ControllerName = "Volunteers",
                    ActionName = "ClockVolunteerIn"
                };

                TempData["Error"] = error;

                return RedirectToAction("Index", "Errors", new { Area = "" });
            }
        }

        public ActionResult ClockVolunteerOut(Guid timePunchId)
        {
            try
            {
                Volunteer currentVolunteer = new Volunteer();

                var punch = _timePunches.GetTimePunchById(timePunchId);

                if (punch != null)
                {
                    var volOpp = _volunteerOpportunity.GetVolunteerOpportunityById(punch.VolunteerOpportunityId);
                    currentVolunteer = _volunteer.GetVolunteerById(punch.VolunteerId);

                    if (volOpp != null)
                    {
                        punch.PunchOutDateTime = DateTime.Now;
                        _timePunches.UpdateTimePunchOut(punch, volOpp.CreditValue);

                        if (currentVolunteer.Id != Guid.Empty)
                        {
                            _volunteer.UpdateCreditBalance(currentVolunteer.Id);

                            Session["CurrentMessage"] = Tools.Messages.CreateMessage("Clocked Out!", currentVolunteer.FullName + " has been successfully clocked out.",
                                MessageConstants.Success);
                        }

                        Session["CurrentVolunteer"] = null;
                    }
                }

                return RedirectToAction("Index", "Volunteers");

            }
            catch (Exception ex)
            {
                Error error = new Error
                {
                    Message = ex.Message,
                    InnerException = (ex.InnerException != null) ? ex.InnerException.Message : "",
                    ControllerName = "Volunteers",
                    ActionName = "ClockVolunteerOut"
                };

                TempData["Error"] = error;

                return RedirectToAction("Index", "Errors", new { Area = "" });
            }
        }

        private void GetVolunteerClockedInMessage(Guid volunteerId)
        {
            var volunteer = _volunteer.GetVolunteers().Where(v => v.Id == volunteerId).FirstOrDefault();

            Session["CurrentMessage"] = Tools.Messages.CreateMessage("Volunteer Already Clocked In!",
                volunteer.FullName + " is already clocked in. Please clock out and then try again.",
                MessageConstants.Error, 6000);

        }

        private void GetClockedInLimitMessage()
        {
            Session["CurrentMessage"] = Tools.Messages.CreateMessage("Volunteer Limit Reached!",
                "The limit for this opportunity has been reached. Please wait for someone to clock out before clocking in.",
                MessageConstants.Error, 6000);
        }

        private void GetClockedInSuccessMessage()
        {
            Session["CurrentMessage"] = Tools.Messages.CreateMessage("Clocked In!",
                "You have successfully clocked into the volunteer opportunity.",
                MessageConstants.Success, 3000);
        }

        private void CreateTimePunchRecord(Guid volunteerId, Guid volOppId)
        {
            _timePunches.CreateTimePunchIn(new VolTimePunch
            {
                VolunteerId = volunteerId,
                PunchInDateTime = DateTime.Now,
                VolunteerOpportunityId = volOppId
            });

        }
    }
}