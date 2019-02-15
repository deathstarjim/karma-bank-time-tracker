using System;
using System.Linq;
using System.Web.Mvc;
using TimeTracker.Core.Contracts;
using TimeTracker.Core.Models;
using TimeTracker.UI.Areas.OrgAdmins.ViewModels;
using TimeTracker.UI.Models;

namespace TimeTracker.UI.Areas.OrgAdmins.Controllers
{
    [Authorize]
    public class AdministratorsController : Controller
    {
        private ISecurity _security;
        private IAdministrator _admins;
        private IAdminTimePunch _adminTimePunch;
        private ISystemRole _roles;
        private IAdminTransaction _adminTransactions;
        private ITransactionType _transTypes;

        public AdministratorsController(ISecurity security, IAdministrator admins, ISystemRole roles, IAdminTimePunch adminTimePunch, 
            IAdminTransaction adminTransactions, ITransactionType transTypes)
        {
            _admins = admins;
            _security = security;
            _roles = roles;
            _adminTimePunch = adminTimePunch;
            _adminTransactions = adminTransactions;
            _transTypes = transTypes;
        }

        public ActionResult Index()
        {
            try
            {
                if (Tools.OrgAdminTools.CheckAdminLoggedOut())
                    return RedirectToAction("Index", "Login", new { Area = "OrgAdmins" });

                AdministratorsViewModel model = new AdministratorsViewModel();

                model.CurrentAdministrator = Tools.OrgAdminTools.GetCurrentAdmin(_admins.GetAdministrators());
                model.Administrators = _admins.GetAdministrators();
                model.OpenPunches = _adminTimePunch.OpenTimePunches();

                return View(model);

            }
            catch (Exception ex)
            {
                Error error = new Error { Message = ex.Message, InnerException = ex.InnerException.Message, ControllerName = "Administrators", ActionName = "Index" };
                return RedirectToAction("Index", "Errors", new { Area = "", error });
            }
        }

        public ActionResult AdministratorDetails(Guid administratorId)
        {
            try
            {
                if (Tools.OrgAdminTools.CheckAdminLoggedOut())
                    return RedirectToAction("Index", "Login", new { Area = "OrgAdmins" });

                AdministratorsViewModel model = new AdministratorsViewModel();

                model.CurrentAdministrator = Tools.OrgAdminTools.GetCurrentAdmin(_admins.GetAdministrators());
                model.SelectedAdministrator = _admins.GetAdministratorById(administratorId);
                model.AdminTimePunches = _adminTimePunch.GetTimePunchesByAdminId(model.SelectedAdministrator.Id);
                model.TransactionTypes = _transTypes.GetTransactionTypes();
                model.Transactions = _adminTransactions.GetTransactionsByAdminId(model.SelectedAdministrator.Id);

                return View(model);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "Errors", new { Area = "", errorMessage = ex.Message + " - " + ex.InnerException });
            }
        }

        public ActionResult EditAdminInfo(Guid administratorId)
        {
            try
            {
                if (Tools.OrgAdminTools.CheckAdminLoggedOut())
                    return RedirectToAction("Index", "Login", new { Area = "OrgAdmins" });

                AdministratorsViewModel model = new AdministratorsViewModel();
                model.CurrentAdministrator = Tools.OrgAdminTools.GetCurrentAdmin(_admins.GetAdministrators());
                model.SelectedAdministrator = _admins.GetAdministratorById(administratorId);

                return PartialView("_EditAdminInfo", model);

            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "Errors", new { Area = "", errorMessage = ex.Message + " - " + ex.InnerException });
            }
        }

        [HttpPost]
        public ActionResult EditAdminInfo(AdministratorsViewModel model)
        {
            _admins.UpdateAdministratorInformation(model.SelectedAdministrator);

            Session["CurrentMessage"] = UI.Tools.Messages.CreateMessage("Administrator Updated!", 
                model.SelectedAdministrator.FullName + " has been updated.", Models.MessageConstants.Success);

            return RedirectToAction("Index");
        }

        public ActionResult UpdateAdminPassword(Guid administratorId)
        {
            try
            {
                if (Tools.OrgAdminTools.CheckAdminLoggedOut())
                    return RedirectToAction("Index", "Login", new { Area = "OrgAdmins" });

                AdministratorsViewModel model = new AdministratorsViewModel();
                model.CurrentAdministrator = Tools.OrgAdminTools.GetCurrentAdmin(_admins.GetAdministrators());
                model.SelectedAdministrator = _admins.GetAdministratorById(administratorId);

                return PartialView("_UpdateAdminPassword", model);

            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "Errors", new { Area = "", errorMessage = ex.Message + " - " + ex.InnerException });
            }
        }

        [HttpPost]
        public ActionResult UpdateAdminPassword(AdministratorsViewModel model)
        {
            try
            {
                string salt = _security.GenerateSalt();
                string securedPassword = _security.HashPassword(model.SelectedAdministrator.Password, salt);

                model.SelectedAdministrator.Password = securedPassword;
                model.SelectedAdministrator.PasswordSalt = salt;

                _admins.UpdateAdministratorPassword(model.SelectedAdministrator);

                Session["CurrentMessage"] = UI.Tools.Messages.CreateMessage("Administrator Updated!",
                    model.SelectedAdministrator.FullName + "'s password has been updated.", Models.MessageConstants.Success);

                return RedirectToAction("Index");

            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "Errors", new { Area = "", errorMessage = ex.Message + " - " + ex.InnerException });
            }
        }

        [HttpPost]
        public ActionResult CreditTransaction(AdministratorsViewModel model)
        {
            try
            {
                model.CreditTransaction.AdministratorId = model.SelectedAdministrator.Id;

                var transactionType = _transTypes.GetTransactionTypeById(model.CreditTransaction.TransactionType.Id);

                if (transactionType.MakeNegative.IsChecked)
                    model.CreditTransaction.CreditAmount = model.CreditTransaction.CreditAmount * -1;

                _adminTransactions.CreateTransaction(model.CreditTransaction);
                _admins.UpdateCreditBalance(model.SelectedAdministrator.Id);

                Session["CurrentMessage"] = UI.Tools.Messages.CreateMessage("Transaction Saved!", "The credit transaction has been successfully saved.",
                    Models.MessageConstants.Success);

                return RedirectToAction("AdministratorDetails", "Administrators", new { Area = "OrgAdmins", administratorId = model.SelectedAdministrator.Id });

            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "Errors", new { Area = "", errorMessage = ex.Message + " - " + ex.InnerException });
            }
        }

        public ActionResult EditPunch(Guid punchId)
        {
            try
            {
                if (Tools.OrgAdminTools.CheckAdminLoggedOut())
                    return RedirectToAction("Index", "Login", new { Area = "OrgAdmins" });

                AdministratorsViewModel model = new AdministratorsViewModel();

                model.CurrentAdministrator = Tools.OrgAdminTools.GetCurrentAdmin(_admins.GetAdministrators());
                model.CurrentTimePunch = _adminTimePunch.GetTimePunchById(punchId);

                return PartialView("_EditAdminPunch", model);

            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "Errors", new { Area = "", errorMessage = ex.Message + " - " + ex.InnerException });
            }
        }

        [HttpPost]
        public ActionResult UpdatePunch(AdministratorsViewModel model)
        {
            try
            {
                _adminTimePunch.UpdateTimePunch(model.CurrentTimePunch);
                _admins.UpdateCreditBalance(model.CurrentTimePunch.AdministratorId);

                Session["CurrentMessage"] = UI.Tools.Messages.CreateMessage("Time Punch Updated!", "The time punch has been successfully updated.",
                    Models.MessageConstants.Success);

                return RedirectToAction("AdministratorDetails", "Administrators", new { administratorId = model.CurrentTimePunch.AdministratorId });

            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "Errors", new { Area = "", errorMessage = ex.Message + " - " + ex.InnerException });
            }
        }

        public ActionResult EditAdminTransaction(Guid transactionId)
        {
            try
            {
                AdministratorsViewModel model = new AdministratorsViewModel();

                model.CreditTransaction = _adminTransactions.GetTransactionById(transactionId);
                model.SelectedTransactionType = model.CreditTransaction.TransactionType.Id;
                model.TransactionTypes = _transTypes.GetTransactionTypes();

                return PartialView("_EditAdminTransaction", model);

            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "Errors", new { Area = "", errorMessage = ex.Message + " - " + ex.InnerException });
            }
        }

        [HttpPost]
        public ActionResult UpdateAdminTransaction(AdministratorsViewModel model)
        {
            try
            {
                var transactionType = _transTypes.GetTransactionTypeById(model.SelectedTransactionType);

                if (transactionType != null && transactionType.Id != Guid.Empty)
                {
                    //If changing from transaction type that gains credits to a purchase, make credit amount negative
                    if (transactionType.MakeNegative.IsChecked && model.CreditTransaction.CreditAmount > 0)
                        model.CreditTransaction.CreditAmount = model.CreditTransaction.CreditAmount * -1;

                    //If changing transaction type from purchase to something else where credits are gained
                    if (!transactionType.MakeNegative.IsChecked && model.CreditTransaction.CreditAmount < 0)
                        model.CreditTransaction.CreditAmount = model.CreditTransaction.CreditAmount * -1;

                    model.CreditTransaction.TransactionType.Id = transactionType.Id;

                    _adminTransactions.UpdateTransaction(model.CreditTransaction);
                    _admins.UpdateCreditBalance(model.CreditTransaction.AdministratorId);
                }

                Session["CurrentMessage"] = UI.Tools.Messages.CreateMessage("Transaction Updated!", "The transaction has been successfully updated.",
                    Models.MessageConstants.Success);

                return RedirectToAction("AdministratorDetails", "Administrators", new { administratorId = model.CreditTransaction.AdministratorId });

            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "Errors", new { Area = "", errorMessage = ex.Message + " - " + ex.InnerException });
            }
        }

        public ActionResult DeleteAdminTimePunch(Guid timePunchId, Guid administratorId)
        {
            try
            {
                _adminTimePunch.DeleteTimePunch(timePunchId, administratorId);
                _admins.UpdateCreditBalance(administratorId);

                Session["CurrentMessage"] = UI.Tools.Messages.CreateMessage("Time Punch Deleted!", "The time punch has been successfully deleted.",
                    Models.MessageConstants.Success);

                return RedirectToAction("AdministratorDetails", "Administrators", new { administratorId });

            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "Errors", new { Area = "", errorMessage = ex.Message + " - " + ex.InnerException });
            }
        }

        public ActionResult DeleteAdminTransaction(Guid transactionId, Guid administratorId)
        {
            try
            {
                _adminTransactions.DeleteTransaction(transactionId);
                _admins.UpdateCreditBalance(administratorId);

                Session["CurrentMessage"] = UI.Tools.Messages.CreateMessage("Transaction Deleted!", "The transaction has been successfully deleted.",
                    Models.MessageConstants.Success);

                return RedirectToAction("AdministratorDetails", "Administrators", new { administratorId });

            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "Errors", new { Area = "", errorMessage = ex.Message + " - " + ex.InnerException });
            }
        }

        public ActionResult ClockAdminIn(Guid administratorId)
        {
            try
            {
                bool adminClockedIn = _adminTimePunch.CheckAdminClockedIn(administratorId);

                if (adminClockedIn)
                    Session["CurrentMessage"] = UI.Tools.Messages.CreateMessage("Whoops!", "You are already clocked in!",
                        Models.MessageConstants.Error);

                if (!adminClockedIn)
                {
                    _adminTimePunch.PunchAdminIn(new AdminTimePunch
                    {
                        AdministratorId = administratorId,
                        PunchInDateTime = DateTime.Now
                    });

                    Session["CurrentMessage"] = UI.Tools.Messages.CreateMessage("Clocked In!", "You are now clocked in!",
                        Models.MessageConstants.Success);
                }

                return RedirectToAction("Index", "Administrators", new { Area = "OrgAdmins" });

            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "Errors", new { Area = "", errorMessage = ex.Message + " - " + ex.InnerException });
            }
        }

        public ActionResult ClockAdminOut(Guid punchId)
        {
            try
            {
                var punch = _adminTimePunch.GetTimePunchById(punchId);

                if (punch != null)
                {
                    punch.PunchOutDateTime = DateTime.Now;
                    _adminTimePunch.PunchAdminOut(punch);
                    _admins.UpdateCreditBalance(punch.AdministratorId);
                }

                Session["CurrentMessage"] = UI.Tools.Messages.CreateMessage("Clocked Out!", "You are now clocked out!",
                    Models.MessageConstants.Success);

                return RedirectToAction("Index", "Administrators", new { Area = "OrgAdmins" });

            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "Errors", new { Area = "", errorMessage = ex.Message + " - " + ex.InnerException });
            }
        }

        public ActionResult AdministratorRegistration()
        {
            try
            {
                if (Tools.OrgAdminTools.CheckAdminLoggedOut())
                    return RedirectToAction("Index", "Login", new { Area = "OrgAdmins" });

                AdministratorsViewModel model = new AdministratorsViewModel();
                model.CurrentAdministrator = Tools.OrgAdminTools.GetCurrentAdmin(_admins.GetAdministrators());

                return View(model);

            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "Errors", new { Area = "", errorMessage = ex.Message + " - " + ex.InnerException });
            }
        }

        [HttpPost]
        public ActionResult AdministratorRegistration(AdministratorsViewModel model)
        {
            try
            {
                string salt = _security.GenerateSalt();
                string securedPassword = _security.HashPassword(model.NewAdministrator.Password, salt);

                model.NewAdministrator.Password = securedPassword;
                model.NewAdministrator.PasswordSalt = salt;

                var roles = _roles.GetSystemRoles();

                model.NewAdministrator.Role = roles.Where(r => r.Name == "Organizational Administrator").FirstOrDefault();

                model.NewAdministrator = _admins.CreateAdministrator(model.NewAdministrator);

                model.NewAdministrator = new Administrator();

                Session["CurrentMessage"] = UI.Tools.Messages.CreateMessage("Administrator Registered!", "You have been successfully registered.", Models.MessageConstants.Success);

                ModelState.Clear();

                return View(model);

            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "Errors", new { Area = "", errorMessage = ex.Message + " - " + ex.InnerException });
            }
        }
    }
}