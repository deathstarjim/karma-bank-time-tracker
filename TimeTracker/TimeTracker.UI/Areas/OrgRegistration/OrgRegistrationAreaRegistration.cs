using System.Web.Mvc;

namespace TimeTracker.UI.Areas.OrgRegistration
{
    public class OrgRegistrationAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "OrgRegistration";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "OrgRegistration_default",
                "OrgRegistration/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}