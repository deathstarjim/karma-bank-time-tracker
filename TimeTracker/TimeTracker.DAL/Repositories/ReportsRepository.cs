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
    public class ReportsRepository : IReports
    {
        private DataLoadHelper _helper = new DataLoadHelper(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);

        #region "Base Query"
        private const string _baseCreditQuery = @"
                                            SELECT
	                                            vols.FullName as VolunteerName
	                                            ,volopps.OpportunityName
	                                            ,punches.PunchInDateTime
	                                            ,punches.PunchOutDateTime
	                                            ,CONVERT(decimal(18, 2), punches.TimeInSeconds) / 60 as DurationInMinutes
	                                            ,CONVERT(decimal(18, 2), punches.TimeInSeconds) / 60 / 60 as DurationInHours
	                                            ,punches.CreditsEarned
                                            FROM Volunteers vols
	                                            LEFT OUTER JOIN TimePunches punches
		                                            ON vols.VolunteerId = punches.VolunteerId
	                                            INNER JOIN VolunteerOpportunities volopps
			                                            ON volopps.VolunteerOpportunityId = punches.VolunteerOpportunityId
                                            WHERE punches.PunchOutDateTime IS NOT NULL";

        private const string _baseTransQuery = @"
                                            SELECT
	                                            vols.FullName as VolunteerName
	                                            ,transactions.CreateDateTime as TransactionDateTime
	                                            ,transactions.CreditAmount
	                                            ,transtypes.TransactionType
                                            FROM Volunteers vols
	                                            INNER JOIN CreditTransactions transactions
		                                            ON vols.VolunteerId = transactions.VolunteerId
	                                            INNER JOIN TransactionTypes transtypes
		                                            ON transactions.TransactionTypeId = transtypes.TransactionTypeId";

        #endregion

        public List<CreditReportItem> GetCreditReportByDateRange(DateTime startDate, DateTime endDate)
        {
            List<CreditReportItem> items = new List<CreditReportItem>();

            string sql = _baseCreditQuery + @" AND punches.PunchOutDateTime BETWEEN @StartDate AND @EndDate 
                                               ORDER BY vols.FullName ASC, punches.PunchOutDateTime DESC";

            var parameters = new[]
            {
                new SqlParameter("@StartDate", startDate),
                new SqlParameter("@EndDate", endDate)
            };

            var result = _helper.ExecSqlPullDataTable(sql, parameters);

            if (Tools.DataTableHasRows(result))
                items = (from DataRow row in result.Rows select Maps.ReportsMaps.MapReportItem(row)).ToList();

            return items;
        }

        public List<CreditReportItem> GetCreditReportByVolunteerId(Guid volunteerId)
        {
            List<CreditReportItem> items = new List<CreditReportItem>();

            string sql = _baseCreditQuery + @" WHERE vols.VolunteerId = @VolunteerId";

            var parameters = new[]
            {
                new SqlParameter("@VolunteerId", volunteerId)
            };

            var result = _helper.ExecSqlPullDataTable(sql, parameters);

            if (Tools.DataTableHasRows(result))
                items = (from DataRow row in result.Rows select Maps.ReportsMaps.MapReportItem(row)).ToList();

            return items;
        }

        public List<TransactionReportItem> GetTransactionsByDateRange(DateTime startDate, DateTime endDate)
        {
            List<TransactionReportItem> items = new List<TransactionReportItem>();

            string sql = _baseTransQuery + @" WHERE transactions.CreateDateTime BETWEEN @StartDate AND @EndDate";

            var parameters = new[]
            {
                new SqlParameter("@StartDate", startDate),
                new SqlParameter("@EndDate", endDate)
            };

            var result = _helper.ExecSqlPullDataTable(sql, parameters);

            if (Tools.DataTableHasRows(result))
                items = (from DataRow row in result.Rows select Maps.ReportsMaps.MapTransactionItem(row)).ToList();

            return items;
        }

        public DataTable VolunteerSummaryByDateRange(DateTime startDate, DateTime endDate)
        {
            string sql = @"
                            SELECT
	                            vols.FullName + ' - ' + vols.[SecurityWordPhrase]  as VolunteerName
	                            ,SUM(CONVERT(decimal(18, 2), punches.TimeInSeconds)) / 60 / 60 as DurationInHours
	                            ,SUM(punches.CreditsEarned) as TotalCreditsEarned
                            FROM Volunteers vols
                            LEFT OUTER JOIN TimePunches punches
	                            ON vols.VolunteerId = punches.VolunteerId
                            INNER JOIN VolunteerOpportunities volopps
		                            ON volopps.VolunteerOpportunityId = punches.VolunteerOpportunityId
                            WHERE punches.PunchOutDateTime IS NOT NULL
	                            AND punches.PunchOutDateTime BETWEEN @StartDate AND @EndDate
                            GROUP BY vols.FullName, vols.[SecurityWordPhrase]
                            ORDER BY vols.FullName";

            var parameters = new[]
            {
                new SqlParameter("@StartDate", startDate),
                new SqlParameter("@EndDate", endDate)
            };

            var result = _helper.ExecSqlPullDataTable(sql, parameters);

            return result;
        }
    }
}
