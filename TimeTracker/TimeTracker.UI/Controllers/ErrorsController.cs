using System;
using System.Web.Mvc;
using TimeTracker.UI.Tools;

namespace TimeTracker.UI.Controllers
{
    public class ErrorsController : Controller
    {
        // GET: Errors
        public ActionResult Index(string errorMessage = "")
        {
            if (!string.IsNullOrEmpty(errorMessage))
                ViewData.Add("ErrorMessage", errorMessage);

            Helpers.CreateLogFolder();
            Helpers.CreateLogFile();

            Helpers.WriteLine("An error occurred: " + errorMessage + " on " + DateTime.Now.ToLongDateString() + ".");

            return View("Error");
        }

        public ActionResult PageNotFound()
        {
            return View();
        }

    }
}