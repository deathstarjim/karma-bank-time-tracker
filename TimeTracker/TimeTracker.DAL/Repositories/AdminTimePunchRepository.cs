using FDCommonTools.SqlTools;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using TimeTracker.Core.Contracts;
using TimeTracker.Core.Models;

namespace TimeTracker.DAL.Repositories
{
    public class AdminTimePunchRepository : IAdminTimePunch
    {
        private DataLoadHelper _helper = new DataLoadHelper(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);

        #region "Base Query"
        private const string _baseQuery = @"
                            SELECT
	                            admins.[AdministratorId]
	                            ,admins.[FirstName] + ' ' + admins.[LastName] as FullName
                                ,opunches.AdminTimePunchId
	                            ,opunches.PunchInDateTime
	                            ,opunches.PunchOutDateTime
	                            ,opunches.CreditsEarned
                            FROM OrgAdminTimePunches opunches
                                LEFT OUTER JOIN Administrators admins
		                            ON opunches.AdministratorId = admins.AdministratorId";
        #endregion

        public AdminTimePunch GetTimePunchById(Guid punchId)
        {
            AdminTimePunch punch = new AdminTimePunch();

            string sql = _baseQuery + @" WHERE opunches.AdminTimePunchId = @PunchId";

            var parameters = new[]
            {
                new SqlParameter("@PunchId", punchId)
            };

            var result = _helper.ExecSqlPullDataTable(sql, parameters);

            if (Tools.DataTableHasRows(result))
                punch = (from DataRow row in result.Rows select Maps.AdministratorMaps.MapAdminTimePunch(row)).FirstOrDefault();

            return punch;
        }

        public List<AdminTimePunch> OpenTimePunches()
        {
            List<AdminTimePunch> punches = new List<AdminTimePunch>();
            string sql = _baseQuery + @" WHERE opunches.PunchInDateTime IS NOT NULL
	                            AND opunches.PunchOutDateTime IS NULL";

            var result = _helper.ExecSqlPullDataTable(sql);

            if (Tools.DataTableHasRows(result))
                punches = (from DataRow row in result.Rows select Maps.AdministratorMaps.MapAdminTimePunch(row)).ToList();

            return punches;
        }

        public AdminTimePunch PunchAdminIn(AdminTimePunch newPunch)
        {
            string sql = @"
                            INSERT INTO [dbo].[OrgAdminTimePunches]
	                            (
		                            [AdministratorId]
		                            ,[PunchInDateTime]
	                            )
                            OUTPUT inserted.AdminTimePunchId
                            VALUES
	                            (
		                            @AdministratorId
		                            ,@PunchInDateTime
	                            )";

            var parameters = new[]
            {
                new SqlParameter("@AdministratorId", newPunch.AdministratorId),
                new SqlParameter("@PunchInDateTime", newPunch.PunchInDateTime)
            };

            var result = (Guid)_helper.ExecScalarSqlPullObject(sql, parameters);

            if (result != Guid.Empty)
                newPunch.Id = result;

            return newPunch;
        }

        public void PunchAdminOut(AdminTimePunch punch)
        {
            string sql = @"
                            UPDATE OrgAdminTimePunches
                            SET PunchOutDateTime = @PunchOut
	                            ,CreditsEarned = 5 * CONVERT(decimal(18, 2), DATEDIFF(SECOND, @PunchIn, @PunchOut)) / 60 / 60
	                            ,TimeInSeconds = DATEDIFF(SECOND, @PunchIn, @PunchOut)
                            WHERE AdminTimePunchId = @TimePunchId";

            var parameters = new[]
            {
                new SqlParameter("@PunchOut", punch.PunchOutDateTime),
                new SqlParameter("@PunchIn", punch.PunchInDateTime),
                new SqlParameter("@TimePunchId", punch.Id)
            };

            var result = _helper.ExecNonQuery(sql, parameters);
        }

        public List<AdminTimePunch> GetTimePunchesByAdminId(Guid administratorId)
        {
            List<AdminTimePunch> punches = new List<AdminTimePunch>();

            string sql = _baseQuery + @" WHERE admins.AdministratorId = @AdminId";

            var parameters = new[]
            {
                new SqlParameter("@AdminId", administratorId)
            };

            var result = _helper.ExecSqlPullDataTable(sql, parameters);

            if (Tools.DataTableHasRows(result))
                punches = (from DataRow row in result.Rows select Maps.AdministratorMaps.MapAdminTimePunch(row)).ToList();

            return punches;
        }

        public void UpdateTimePunch(AdminTimePunch punch)
        {
            string sql = @"
                            UPDATE OrgAdminTimePunches
                            SET PunchInDateTime = @PunchIn
                                ,PunchOutDateTime = @PunchOut
	                            ,CreditsEarned = 5 * CONVERT(decimal(18, 2), DATEDIFF(SECOND, @PunchIn, @PunchOut)) / 60 / 60
	                            ,TimeInSeconds = DATEDIFF(SECOND, @PunchIn, @PunchOut)
                            WHERE AdminTimePunchId = @TimePunchId";

            var parameters = new[]
            {
                new SqlParameter("@PunchIn", punch.PunchInDateTime),
                new SqlParameter("@PunchOut", punch.PunchOutDateTime),
                new SqlParameter("@TimePunchId", punch.Id)
            };

            var result = _helper.ExecNonQuery(sql, parameters);
        }

        public void DeleteTimePunch(Guid timePunchId, Guid administratorId)
        {
            string sql = @"DELETE FROM [dbo].[OrgAdminTimePunches] WHERE AdminTimePunchId = @TimePunchId AND AdministratorId = @AdminId";

            var parameters = new[]
            {
                new SqlParameter("@TimePunchId", timePunchId),
                new SqlParameter("@AdminId", administratorId)
            };

            var result = _helper.ExecNonQuery(sql, parameters);
        }

        public bool CheckAdminClockedIn(Guid adminId)
        {
            bool userClockedIn = false;
            int? clockedInCount = 0;

            string sql = @"
                            SELECT 
	                            COUNT(*) as ClockedInCount
                            FROM [OrgAdminTimePunches] punches
                            WHERE punches.PunchOutDateTime IS NULL
	                            AND punches.AdministratorId = @AdminId";

            var parameters = new[]
            {
                new SqlParameter("@AdminId", adminId)
            };

            var result = _helper.ExecSqlPullDataTable(sql, parameters);

            clockedInCount = result.Rows[0].Field<int>("ClockedInCount");

            if (clockedInCount != null && clockedInCount > 0)
                userClockedIn = true;

            return userClockedIn;

        }
    }
}
