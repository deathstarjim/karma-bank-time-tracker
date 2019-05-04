using System;
using System.Linq;
using System.Web.Mvc;
using TimeTracker.Core.Contracts;
using TimeTracker.UI.Areas.OrgAdmins.ViewModels;
using TimeTracker.UI.Models;

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
            try
            {
                if (Tools.OrgAdminTools.CheckAdminLoggedOut())
                    return RedirectToAction("Index", "Login", new { Area = "OrgAdmins" });

                VolMgmtViewModel model = new VolMgmtViewModel();

                model.Volunteers = _vols.GetVolunteers();
                model.CurrentAdministrator = Tools.OrgAdminTools.GetCurrentAdmin(_admins.GetAdministrators());

                return View(model);

            }
            catch (Exception ex)
            {
                Error error = new Error
                {
                    Message = ex.Message,
                    InnerException = (ex.InnerException != null) ? ex.InnerException.Message : "",
                    ControllerName = "VolMgmt",
                    ActionName = "Index"
                };

                TempData["Error"] = error;

                return RedirectToAction("Index", "Errors", new { Area = "" });
            }
        }

        public ActionResult VolunteerDetails(Guid volunteerId)
        {
            try
            {
                if (Tools.OrgAdminTools.CheckAdminLoggedOut())
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
            catch (Exception ex)
            {
                Error error = new Error
                {
                    Message = ex.Message,
                    InnerException = (ex.InnerException != null) ? ex.InnerException.Message : "",
                    ControllerName = "VolMgmt",
                    ActionName = "VolunteerDetails"
                };

                TempData["Error"] = error;

                return RedirectToAction("Index", "Errors", new { Area = "" });
            }
        }

        public ActionResult EditPunch(Guid punchId)
        {
            try
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
            catch (Exception ex)
            {
                Error error = new Error
                {
                    Message = ex.Message,
                    InnerException = (ex.InnerException != null) ? ex.InnerException.Message : "",
                    ControllerName = "VolMgmt",
                    ActionName = "EditPunch"
                };

                TempData["Error"] = error;

                return RedirectToAction("Index", "Errors", new { Area = "" });
            }
        }

        [HttpPost]
        public ActionResult UpdatePunch(VolMgmtViewModel model)
        {
            try
            {
                var volOpp = _volOpps.GetVolunteerOpportunityById(model.SelectedVolunteerOpportunity);
                var currentVolunteer = _vols.GetVolunteerById(model.CurrentVolPunch.VolunteerId);

                if (volOpp != null)
                {
                    model.CurrentVolPunch.VolunteerOpportunityId = model.SelectedVolunteerOpportunity;
                    _punches.UpdateTimePunch(model.CurrentVolPunch, volOpp.CreditValue);

                    if (currentVolunteer != null && currentVolunteer.Id != Guid.Empty)
                        _vols.UpdateCreditBalance(currentVolunteer.Id);

                    Session["CurrentMessage"] = UI.Tools.Messages.CreateMessage("Time Punch Updated!", "The time punch record has been successfully updated.",
                        MessageConstants.Success);
                }

                return RedirectToAction("VolunteerDetails", "VolMgmt", new { Area = "OrgAdmins", volunteerId = model.CurrentVolPunch.VolunteerId });

            }
            catch (Exception ex)
            {
                Error error = new Error
                {
                    Message = ex.Message,
                    InnerException = (ex.InnerException != null) ? ex.InnerException.Message : "",
                    ControllerName = "VolMgmt",
                    ActionName = "UpdatePunch"
                };

                TempData["Error"] = error;

                return RedirectToAction("Index", "Errors", new { Area = "" });
            }

        }

        public ActionResult DeletePunch(Guid timePunchId, Guid volunteerId)
        {
            try
            {
                _punches.DeleteTimePunch(timePunchId);
                _vols.UpdateCreditBalance(volunteerId);

                Session["CurrentMessage"] = UI.Tools.Messages.CreateMessage("Time Punch Deleted!", "The time punch record has been successfully deleted.",
                    MessageConstants.Success);

                return RedirectToAction("VolunteerDetails", "VolMgmt", new { Area = "OrgAdmins", volunteerId = volunteerId });

            }
            catch (Exception ex)
            {
                Error error = new Error
                {
                    Message = ex.Message,
                    InnerException = (ex.InnerException != null) ? ex.InnerException.Message : "",
                    ControllerName = "VolMgmt",
                    ActionName = "DeletePunch"
                };

                TempData["Error"] = error;

                return RedirectToAction("Index", "Errors", new { Area = "" });
            }
        }

        public ActionResult EditTransaction(Guid transactionId)
        {
            try
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
            catch (Exception ex)
            {
                Error error = new Error
                {
                    Message = ex.Message,
                    InnerException = (ex.InnerException != null) ? ex.InnerException.Message : "",
                    ControllerName = "VolMgmt",
                    ActionName = "EditTransaction"
                };

                TempData["Error"] = error;

                return RedirectToAction("Index", "Errors", new { Area = "" });
            }
        }

        [HttpPost]
        public ActionResult UpdateTransaction(VolMgmtViewModel model)
        {
            try
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
                    MessageConstants.Success);

                return RedirectToAction("VolunteerDetails", "VolMgmt", new { Area = "OrgAdmins", volunteerId = model.CreditTransaction.VolunteerId });

            }
            catch (Exception ex)
            {
                Error error = new Error
                {
                    Message = ex.Message,
                    InnerException = (ex.InnerException != null) ? ex.InnerException.Message : "",
                    ControllerName = "VolMgmt",
                    ActionName = "UpdateTransaction"
                };

                TempData["Error"] = error;

                return RedirectToAction("Index", "Errors", new { Area = "" });
            }
        }

        public ActionResult DeleteTransaction(Guid transactionId, Guid volunteerId)
        {
            try
            {
                _volTransactions.DeleteTransaction(transactionId);
                _vols.UpdateCreditBalance(volunteerId);

                Session["CurrentMessage"] = UI.Tools.Messages.CreateMessage("Transaction Deleted!", "The transaction record has been successfully deleted.",
                    MessageConstants.Success);


                return RedirectToAction("VolunteerDetails", "VolMgmt", new { Area = "OrgAdmins", volunteerId = volunteerId });

            }
            catch (Exception ex)
            {
                Error error = new Error
                {
                    Message = ex.Message,
                    InnerException = (ex.InnerException != null) ? ex.InnerException.Message : "",
                    ControllerName = "VolMgmt",
                    ActionName = "DeleteTransaction"
                };

                TempData["Error"] = error;

                return RedirectToAction("Index", "Errors", new { Area = "" });
            }
        }

        [HttpPost]
        public ActionResult CreditTransaction(VolMgmtViewModel model)
        {
            try
            {
                model.CreditTransaction.VolunteerId = model.CurrentVolunteer.Id;

                var transactionType = _transTypes.GetTransactionTypeById(model.CreditTransaction.TransactionType.Id);

                if (transactionType.MakeNegative.IsChecked)
                    model.CreditTransaction.CreditAmount = model.CreditTransaction.CreditAmount * -1;

                _volTransactions.CreateTransaction(model.CreditTransaction);
                _vols.UpdateCreditBalance(model.CurrentVolunteer.Id);

                Session["CurrentMessage"] = UI.Tools.Messages.CreateMessage("Transaction Saved!", "The transaction has been successfully saved.",
                    MessageConstants.Success);

                return RedirectToAction("VolunteerDetails", "VolMgmt", new { Area = "OrgAdmins", volunteerId = model.CurrentVolunteer.Id });

            }
            catch (Exception ex)
            {
                Error error = new Error
                {
                    Message = ex.Message,
                    InnerException = (ex.InnerException != null) ? ex.InnerException.Message : "",
                    ControllerName = "VolMgmt",
                    ActionName = "CreditTransaction - Post"
                };

                TempData["Error"] = error;

                return RedirectToAction("Index", "Errors", new { Area = "" });
            }
        }

        [HttpPost]
        public ActionResult CreateTimePunch(VolMgmtViewModel model)
        {
            try
            {
                var volOpps = _volOpps.GetVolunteerOpportunityById(model.CurrentVolPunch.VolunteerOpportunityId);

                model.CurrentVolPunch.VolunteerId = model.CurrentVolunteer.Id;

                model.CurrentVolPunch = _punches.CreateTimePunch(model.CurrentVolPunch, volOpps.CreditValue);

                _vols.UpdateCreditBalance(model.CurrentVolPunch.VolunteerId);

                Session["CurrentMessage"] = UI.Tools.Messages.CreateMessage("Time Punch Saved!", "The time punch has been successfully saved.",
                    MessageConstants.Success);

                return RedirectToAction("VolunteerDetails", "VolMgmt", new { Area = "OrgAdmins", volunteerId = model.CurrentVolPunch.VolunteerId });

            }
            catch (Exception ex)
            {
                Error error = new Error
                {
                    Message = ex.Message,
                    InnerException = (ex.InnerException != null) ? ex.InnerException.Message : "",
                    ControllerName = "VolMgmt",
                    ActionName = "CreateTimePunch"
                };

                TempData["Error"] = error;

                return RedirectToAction("Index", "Errors", new { Area = "" });
            }
        }

        public ActionResult EditVolunteerDetails(Guid volunteerId)
        {
            try
            {
                VolMgmtViewModel model = new VolMgmtViewModel();

                model.CurrentVolunteer = _vols.GetVolunteerById(volunteerId);

                return PartialView("_EditVolunteerDetails", model);

            }
            catch (Exception ex)
            {
                Error error = new Error
                {
                    Message = ex.Message,
                    InnerException = (ex.InnerException != null) ? ex.InnerException.Message : "",
                    ControllerName = "VolMgmt",
                    ActionName = "EditVolunteerDetails"
                };

                TempData["Error"] = error;

                return RedirectToAction("Index", "Errors", new { Area = "" });
            }
        }

        [HttpPost]
        public ActionResult UpdateVolunteerDetails(VolMgmtViewModel model)
        {
            try
            {
                _vols.UpdateVolunteerDetails(model.CurrentVolunteer);

                model.CurrentVolunteer = _vols.GetVolunteerById(model.CurrentVolunteer.Id);

                Session["CurrentMessage"] = UI.Tools.Messages.CreateMessage("Volunteer Info Updated!", model.CurrentVolunteer.FullName + "'s information has been updated successfully.",
                    MessageConstants.Success);

                return RedirectToAction("VolunteerDetails", new { volunteerId = model.CurrentVolunteer.Id });

            }
            catch (Exception ex)
            {
                Error error = new Error
                {
                    Message = ex.Message,
                    InnerException = (ex.InnerException != null) ? ex.InnerException.Message : "",
                    ControllerName = "VolMgmt",
                    ActionName = "UpdateVolunteerDetails"
                };

                TempData["Error"] = error;

                return RedirectToAction("Index", "Errors", new { Area = "" });
            }
        }
    }
}