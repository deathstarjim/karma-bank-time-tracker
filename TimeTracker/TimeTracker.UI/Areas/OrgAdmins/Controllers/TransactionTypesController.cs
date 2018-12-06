using System;
using System.Web.Mvc;
using TimeTracker.Core.Contracts;
using TimeTracker.UI.Areas.OrgAdmins.ViewModels;

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
            if (Tools.OrgAdminTools.CheckAdminLoggedOut())
                return RedirectToAction("Index", "Login", new { Area = "OrgAdmins" });

            TransactionTypesViewModel model = new TransactionTypesViewModel();

            model.CurrentAdministrator = Tools.OrgAdminTools.GetCurrentAdmin(_admins.GetAdministrators());
            model.TransactionTypes = _transTypes.GetTransactionTypes();
            model.ReplacementTransactionTypes = _transTypes.GetTransactionTypes();

            return View(model);
        }

        [HttpPost]
        public ActionResult CreateTransactionType(TransactionTypesViewModel model)
        {
            _transTypes.CreateTransactionType(model.CurrentTransactionType);

            Session["CurrentMessage"] = UI.Tools.Messages.CreateMessage("Transaction Type Saved!", "The transaction type has been successfully saved.",
                Models.MessageConstants.Success);

            return RedirectToAction("Index", "TransactionTypes");
        }

        public ActionResult EditTransactionType(Guid transactionTypeId)
        {
            if (Tools.OrgAdminTools.CheckAdminLoggedOut())
                return RedirectToAction("Index", "Login", new { Area = "OrgAdmins" });

            TransactionTypesViewModel model = new TransactionTypesViewModel();
            model.CurrentAdministrator = Tools.OrgAdminTools.GetCurrentAdmin(_admins.GetAdministrators());

            model.CurrentTransactionType = _transTypes.GetTransactionTypeById(transactionTypeId);

            return PartialView("_EditTransactionType", model);
        }

        [HttpPost]
        public ActionResult UpdateTransactionType(TransactionTypesViewModel model)
        {
            _transTypes.UpdateTransactionType(model.CurrentTransactionType);

            Session["CurrentMessage"] = UI.Tools.Messages.CreateMessage("Transaction Type Updated!", "The transaction type has been successfully updated.",
                Models.MessageConstants.Success);

            return RedirectToAction("Index", "TransactionTypes");
        }

        [HttpPost]
        public ActionResult DeleteTransactionType(TransactionTypesViewModel model)
        {
            _transTypes.UpdateVolunteerTransactionTypes(model.ReplacementTransactionType, model.SelectedTransactionType);
            _transTypes.UpdateOrgAdminTransactionTypes(model.ReplacementTransactionType, model.SelectedTransactionType);

            _transTypes.DeleteTransactionType(model.SelectedTransactionType);

            Session["CurrentMessage"] = UI.Tools.Messages.CreateMessage("Transaction Type Deleted!", "The transaction type has been successfully deleted.",
                Models.MessageConstants.Success);

            return RedirectToAction("Index", "TransactionTypes");
        }
    }
}