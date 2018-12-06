using FDCommonTools.SqlTools;
using System.Data;
using TimeTracker.Core.Models;

namespace TimeTracker.DAL.Maps
{
    public class VolunteerMaps
    {
        public static Volunteer MapVolunteer(DataRow row)
        {
            Volunteer volunteer = new Volunteer();

            if(row != null)
            {
                volunteer.Id = DataFieldHelper.GetUniqueIdentifier(row, "VolunteerId");
                volunteer.FirstName = DataFieldHelper.GetString(row, "FirstName");
                volunteer.LastName = DataFieldHelper.GetString(row, "LastName");
                volunteer.FullName = DataFieldHelper.GetString(row, "FullName");
                volunteer.Email = DataFieldHelper.GetString(row, "EmailAddress");
                volunteer.PhoneNumber = DataFieldHelper.GetString(row, "PhoneNumber");
                volunteer.EmergencyContactPhone = DataFieldHelper.GetString(row, "EmergencyContactNumber");
                volunteer.SecurityPhrase = DataFieldHelper.GetString(row, "SecurityWordPhrase");
                volunteer.CreditBalance = DataFieldHelper.GetDecimal(row, "CreditBalance");
            }

            return volunteer;
        }
    }
}
