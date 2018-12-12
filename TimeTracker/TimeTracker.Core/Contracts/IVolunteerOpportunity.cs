using System;
using System.Collections.Generic;
using TimeTracker.Core.Models;

namespace TimeTracker.Core.Contracts
{
    public interface IVolunteerOpportunity
    {
        Guid CreateVolunteerOpportunity(VolunteerOpportunity newOpportunity);

        List<VolunteerOpportunity> GetVolunteerOpportunities();

        List<VolunteerOpportunity> GetVisibleOpportunities();

        VolunteerOpportunity GetVolunteerOpportunityById(Guid volOppId);

        void UpdateVolunteerOpportunity(VolunteerOpportunity opportunity);

        int GetClockedInVolunteerCount(Guid volOppId);
    }
}
