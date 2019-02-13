using System;
using System.Collections.Generic;
using TimeTracker.Core.Models;
using TimeTracker.UI.Models;

namespace TimeTracker.UI.Areas.Volunteers.ViewModels
{
    public class VolunteerViewModel
    {
        public VolunteerViewModel()
        {
            Volunteers = new List<Volunteer>();
            CurrentVolunteer = new Volunteer();
            UserMessage = new UserMessage();
            VolunteerOpportunities = new List<VolunteerOpportunity>();
            OpenTimePunches = new List<VolTimePunch>();
            VolunteerTransactions = new List<VolCreditTransaction>();
        }

        public List<Volunteer> Volunteers { get; set; }

        public Volunteer CurrentVolunteer { get; set; }

        public UserMessage UserMessage { get; set; }

        public string SearchTerm { get; set; }

        public List<VolunteerOpportunity> VolunteerOpportunities { get; set; }

        public Guid CurrentOpportunityId { get; set; }

        public List<VolTimePunch> OpenTimePunches { get; set; }

        public List<VolCreditTransaction> VolunteerTransactions { get; set; }

        public string ControllerName { get; set; }

        public string ResultsMessage { get; set; }
    }
}