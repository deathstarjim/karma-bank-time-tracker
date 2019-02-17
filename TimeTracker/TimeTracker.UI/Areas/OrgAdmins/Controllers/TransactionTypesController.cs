using System;
using System.Web.Mvc;
using TimeTracker.Core.Contracts;
using TimeTracker.UI.Areas.OrgAdmins.ViewModels;
using TimeTracker.UI.Models;

namespace TimeTracker.UI.Areas.OrgAdmins.Controllers
{
    public class TransactionTypesController : Controller
    {
        private ITransactionType _transTypes;
        private IAdministrator _admins;

        public TransactionTypesController(ITransactionType transTypes, IAdministrator admins)
        {
            _transTypes = transTypes;
            _admins = admins;
        }

        public ActionResult Index()
        {
            try
            {
                if (Tools.OrgAdminTools.CheckAdminLoggedOut())
                    return RedirectToAction("Index", "Login", new { Area = "OrgAdmins" });

                TransactionTypesViewModel model = new TransactionTypesViewModel();

                model.CurrentAdministrator = Tools.OrgAdminTools.GetCurrentAdmin(_admins.GetAdministrators());
                model.TransactionTypes = _transTypes.GetTransactionTypes();
                model.ReplacementTransactionTypes = _transTypes.GetTransactionTypes();

                return View(model);

            }
            catch (Exception ex)
            {
                Error error = new Error
                {
                    Message = ex.Message,
                    InnerException = (ex.InnerException != null) ? ex.InnerException.Message : "",
                    ControllerName = "TransactionTypes",
                    ActionName = "Index"
                };

                TempData["Error"] = error;

                return RedirectToAction("Index", "Errors", new { Area = "" });
            }
        }

        [HttpPost]
        public ActionResult CreateTransactionType(TransactionTypesViewModel model)
        {
            try
            {
                _transTypes.CreateTransactionType(model.CurrentTransactionType);

                Session["CurrentMessage"] = UI.Tools.Messages.CreateMessage("Transaction Type Saved!", "The transaction type has been successfully saved.",
                    MessageConstants.Success);

                return RedirectToAction("Index", "TransactionTypes");

            }
            catch (Exception ex)
            {
                Error error = new Error
                {
                    Message = ex.Message,
                    InnerException = (ex.InnerException != null) ? ex.InnerException.Message : "",
                    ControllerName = "TransactionTypes",
                    ActionName = "CreateTransactionType"
                };

                TempData["Error"] = error;

                return RedirectToAction("Index", "Errors", new { Area = "" });
            }
        }

        public ActionResult EditTransactionType(Guid transactionTypeId)
        {
            try
            {
                if (Tools.OrgAdminTools.CheckAdminLoggedOut())
                    return RedirectToAction("Index", "Login", new { Area = "OrgAdmins" });

                TransactionTypesViewModel model = new TransactionTypesViewModel();
                model.CurrentAdministrator = Tools.OrgAdminTools.GetCurrentAdmin(_admins.GetAdministrators());

                model.CurrentTransactionType = _transTypes.GetTransactionTypeById(transactionTypeId);

                return PartialView("_EditTransactionType", model);

            }
            catch (Exception ex)
            {
                Error error = new Error
                {
                    Message = ex.Message,
                    InnerException = (ex.InnerException != null) ? ex.InnerException.Message : "",
                    ControllerName = "TransactionTypes",
                    ActionName = "EditTransactionType"
                };

                TempData["Error"] = error;

                return RedirectToAction("Index", "Errors", new { Area = "" });
            }
        }

        [HttpPost]
        public ActionResult UpdateTransactionType(TransactionTypesViewModel model)
        {
            try
            {
                _transTypes.UpdateTransactionType(model.CurrentTransactionType);

                Session["CurrentMessage"] = UI.Tools.Messages.CreateMessage("Transaction Type Updated!", "The transaction type has been successfully updated.",
                    MessageConstants.Success);

                return RedirectToAction("Index", "TransactionTypes");

            }
            catch (Exception ex)
            {
                Error error = new Error
                {
                    Message = ex.Message,
                    InnerException = (ex.InnerException != null) ? ex.InnerException.Message : "",
                    ControllerName = "TransactionTypes",
                    ActionName = "UpdateTransactionType"
                };

                TempData["Error"] = error;

                return RedirectToAction("Index", "Errors", new { Area = "" });
            }
        }

        [HttpPost]
        public ActionResult DeleteTransactionType(TransactionTypesViewModel model)
        {
            try
            {
                _transTypes.UpdateVolunteerTransactionTypes(model.ReplacementTransactionType, model.SelectedTransactionType);
                _transTypes.UpdateOrgAdminTransactionTypes(model.ReplacementTransactionType, model.SelectedTransactionType);

                _transTypes.DeleteTransactionType(model.SelectedTransactionType);

                Session["CurrentMessage"] = UI.Tools.Messages.CreateMessage("Transaction Type Deleted!", "The transaction type has been successfully deleted.",
                    MessageConstants.Success);

                return RedirectToAction("Index", "TransactionTypes");

            }
            catch (Exception ex)
            {
                Error error = new Error
                {
                    Message = ex.Message,
                    InnerException = (ex.InnerException != null) ? ex.InnerException.Message : "",
                    ControllerName = "TransactionTypes",
                    ActionName = "DeleteTransactionType"
                };

                TempData["Error"] = error;

                return RedirectToAction("Index", "Errors", new { Area = "" });
            }
        }
    }
}