using FDCommonTools.SqlTools;
using System.Data;
using TimeTracker.Core.Models;

namespace TimeTracker.DAL.Maps
{
    public class VolTimePunchMaps
    {
        public static VolTimePunch MapTimePunch(DataRow row)
        {
            VolTimePunch punch = new VolTimePunch();

            if(row != null)
            {
                punch.Id = DataFieldHelper.GetUniqueIdentifier(row, "TimePunchId");
                punch.VolunteerId = DataFieldHelper.GetUniqueIdentifier(row, "VolunteerId");
                punch.VolunteerOpportunityId = DataFieldHelper.GetUniqueIdentifier(row, "VolunteerOpportunityId");
                punch.VolunteerOpportunityName = DataFieldHelper.GetString(row, "OpportunityName");
                punch.VolunteerFullName = DataFieldHelper.GetString(row, "VolunteerFullName");
                punch.PunchInDateTime = DataFieldHelper.GetDateTime(row, "PunchInDateTime");
                punch.PunchOutDateTime = DataFieldHelper.GetDateTime(row, "PunchOutDateTime");
                punch.CreditsEarned = DataFieldHelper.GetDecimal(row, "CreditsEarned");
            }

            return punch;
        }

    }
}
