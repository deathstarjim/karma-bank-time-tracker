using FDCommonTools.SqlTools;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using TimeTracker.Core.Contracts;
using TimeTracker.Core.Models;
using System.Linq;

namespace TimeTracker.DAL.Repositories
{
    public class VolunteerRepository : IVolunteer
    {
        private DataLoadHelper _helper = new DataLoadHelper(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);

        #region "Base Query"
        private readonly string _baseQuery = @"
                            SELECT 
	                            vols.[VolunteerId]
	                            ,vols.[FirstName]
	                            ,vols.[LastName]
	                            ,vols.[FullName]
	                            ,vols.[EmailAddress]
	                            ,vols.[PhoneNumber]
	                            ,vols.[EmergencyContactNumber]
	                            ,vols.[CreateDateTime]
                                ,vols.[SecurityWordPhrase]
                                ,vols.[CreditBalance]
                            FROM [dbo].[Volunteers] vols";
        #endregion

        public Guid CreateVolunteer(Volunteer newVolunteer)
        {
            string sql = @"
                            INSERT INTO [dbo].[Volunteers]
	                            (
		                            [SystemRoleId]
		                            ,[FirstName]
		                            ,[LastName]
		                            ,[FullName]
		                            ,[EmailAddress]
		                            ,[PhoneNumber]
		                            ,[EmergencyContactNumber]
                                    ,[SecurityWordPhrase]
	                            )
	                            OUTPUT inserted.VolunteerId
	                            VALUES
	                            (
		                            @SystemRoleId
		                            ,@FirstName
		                            ,@LastName
		                            ,@FullName
		                            ,@EmailAddress
		                            ,@PhoneNumber
		                            ,@EmergencyContactNumber
                                    ,@SecurityWordPhrase
	                            )";

            var parameters = new[]
            {
                new SqlParameter("@SystemRoleId", newVolunteer.Role.Id),
                new SqlParameter("@FirstName", newVolunteer.FirstName ?? ""),
                new SqlParameter("@LastName", newVolunteer.LastName ?? ""),
                new SqlParameter("@FullName", newVolunteer.FirstName + ' ' + newVolunteer.LastName ?? ""),
                new SqlParameter("@EmailAddress", newVolunteer.Email ?? ""),
                new SqlParameter("@PhoneNumber", newVolunteer.PhoneNumber ?? ""),
                new SqlParameter("@EmergencyContactNumber", newVolunteer.EmergencyContactPhone),
                new SqlParameter("@SecurityWordPhrase", newVolunteer.SecurityPhrase)
            };

            var result = (Guid)_helper.ExecScalarSqlPullObject(sql, parameters);

            if (result != Guid.Empty)
                newVolunteer.Id = result;

            return newVolunteer.Id;
        }

        public Volunteer GetVolunteerById(Guid volunteerId)
        {
            Volunteer volunteer = new Volunteer();

            string sql = _baseQuery + @" WHERE vols.VolunteerId = @VolunteerId";

            var parameters = new[]
            {
                new SqlParameter("@VolunteerId", volunteerId)
            };

            var result = _helper.ExecSqlPullDataTable(sql, parameters);

            if (Tools.DataTableHasRows(result))
                volunteer = (from DataRow row in result.Rows select Maps.VolunteerMaps.MapVolunteer(row)).FirstOrDefault();

            return volunteer;
        }

        public List<Volunteer> GetVolunteers()
        {
            List<Volunteer> volunteers = new List<Volunteer>();

            string sql = _baseQuery;
            
            var result = _helper.ExecSqlPullDataTable(sql);

            if (result != null)
                volunteers = (from DataRow row in result.Rows select Maps.VolunteerMaps.MapVolunteer(row)).ToList();

            return volunteers;
        }

        public List<Volunteer> SearchVolunteersByName(string name)
        {
            List<Volunteer> volunteers = new List<Volunteer>();

            string sql = _baseQuery + @" WHERE FullName LIKE '%{0}%'";

            sql = string.Format(sql, name);
            
            var result = _helper.ExecSqlPullDataTable(sql);

            if (result != null)
                volunteers = (from DataRow row in result.Rows select Maps.VolunteerMaps.MapVolunteer(row)).ToList();

            return volunteers;
        }

        public void UpdateCreditBalance(Guid volunteerId)
        {
            string sql = @"
                            UPDATE vols
                            SET CreditBalance = ISNULL(CreditSum.TotalCredits, 0) + ISNULL(CreditTrans.TotalTransCredits, 0)
                            FROM Volunteers vols
	                            LEFT OUTER JOIN 
	                            (
		                            SELECT
			                            punches.VolunteerId
			                            ,SUM(punches.CreditsEarned) as TotalCredits
		                            FROM TimePunches punches
		                            WHERE punches.VolunteerId = @VolunteerId
		                            GROUP BY punches.VolunteerId
	                            ) as CreditSum
		                            ON vols.VolunteerId = CreditSum.VolunteerId
	                            LEFT OUTER JOIN
	                            (
		                            SELECT
			                            trans.VolunteerId
			                            ,SUM(trans.CreditAmount) as TotalTransCredits
		                            FROM CreditTransactions trans
		                            WHERE trans.VolunteerId = @VolunteerId
		                            GROUP BY trans.VolunteerId
	                            ) as CreditTrans
		                            ON CreditTrans.VolunteerId = vols.VolunteerId
                            WHERE vols.VolunteerId = @VolunteerId";

            var parameters = new[]
            {
                new SqlParameter("@VolunteerId", volunteerId)
            };

            var result = _helper.ExecNonQuery(sql, parameters);
        }

        public void UpdateVolunteerDetails(Volunteer volunteer)
        {
            string sql = @"
                            UPDATE [dbo].[Volunteers]
                            SET [FirstName] = @FirstName
                                ,[LastName] = @LastName
                                ,[FullName] = @FullName
                                ,[EmailAddress] = @EmailAddress
                                ,[PhoneNumber] = @PhoneNumber
                                ,[EmergencyContactNumber] = @EmergencyContactNumber
                                ,[SecurityWordPhrase] = @SecurityWordPhrase
                                ,[Active] = @Active
                            WHERE VolunteerId = @VolId";

            var parameters = new[]
            {
                new SqlParameter("@FirstName", volunteer.FirstName),
                new SqlParameter("@LastName", volunteer.LastName),
                new SqlParameter("@FullName", volunteer.FirstName + ' ' + volunteer.LastName),
                new SqlParameter("@EmailAddress", volunteer.Email ?? ""),
                new SqlParameter("@PhoneNumber", volunteer.PhoneNumber ?? ""),
                new SqlParameter("@EmergencyContactNumber", volunteer.EmergencyContactPhone),
                new SqlParameter("@SecurityWordPhrase", volunteer.SecurityPhrase ?? ""),
                new SqlParameter("@Active", volunteer.Active ?? false),
                new SqlParameter("@VolId", volunteer.Id)
            };

            var result = _helper.ExecNonQuery(sql, parameters);
        }
    }
}
