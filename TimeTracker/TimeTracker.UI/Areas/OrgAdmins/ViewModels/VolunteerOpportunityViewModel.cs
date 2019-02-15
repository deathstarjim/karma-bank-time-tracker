using System.Collections.Generic;
using TimeTracker.Core.Models;

namespace TimeTracker.UI.Areas.OrgAdmins.ViewModels
{
    public class VolunteerOpportunityViewModel : BaseOrgAdminViewModel
    {
        public VolunteerOpportunityViewModel()
        {
            VolunteerOpportunities = new List<VolunteerOpportunity>();
            CurrentOpportunity = new VolunteerOpportunity();
        }

        public List<VolunteerOpportunity> VolunteerOpportunities { get; set; }

        public VolunteerOpportunity CurrentOpportunity { get; set; }

        public bool IsValid { get; set; }

        /// <summary>
        /// Validation message
        /// </summary>
        public string Message { get; set; }

    }
}