using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using TimeTracker.Core.Contracts;
using TimeTracker.UI.Areas.OrgAdmins.Tools;
using TimeTracker.UI.Areas.OrgAdmins.ViewModels;
using TimeTracker.UI.Models;

namespace TimeTracker.UI.Areas.OrgAdmins.Controllers
{
    public class ReportsController : Controller
    {
        private IReports _reports;
        private IAdministrator _admins;
        private ExportTools _exports = new ExportTools();

        public ReportsController(IReports reports, IAdministrator admins)
        {
            _reports = reports;
            _admins = admins;
        }

        public ActionResult Index()
        {
            try
            {
                if (OrgAdminTools.CheckAdminLoggedOut())
                    return RedirectToAction("Index", "Login", new { Area = "OrgAdmins" });

                ReportsViewModel model = new ReportsViewModel();
                model.CurrentAdministrator = OrgAdminTools.GetCurrentAdmin(_admins.GetAdministrators());

                return View(model);

            }
            catch (Exception ex)
            {
                Error error = new Error
                {
                    Message = ex.Message,
                    InnerException = (ex.InnerException != null) ? ex.InnerException.Message : "",
                    ControllerName = "Reports",
                    ActionName = "Index"
                };

                TempData["Error"] = error;

                return RedirectToAction("Index", "Errors", new { Area = "" });
            }
        }

        [HttpPost]
        public ActionResult TransactionsByDateRange(ReportsViewModel model)
        {
            try
            {
                var dates = GetDateRange(model.TransactionDateRange);

                model.TransactionResults = _reports.GetTransactionsByDateRange(dates.FirstOrDefault(), dates.LastOrDefault());

                return PartialView("_TransactionResults", model);

            }
            catch (Exception ex)
            {
                Error error = new Error
                {
                    Message = ex.Message,
                    InnerException = (ex.InnerException != null) ? ex.InnerException.Message : "",
                    ControllerName = "Reports",
                    ActionName = "TransactionsByDateRange"
                };

                TempData["Error"] = error;

                return RedirectToAction("Index", "Errors", new { Area = "" });
            }
        }

        public ActionResult TransactionsByDateRangeExcel(string dateRange)
        {
            try
            {
                var dates = GetDateRange(dateRange);

                var transResults = _reports.GetTransactionsByDateRange(dates.FirstOrDefault(), dates.LastOrDefault());

                DataTable results = ToDataTable(transResults);

                SaveExcelExport(results, dates.FirstOrDefault(), dates.LastOrDefault());

                return RedirectToAction("Index", "Reports");

            }
            catch (Exception ex)
            {
                Error error = new Error
                {
                    Message = ex.Message,
                    InnerException = (ex.InnerException != null) ? ex.InnerException.Message : "",
                    ControllerName = "Reports",
                    ActionName = "TransactionsByDateRangeExcel"
                };

                TempData["Error"] = error;

                return RedirectToAction("Index", "Errors", new { Area = "" });
            }
        }

        [HttpPost]
        public ActionResult CreditsByDateRange(ReportsViewModel model)
        {
            try
            {
                var dates = GetDateRange(model.CreditDateRange);

                model.CreditResults = _reports.GetCreditReportByDateRange(dates.FirstOrDefault(), dates.LastOrDefault());

                return PartialView("_CreditResults", model);

            }
            catch (Exception ex)
            {
                Error error = new Error
                {
                    Message = ex.Message,
                    InnerException = (ex.InnerException != null) ? ex.InnerException.Message : "",
                    ControllerName = "Reports",
                    ActionName = "CreditsByDateRange"
                };

                TempData["Error"] = error;

                return RedirectToAction("Index", "Errors", new { Area = "" });
            }
        }

        public ActionResult CreditsByDateRangeExcel(string dateRange)
        {
            try
            {
                var dates = GetDateRange(dateRange);

                var creditResults = _reports.GetCreditReportByDateRange(dates.FirstOrDefault(), dates.LastOrDefault());

                DataTable results = ToDataTable(creditResults);

                SaveExcelExport(results, dates.FirstOrDefault(), dates.LastOrDefault());

                return RedirectToAction("Index", "Reports");

            }
            catch (Exception ex)
            {
                Error error = new Error
                {
                    Message = ex.Message,
                    InnerException = (ex.InnerException != null) ? ex.InnerException.Message : "",
                    ControllerName = "Reports",
                    ActionName = "CreditsByDateRangeExcel"
                };

                TempData["Error"] = error;

                return RedirectToAction("Index", "Errors", new { Area = "" });
            }
        }

        [HttpPost]
        public ActionResult CreditsByDateRangeExcel(ReportsViewModel model)
        {
            try
            {
                var dates = GetDateRange(model.CreditDateRange);
                DateTime startDate = dates.FirstOrDefault();
                DateTime endDate = dates.LastOrDefault();

                model.CreditResults = _reports.GetCreditReportByDateRange(dates.FirstOrDefault(), dates.LastOrDefault());

                DataTable results = new DataTable();

                if (model.CreditResults != null && model.CreditResults.Count > 0)
                    results = ToDataTable(model.CreditResults);

                SaveExcelExport(results, startDate, endDate);

                return RedirectToAction("Index", "Reports");

            }
            catch (Exception ex)
            {
                Error error = new Error
                {
                    Message = ex.Message,
                    InnerException = (ex.InnerException != null) ? ex.InnerException.Message : "",
                    ControllerName = "Reports",
                    ActionName = "CreditsByDateRangeExcel - Post"
                };

                TempData["Error"] = error;

                return RedirectToAction("Index", "Errors", new { Area = "" });
            }
        }

        [HttpPost]
        public ActionResult VolSummaryByDateRange(ReportsViewModel model)
        {
            try
            {
                var dates = GetDateRange(model.VolSummaryDateRange);

                var summaryResults = _reports.VolunteerSummaryByDateRange(dates.FirstOrDefault(), dates.LastOrDefault());

                if (summaryResults != null)
                    SaveExcelExport(summaryResults, dates.FirstOrDefault(), dates.LastOrDefault());

                return RedirectToAction("Index", "Reports");

            }
            catch (Exception ex)
            {
                Error error = new Error
                {
                    Message = ex.Message,
                    InnerException = (ex.InnerException != null) ? ex.InnerException.Message : "",
                    ControllerName = "Reports",
                    ActionName = "VolSummaryByDateRange"
                };

                TempData["Error"] = error;

                return RedirectToAction("Index", "Errors", new { Area = "" });
            }
        }

        private List<DateTime> GetDateRange(string dateRange)
        {
            return dateRange.Split('-').Select(d => Convert.ToDateTime(d)).ToList();
        }

        private void SaveExcelExport(DataTable dataTable, DateTime startDate, DateTime endDate)
        {
            using (XLWorkbook workBook = new XLWorkbook())
            {
                string tabName = string.Format("Results - {0} to {1}", startDate.ToString("MM-dd-yy"), endDate.ToString("MM-dd-yy"));
                string fileName = string.Format("Report_{0}_{1}.xlsx", startDate.ToString("MM-dd-yyyy"), endDate.ToString("MM-dd-yyyy"));

                if (tabName.Length > 31)
                    tabName = tabName.Substring(0, 31);

                workBook.Worksheets.Add(dataTable, tabName);

                _exports.DownloadExcelFile(fileName, workBook, Response);
            }
        }

        private DataTable ToDataTable<T>(List<T> items)
        {
            DataTable dataTable = new DataTable(typeof(T).Name);

            //Get all the properties by using reflection   
            PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            //Setting column names as Property names  
            foreach (PropertyInfo prop in Props)
                dataTable.Columns.Add(prop.Name);

            foreach (T item in items)
            {
                var values = new object[Props.Length];

                for (int i = 0; i < Props.Length; i++)
                    values[i] = Props[i].GetValue(item, null);

                dataTable.Rows.Add(values);
            }

            return dataTable;
        }
    }
}