using System;
using System.Linq;
using System.Web.Mvc;
using TimeTracker.Core.Contracts;
using TimeTracker.Core.Models;
using TimeTracker.UI.Areas.Volunteers.ViewModels;

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
            VolunteerViewModel model = new VolunteerViewModel();

            model.Volunteers = _volunteer.GetVolunteers();
            model.OpenTimePunches = _timePunches.GetOpenTimePunches();
            model.ControllerName = "Volunteers";

            return View(model);
        }

        [HttpPost]
        public ActionResult Index(VolunteerViewModel model)
        {
            model.CurrentVolunteer.Id = _volunteer.CreateVolunteer(model.CurrentVolunteer);
            model.CurrentVolunteer = new Volunteer();
            model.Volunteers = _volunteer.GetVolunteers();

            return View(model);
        }

        public ActionResult VolunteerRegistration()
        {
            VolunteerViewModel model = new VolunteerViewModel();

            return View(model);
        }

        [HttpPost]
        public ActionResult VolunteerRegistration(VolunteerViewModel model)
        {
            var roles = _roles.GetSystemRoles();

            model.CurrentVolunteer.Role = roles.Where(r => r.Name == "Volunteer").FirstOrDefault();

            _volunteer.CreateVolunteer(model.CurrentVolunteer);

            Session["CurrentMessage"] = Tools.Messages.CreateMessage("Volunteer Registered!", model.CurrentVolunteer.FullName + " has been successfully registered.", 
                Models.MessageConstants.Success);

            model.CurrentVolunteer = new Volunteer();

            ModelState.Clear();

            return View(model);
        }

        [HttpPost]
        public ActionResult VolunteerNameSearch(VolunteerViewModel model)
        {
            model.Volunteers = _volunteer.SearchVolunteersByName(model.SearchTerm);

            model.ResultsMessage = model.Volunteers.Count.ToString() + " volunteers found.";

            return PartialView("_VolunteerNameSearch", model);
        }

        public ActionResult OpportunitiesList(Guid volunteerId)
        {
            if (volunteerId == Guid.Empty)
            {
                Session["CurrentMessage"] = Tools.Messages.CreateMessage("Volunteer Not Found!", " There was an issue with the selected volunteer. Please try again.", 
                    Models.MessageConstants.Error, 6000);

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

        public ActionResult ClockVolunteerIn(Guid volunteerId, Guid volOppId, int volOppLimit)
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

        public ActionResult ClockVolunteerOut(Guid timePunchId)
        {
            Volunteer currentVolunteer = new Volunteer();

            var punch = _timePunches.GetTimePunchById(timePunchId);

            if(punch != null)
            {
                var volOpp = _volunteerOpportunity.GetVolunteerOpportunityById(punch.VolunteerOpportunityId);
                currentVolunteer = _volunteer.GetVolunteerById(punch.VolunteerId);

                if (volOpp != null)
                {
                    punch.PunchOutDateTime = DateTime.Now;
                    _timePunches.UpdateTimePunchOut(punch, volOpp.CreditValue);

                    if(currentVolunteer.Id != Guid.Empty)
                    {
                        _volunteer.UpdateCreditBalance(currentVolunteer.Id);

                        Session["CurrentMessage"] = Tools.Messages.CreateMessage("Clocked Out!", currentVolunteer.FullName + " has been successfully clocked out.", 
                            Models.MessageConstants.Success);
                    }

                    Session["CurrentVolunteer"] = null;
                }
            }

            return RedirectToAction("Index", "Volunteers");
        }

        private void GetVolunteerClockedInMessage(Guid volunteerId)
        {
            var volunteer = _volunteer.GetVolunteers().Where(v => v.Id == volunteerId).FirstOrDefault();

            Session["CurrentMessage"] = Tools.Messages.CreateMessage("Volunteer Already Clocked In!",
                volunteer.FullName + " is already clocked in. Please clock out and then try again.",
                Models.MessageConstants.Error, 6000);
        }

        private void GetClockedInLimitMessage()
        {
            Session["CurrentMessage"] = Tools.Messages.CreateMessage("Volunteer Limit Reached!",
                "The limit for this opportunity has been reached. Please wait for someone to clock out before clocking in.",
                Models.MessageConstants.Error, 6000);
        }

        private void GetClockedInSuccessMessage()
        {
            Session["CurrentMessage"] = Tools.Messages.CreateMessage("Clocked In!",
                "You have successfully clocked into the volunteer opportunity.",
                Models.MessageConstants.Success, 3000);
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