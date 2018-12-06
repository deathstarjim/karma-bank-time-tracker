using System.Collections.Generic;
using TimeTracker.Core.Models;

namespace TimeTracker.UI.Areas.Volunteers.ViewModels
{
    public class TimePunchViewModel
    {
        public TimePunchViewModel()
        {
            CurrentTimePunch = new VolTimePunch();
            TimePunches = new List<VolTimePunch>();
            Volunteers = new List<Volunteer>();
            VolunteerOpportunities = new List<VolunteerOpportunity>();
        }

        public VolTimePunch CurrentTimePunch { get; set; }

        public List<VolTimePunch> TimePunches { get; set; }

        public List<Volunteer> Volunteers { get; set; }

        public List<VolunteerOpportunity> VolunteerOpportunities { get; set; }
    }
}