using System;
using System.ComponentModel;
using TimeTracker.Core.BaseModels;

namespace TimeTracker.Core.Models
{
    public class VolTimePunch : BasePunch
    {
        public Guid VolunteerId { get; set; }

        [DisplayName("Select an Opportunity:")]
        public Guid VolunteerOpportunityId { get; set; }

        public string VolunteerOpportunityName { get; set; }

        public string VolunteerFullName { get; set; }

    }
}
