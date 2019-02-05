using TimeTracker.Core.Models;

namespace TimeTracker.UI.Areas.OrgRegistration.ViewModels
{
    public class RegistrationViewModel
    {
        public RegistrationViewModel()
        {
            CurrentOrg = new Organization();
            CurrentAdmin = new Administrator();
        }

        public Organization CurrentOrg { get; set; }

        public Administrator CurrentAdmin { get; set; }
    }
}