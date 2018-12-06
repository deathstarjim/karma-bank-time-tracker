using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TimeTracker.Core.Models;

namespace TimeTracker.UI.Areas.OrgAdmins.ViewModels
{
    public class LoginViewModel
    {
        public LoginViewModel()
        {
            CurrentAdmin = new Administrator();
        }

        public Administrator CurrentAdmin { get; set; }
    }
}