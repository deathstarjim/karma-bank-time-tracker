using System;
using System.Linq;
using System.Web.Mvc;
using TimeTracker.Core.Contracts;
using TimeTracker.UI.Areas.OrgAdmins.ViewModels;

namespace TimeTracker.UI.Areas.OrgAdmins.Controllers
{
    [Authorize]
    public class VolMgmtController : Controller
    {
        private IVolunteer _vols;
        private IVolunteerOpportunity _volOpps;
        private IAdministrator _admins;
        private IVolTimePunch _punches;
        private IVolTransaction _volTransactions;
        private ITransactionType _transTypes;

        public VolMgmtController(IVolunteer vols, IAdministrator admins, IVolTimePunch punches, IVolunteerOpportunity volOpps, IVolTransaction transactions,
            ITransactionType transTypes)
        {
            _vols = vols;
            _admins = admins;
            _punches = punches;
            _volOpps = volOpps;
            _volTransactions = transactions;
            _transTypes = transTypes;
        }

        public ActionResult Index()
        {
            if (Tools.OrgAdminTools.CheckAdminLoggedOut())
                return RedirectToAction("Index", "Login", new { Area = "OrgAdmins" });

            VolMgmtViewModel model = new VolMgmtViewModel();

            model.Volunteers = _vols.GetVolunteers();
            model.CurrentAdministrator = Tools.OrgAdminTools.GetCurrentAdmin(_admins.GetAdministrators());

            return View(model);
        }

        public ActionResult VolunteerDetails(Guid volunteerId)
        {
            if(Tools.OrgAdminTools.CheckAdminLoggedOut())
                return RedirectToAction("Index", "Login", new { Area = "OrgAdmins" });

            VolMgmtViewModel model = new VolMgmtViewModel();

            model.CurrentAdministrator = Tools.OrgAdminTools.GetCurrentAdmin(_admins.GetAdministrators());
            model.Volunteers = _vols.GetVolunteers();
            model.CurrentVolunteer = _vols.GetVolunteerById(volunteerId);
            model.VolTimePunches = _punches.GetTimePunchesByVolunteerId(volunteerId);
            model.TransactionTypes = _transTypes.GetTransactionTypes();
            model.CreditTransactions = _volTransactions.GetTransactionsByVolunteerId(volunteerId);
            model.VolunteerOpportunities = _volOpps.GetVolunteerOpportunities();

            return View(model);
        }

        public ActionResult EditPunch(Guid punchId)
        {
            if (Tools.OrgAdminTools.CheckAdminLoggedOut())
                return RedirectToAction("Index", "Login", new { Area = "OrgAdmins" });

            VolMgmtViewModel model = new VolMgmtViewModel();

            model.CurrentAdministrator = Tools.OrgAdminTools.GetCurrentAdmin(_admins.GetAdministrators());
            model.CurrentVolPunch = _punches.GetTimePunchById(punchId);
            model.SelectedVolunteerOpportunity = model.CurrentVolPunch.VolunteerOpportunityId;
            model.VolunteerOpportunities = _volOpps.GetVolunteerOpportunities();

            return PartialView("_EditPunch", model);
        }

        [HttpPost]
        public ActionResult UpdatePunch(VolMgmtViewModel model)
        {
            var volOpp = _volOpps.GetVolunteerOpportunityById(model.SelectedVolunteerOpportunity);
            var currentVolunteer = _vols.GetVolunteerById(model.CurrentVolPunch.VolunteerId);

            if(volOpp != null)
            {
                model.CurrentVolPunch.VolunteerOpportunityId = model.SelectedVolunteerOpportunity;
                _punches.UpdateTimePunch(model.CurrentVolPunch, volOpp.CreditValue);

                if (currentVolunteer != null && currentVolunteer.Id != Guid.Empty)
                    _vols.UpdateCreditBalance(currentVolunteer.Id);

                Session["CurrentMessage"] = UI.Tools.Messages.CreateMessage("Time Punch Updated!", "The time punch record has been successfully updated.",
                    Models.MessageConstants.Success);
            }

            return RedirectToAction("VolunteerDetails", "VolMgmt", new { Area = "OrgAdmins", volunteerId = model.CurrentVolPunch.VolunteerId });
        }

        public ActionResult DeletePunch(Guid timePunchId, Guid volunteerId)
        {
            _punches.DeleteTimePunch(timePunchId);
            _vols.UpdateCreditBalance(volunteerId);

            Session["CurrentMessage"] = UI.Tools.Messages.CreateMessage("Time Punch Deleted!", "The time punch record has been successfully deleted.",
                Models.MessageConstants.Success);

            return RedirectToAction("VolunteerDetails", "VolMgmt", new { Area = "OrgAdmins", volunteerId = volunteerId });
        }

        public ActionResult EditTransaction(Guid transactionId)
        {
            if (Tools.OrgAdminTools.CheckAdminLoggedOut())
                return RedirectToAction("Index", "Login", new { Area = "OrgAdmins" });

            VolMgmtViewModel model = new VolMgmtViewModel();

            model.CurrentAdministrator = Tools.OrgAdminTools.GetCurrentAdmin(_admins.GetAdministrators());
            model.CreditTransaction = _volTransactions.GetTransactionById(transactionId);
            model.SelectedTransactionType = model.CreditTransaction.TransactionType.Id;
            model.TransactionTypes = _transTypes.GetTransactionTypes();

            return PartialView("_EditTransaction", model);
        }

        [HttpPost]
        public ActionResult UpdateTransaction(VolMgmtViewModel model)
        {
            model.CreditTransaction.TransactionType.Id = model.SelectedTransactionType;
            var transactionType = _transTypes.GetTransactionTypeById(model.SelectedTransactionType);

            if (transactionType != null && transactionType.Id != Guid.Empty)
            {
                //If changing from transaction type that gains credits to a purchase, make credit amount negative
                if (transactionType.MakeNegative.IsChecked && model.CreditTransaction.CreditAmount > 0)
                    model.CreditTransaction.CreditAmount = model.CreditTransaction.CreditAmount * -1;

                //If changing transaction type from purchase to something else where credits are gained
                if (!transactionType.MakeNegative.IsChecked && model.CreditTransaction.CreditAmount < 0)
                    model.CreditTransaction.CreditAmount = model.CreditTransaction.CreditAmount * -1;

                model.CreditTransaction.TransactionType = transactionType;

                _volTransactions.UpdateTransaction(model.CreditTransaction);
                _vols.UpdateCreditBalance(model.CreditTransaction.VolunteerId);
            }

            Session["CurrentMessage"] = UI.Tools.Messages.CreateMessage("Transaction Updated!", "The transaction record has been successfully updated.",
                Models.MessageConstants.Success);

            return RedirectToAction("VolunteerDetails", "VolMgmt", new { Area = "OrgAdmins", volunteerId = model.CreditTransaction.VolunteerId });
        }

        public ActionResult DeleteTransaction(Guid transactionId, Guid volunteerId)
        {
            _volTransactions.DeleteTransaction(transactionId);
            _vols.UpdateCreditBalance(volunteerId);

            Session["CurrentMessage"] = UI.Tools.Messages.CreateMessage("Transaction Deleted!", "The transaction record has been successfully deleted.",
                Models.MessageConstants.Success);


            return RedirectToAction("VolunteerDetails", "VolMgmt", new { Area = "OrgAdmins", volunteerId = volunteerId });
        }

        [HttpPost]
        public ActionResult CreditTransaction(VolMgmtViewModel model)
        {
            model.CreditTransaction.VolunteerId = model.CurrentVolunteer.Id;

            var transactionType = _transTypes.GetTransactionTypeById(model.CreditTransaction.TransactionType.Id);

            if (transactionType.MakeNegative.IsChecked)
                model.CreditTransaction.CreditAmount = model.CreditTransaction.CreditAmount * -1;

            _volTransactions.CreateTransaction(model.CreditTransaction);
            _vols.UpdateCreditBalance(model.CurrentVolunteer.Id);

            Session["CurrentMessage"] = UI.Tools.Messages.CreateMessage("Transaction Saved!", "The transaction has been successfully saved.",
                Models.MessageConstants.Success);

            return RedirectToAction("VolunteerDetails", "VolMgmt", new { Area = "OrgAdmins", volunteerId = model.CurrentVolunteer.Id });
        }

        [HttpPost]
        public ActionResult CreateTimePunch(VolMgmtViewModel model)
        {
            var volOpps = _volOpps.GetVolunteerOpportunityById(model.CurrentVolPunch.VolunteerOpportunityId);

            model.CurrentVolPunch.VolunteerId = model.CurrentVolunteer.Id;

            model.CurrentVolPunch = _punches.CreateTimePunch(model.CurrentVolPunch, volOpps.CreditValue);

            _vols.UpdateCreditBalance(model.CurrentVolPunch.VolunteerId);

            Session["CurrentMessage"] = UI.Tools.Messages.CreateMessage("Time Punch Saved!", "The time punch has been successfully saved.",
                Models.MessageConstants.Success);

            return RedirectToAction("VolunteerDetails", "VolMgmt", new { Area = "OrgAdmins", volunteerId = model.CurrentVolPunch.VolunteerId });
        }

        public ActionResult EditVolunteerDetails(Guid volunteerId)
        {
            VolMgmtViewModel model = new VolMgmtViewModel();

            model.CurrentVolunteer = _vols.GetVolunteerById(volunteerId);

            return PartialView("_EditVolunteerDetails", model);
        }

        [HttpPost]
        public ActionResult UpdateVolunteerDetails(VolMgmtViewModel model)
        {
            _vols.UpdateVolunteerDetails(model.CurrentVolunteer);

            model.CurrentVolunteer = _vols.GetVolunteerById(model.CurrentVolunteer.Id);

            Session["CurrentMessage"] = UI.Tools.Messages.CreateMessage("Volunteer Info Updated!", model.CurrentVolunteer.FullName + "'s information has been updated successfully.",
                Models.MessageConstants.Success);

            return RedirectToAction("VolunteerDetails", new { volunteerId = model.CurrentVolunteer.Id });
        }
    }
}