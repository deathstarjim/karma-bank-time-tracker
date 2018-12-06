using FDCommonTools.SqlTools;
using System.Data;
using TimeTracker.Core.Models;

namespace TimeTracker.DAL.Maps
{
    public class VolunteerOpportunityMaps
    {
        public static VolunteerOpportunity MapVolunteerOpportunity(DataRow row)
        {
            VolunteerOpportunity opportunity = new VolunteerOpportunity();

            if(row != null)
            {
                opportunity.Id = DataFieldHelper.GetUniqueIdentifier(row, "VolunteerOpportunityId");
                opportunity.Name = DataFieldHelper.GetString(row, "OpportunityName");
                opportunity.IsOffsite.IsChecked = DataFieldHelper.GetBit(row, "IsOffsiteEvent");
                opportunity.IsVisibleToVolunteer.IsChecked = DataFieldHelper.GetBit(row, "IsVisible");
                opportunity.CreditValue = DataFieldHelper.GetInt(row, "CreditValue");
                opportunity.Description = DataFieldHelper.GetString(row, "OpportunityDescription");
                opportunity.StartDateTime = DataFieldHelper.GetDateTime(row, "OpportunityStartDateTime");
                opportunity.EndDateTime = DataFieldHelper.GetDateTime(row, "OpportunityEndDateTime");
            }

            return opportunity;
        }
    }
}
