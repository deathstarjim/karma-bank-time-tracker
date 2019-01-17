﻿using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using TimeTracker.Core.Contracts;
using TimeTracker.UI.Areas.OrgAdmins.ViewModels;

namespace TimeTracker.UI.Areas.OrgAdmins.Controllers
{
    public class LoginController : Controller
    {
        private ILogin _login;
        private ISecurity _security;
        private IAdministrator _admins;

        public LoginController(ILogin login, ISecurity security, IAdministrator admins)
        {
            _login = login;
            _security = security;
            _admins = admins;
        }

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult LogUserIn(LoginViewModel model)
        {
            var administrator = _admins.GetAdministratorByUserName(model.CurrentAdmin.UserName);
            string hashedPassword = "";

            if (administrator.Id != Guid.Empty)
            {
                hashedPassword = administrator.Password;

                var userIsValid = _security.PasswordIsValid(model.CurrentAdmin.Password, hashedPassword);

                if (userIsValid)
                {
                    var currentEmployee = _login.LogAdminIn(model.CurrentAdmin.UserName, hashedPassword);

                    FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(1, model.CurrentAdmin.UserName, DateTime.Now, DateTime.Now.AddMinutes(2880),
                        false, "", FormsAuthentication.FormsCookiePath);

                    string hash = FormsAuthentication.Encrypt(ticket);

                    HttpCookie cookie = new HttpCookie(FormsAuthentication.FormsCookieName, hash);

                    FormsAuthentication.SetAuthCookie(model.CurrentAdmin.UserName, false);

                    Session["CurrentUserId"] = currentEmployee.Id;

                    return RedirectToAction("Index", "Administrators", new { Area = "OrgAdmins" });

                }
            }

            Session["CurrentMessage"] = UI.Tools.Messages.CreateMessage("Login Failed!", "The user name or password you entered was incorrect. Please try again.",
                Models.MessageConstants.Error, 6000);


            return RedirectToAction("Index", "Login", model);
        }

        public ActionResult LogOut()
        {
            FormsAuthentication.SignOut();

            return RedirectToAction("Index", "Home", new { Area = "" });
        }
    }
}