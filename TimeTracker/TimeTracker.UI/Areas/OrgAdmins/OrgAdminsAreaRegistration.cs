using System.Web.Mvc;

namespace TimeTracker.UI.Areas.OrgAdmins
{
    public class OrgAdminsAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "OrgAdmins";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "OrgAdmins_default",
                "OrgAdmins/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}