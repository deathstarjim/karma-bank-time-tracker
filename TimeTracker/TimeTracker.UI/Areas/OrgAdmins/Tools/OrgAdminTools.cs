using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TimeTracker.Core.Models;

namespace TimeTracker.UI.Areas.OrgAdmins.Tools
{
    public class OrgAdminTools
    {
        /// <summary>
        /// Returns the current administrator based on the session variable with the Administrator Id in it.
        /// </summary>
        /// <returns></returns>
        public static Administrator GetCurrentAdmin(List<Administrator> admins)
        {
            return admins.Where(a => a.Id == new Guid(HttpContext.Current.Session["CurrentUserId"].ToString())).FirstOrDefault();
        }

        /// <summary>
        /// Returns true if the administrator is logged out
        /// </summary>
        /// <returns></returns>
        public static bool CheckAdminLoggedOut()
        {
            return HttpContext.Current.Session["CurrentUserId"] == null;
        }

    }
}