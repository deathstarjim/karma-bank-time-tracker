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
    public class VolTimePunchRepository : IVolTimePunch
    {
        private DataLoadHelper _helper = new DataLoadHelper(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);

        #region Base Query
        private readonly string _baseQuery = @"
                            SELECT 
	                            punches.[TimePunchId]
                                ,vol.FullName as VolunteerFullName
                                ,vol.VolunteerId
                                ,punches.VolunteerOpportunityId
	                            ,volOpps.OpportunityName
                                ,punches.[PunchInDateTime]
                                ,punches.[PunchOutDateTime]
                                ,punches.[CreditsEarned]
                            FROM [dbo].[TimePunches] punches
                                INNER JOIN Volunteers vol
                                    ON punches.VolunteerId = vol.VolunteerId
	                            INNER JOIN VolunteerOpportunities volOpps
		                            ON punches.VolunteerOpportunityId = volOpps.VolunteerOpportunityId";
        #endregion

        public Guid CreateTimePunchIn(VolTimePunch newPunch)
        {
            string sql = @"
                            INSERT INTO [dbo].[TimePunches]
                                   (
			                           [VolunteerId]
			                           ,[PunchInDateTime]
                                       ,[VolunteerOpportunityId]
                                   )
                             OUTPUT inserted.TimePunchId
	                         VALUES
                                   (
			                           @VolunteerId
			                           ,@PunchInDateTime
                                       ,@VolOppId
                                   )";

            var parameters = new[]
            {
                new SqlParameter("@VolunteerId", newPunch.VolunteerId),
                new SqlParameter("@PunchInDateTime", newPunch.PunchInDateTime),
                new SqlParameter("@VolOppId", newPunch.VolunteerOpportunityId)
            };

            var result = (Guid)_helper.ExecScalarSqlPullObject(sql, parameters);

            if (result != Guid.Empty)
                newPunch.Id = result;

            return newPunch.Id;
        }

        public List<VolTimePunch> GetTimePunches()
        {
            List<VolTimePunch> punches = new List<VolTimePunch>();

            var result = _helper.ExecSqlPullDataTable(_baseQuery);

            if (result != null)
                punches = (from DataRow row in result.Rows select Maps.VolTimePunchMaps.MapTimePunch(row)).ToList();

            return punches;
        }

        public void UpdateTimePunchOut(VolTimePunch punch, int creditValue)
        {
            string sql = @"
                            UPDATE TimePunches
                            SET PunchOutDateTime = @PunchOut
	                            ,CreditsEarned = @CreditValue * CONVERT(decimal(18, 2), DATEDIFF(SECOND, @PunchIn, @PunchOut)) / 60 / 60
	                            ,TimeInSeconds = DATEDIFF(SECOND, @PunchIn, @PunchOut)
                            WHERE TimePunchId = @TimePunchId";

            var parameters = new[]
            {
                new SqlParameter("@PunchIn", punch.PunchInDateTime),
                new SqlParameter("@PunchOut", punch.PunchOutDateTime),
                new SqlParameter("@CreditValue", creditValue),
                new SqlParameter("@TimePunchId", punch.Id)
            };

            var result = _helper.ExecNonQuery(sql, parameters);
        }

        public void UpdateTimePunch(VolTimePunch punch, int creditValue)
        {
            string sql = @"
                            UPDATE TimePunches
                            SET PunchInDateTime = @PunchIn
                                ,PunchOutDateTime = @PunchOut
	                            ,CreditsEarned = @CreditValue * CONVERT(decimal(18, 2), DATEDIFF(SECOND, @PunchIn, @PunchOut)) / 60 / 60
	                            ,TimeInSeconds = DATEDIFF(SECOND, @PunchIn, @PunchOut)
                                ,VolunteerOpportunityId = @VolOppId
                            WHERE TimePunchId = @TimePunchId";

            var parameters = new[]
            {
                new SqlParameter("@PunchIn", punch.PunchInDateTime),
                new SqlParameter("@PunchOut", punch.PunchOutDateTime),
                new SqlParameter("@CreditValue", creditValue),
                new SqlParameter("@VolOppId", punch.VolunteerOpportunityId),
                new SqlParameter("@TimePunchId", punch.Id)
            };

            var result = _helper.ExecNonQuery(sql, parameters);
        }

        public List<VolTimePunch> GetOpenTimePunches()
        {
            List<VolTimePunch> punches = new List<VolTimePunch>();

            string sql = _baseQuery + @" WHERE punches.PunchOutDateTime IS NULL";

            var result = _helper.ExecSqlPullDataTable(sql);

            if (Tools.DataTableHasRows(result))
                punches = (from DataRow row in result.Rows select Maps.VolTimePunchMaps.MapTimePunch(row)).ToList();

            return punches;
        }

        public bool CheckVolunteerClockedIn(Guid volunteerId)
        {
            bool userClockedIn = false;
            int? clockedInCount = 0;

            string sql = @"
                            SELECT
	                            COUNT(*) as ClockedInCount
                            FROM TimePunches punches
                            WHERE punches.PunchOutDateTime IS NULL
	                            AND punches.VolunteerId = @VolunteerId";

            var parameters = new[]
            {
                new SqlParameter("@VolunteerId", volunteerId)
            };

            var result = _helper.ExecSqlPullDataTable(sql, parameters);

            clockedInCount = result.Rows[0].Field<int>("ClockedInCount");

            if (clockedInCount != null && clockedInCount > 0)
                userClockedIn = true;

            return userClockedIn;
        }

        public VolTimePunch GetTimePunchById(Guid timePunchId)
        {
            VolTimePunch punch = new VolTimePunch();

            string sql = _baseQuery + @" WHERE TimePunchId = @TimePunchId";

            var parameters = new[]
            {
                new SqlParameter("@TimePunchId", timePunchId)
            };

            var result = _helper.ExecSqlPullDataTable(sql, parameters);

            if (Tools.DataTableHasRows(result))
                punch = (from DataRow row in result.Rows select Maps.VolTimePunchMaps.MapTimePunch(row)).FirstOrDefault();

            return punch;
        }

        public List<VolTimePunch> GetTimePunchesByVolunteerId(Guid volunteerId)
        {
            List<VolTimePunch> punches = new List<VolTimePunch>();

            string sql = _baseQuery + @" WHERE vol.VolunteerId = @VolunteerId";

            var parameters = new[]
            {
                new SqlParameter("@VolunteerId", volunteerId)
            };

            var result = _helper.ExecSqlPullDataTable(sql, parameters);

            if (Tools.DataTableHasRows(result))
                punches = (from DataRow row in result.Rows select Maps.VolTimePunchMaps.MapTimePunch(row)).ToList();

            return punches;
        }

        public void DeleteTimePunch(Guid punchId)
        {
            string sql = @"DELETE FROM TimePunches WHERE TimePunchId = @PunchId";

            var parameters = new[]
            {
                new SqlParameter("@PunchId", punchId)
            };

            var result = _helper.ExecNonQuery(sql, parameters);
        }

        public VolTimePunch CreateTimePunch(VolTimePunch punch, int creditValue)
        {
            string sql = @"
                            INSERT INTO [dbo].[TimePunches]
	                            (
		                            [VolunteerId]
		                            ,[VolunteerOpportunityId]
		                            ,[PunchInDateTime]
		                            ,[PunchOutDateTime]
                                    ,[CreditsEarned]
                                    ,[TimeInSeconds]
	                            )
                            OUTPUT inserted.TimePunchId
                            VALUES
	                            (
		                            @VolunteerId
		                            ,@VolunteerOpportunityId
		                            ,@PunchInDateTime
		                            ,@PunchOutDateTime
                                    ,@CreditValue * CONVERT(decimal(18, 2), DATEDIFF(SECOND, @PunchInDateTime, @PunchOutDateTime)) / 60 / 60
	                                ,DATEDIFF(SECOND, @PunchInDateTime, @PunchOutDateTime)
	                            )";

            var parameters = new[]
            {
                new SqlParameter("@VolunteerId", punch.VolunteerId),
                new SqlParameter("@VolunteerOpportunityId", punch.VolunteerOpportunityId),
                new SqlParameter("@PunchInDateTime", punch.PunchInDateTime),
                new SqlParameter("@PunchOutDateTime", punch.PunchOutDateTime),
                new SqlParameter("@CreditValue", creditValue)
            };

            var result = (Guid)_helper.ExecScalarSqlPullObject(sql, parameters);

            if (result != Guid.Empty)
                punch.Id = result;

            return punch;
        }
    }
}
