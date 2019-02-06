using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TimeTracker.UI.Controllers
{
    public class ErrorsController : Controller
    {
        // GET: Errors
        public ActionResult Index(string errorMessage = "")
        {
            if (!string.IsNullOrEmpty(errorMessage))
                ViewData.Add("ErrorMessage", errorMessage);

            return View("Error");
        }

        public ActionResult PageNotFound()
        {
            return View();
        }

    }
}