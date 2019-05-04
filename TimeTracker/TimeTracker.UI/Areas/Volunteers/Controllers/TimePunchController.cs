using System;
using System.Linq;
using System.Web.Mvc;
using TimeTracker.Core.Contracts;
using TimeTracker.UI.Areas.Volunteers.ViewModels;
using TimeTracker.UI.Models;

namespace TimeTracker.UI.Areas.Volunteers.Controllers
{
    public class TimePunchController : Controller
    {
        private IVolTimePunch _timePunch;
        private IVolunteer _volunteer;
        private IVolunteerOpportunity _opportunities;
        private IVolTransaction _transactions;

        public TimePunchController(IVolTimePunch timePunch, IVolunteer volunteer, IVolunteerOpportunity opportunity, IVolTransaction transactions)
        {
            _timePunch = timePunch;
            _volunteer = volunteer;
            _opportunities = opportunity;
            _transactions = transactions;
        }

        public ActionResult Index()
        {
            try
            {
                VolunteerViewModel model = new VolunteerViewModel();
                model.ControllerName = "TimePunch";

                return View(model);

            }
            catch (Exception ex)
            {
                Error error = new Error
                {
                    Message = ex.Message,
                    InnerException = (ex.InnerException != null) ? ex.InnerException.Message : "",
                    ControllerName = "Time Punch",
                    ActionName = "Index"
                };

                TempData["Error"] = error;

                return RedirectToAction("Index", "Errors", new { Area = "" });
            }
        }

        public ActionResult GetVolunteerHours(Guid volunteerId)
        {
            try
            {
                VolunteerViewModel model = new VolunteerViewModel();

                model.CurrentVolunteer = _volunteer.GetVolunteerById(volunteerId);

                model.OpenTimePunches = _timePunch.GetTimePunchesByVolunteerId(volunteerId).OrderByDescending(v => v.PunchOutDateTime).ToList();
                model.VolunteerTransactions = _transactions.GetTransactionsByVolunteerId(volunteerId);

                return View("GetVolunteerHours", model);

            }
            catch (Exception ex)
            {
                Error error = new Error
                {
                    Message = ex.Message,
                    InnerException = (ex.InnerException != null) ? ex.InnerException.Message : "",
                    ControllerName = "Time Punch",
                    ActionName = "GetVolunteerHours"
                };

                TempData["Error"] = error;

                return RedirectToAction("Index", "Errors", new { Area = "" });
            }
        }
    }
}