using System;
using System.Web.Mvc;
using TimeTracker.UI.Models;
using TimeTracker.UI.Tools;

namespace TimeTracker.UI.Controllers
{
    public class ErrorsController : Controller
    {
        // GET: Errors
        public ActionResult Index()
        {
            Helpers.CreateLogFolder();
            Helpers.CreateLogFile();

            if(TempData["Error"] != null)
            {
                DateTime errorTime = DateTime.Now;

                Error error = (Error)TempData["Error"];
                Helpers.WriteLine("*******************");
                Helpers.WriteLine("An error occurred: on " + errorTime.ToLongDateString() + " at " + errorTime.ToLongTimeString() + ".");
                Helpers.WriteLine("Error Message: " + error.Message);
                Helpers.WriteLine("Inner Exception: " + error.InnerException);
                Helpers.WriteLine("Controller Name: " + error.ControllerName);
                Helpers.WriteLine("Action Name: " + error.ActionName);
                Helpers.WriteLine("*******************");
            }

            return View("Error");
        }

        public ActionResult PageNotFound()
        {
            return View();
        }

    }
}