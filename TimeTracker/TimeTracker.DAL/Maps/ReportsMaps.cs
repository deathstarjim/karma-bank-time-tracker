using FDCommonTools.SqlTools;
using System.Data;
using TimeTracker.Core.Models;

namespace TimeTracker.DAL.Maps
{
    public class ReportsMaps
    {
        public static CreditReportItem MapReportItem(DataRow row)
        {
            CreditReportItem item = new CreditReportItem();

            if(row != null)
            {
                item.VolunteerName = DataFieldHelper.GetString(row, "VolunteerName");
                item.VolOppName = DataFieldHelper.GetString(row, "OpportunityName");
                item.PunchInDateTime = DataFieldHelper.GetDateTime(row, "PunchInDateTime");
                item.PunchOutDateTime = DataFieldHelper.GetDateTime(row, "PunchOutDateTime");
                item.DurationInMinutes = DataFieldHelper.GetDecimal(row, "DurationInMinutes");
                item.DurationInHours = DataFieldHelper.GetDecimal(row, "DurationInHours");
                item.CreditsEarned = DataFieldHelper.GetDecimal(row, "CreditsEarned");
            }

            return item;
        }

        public static TransactionReportItem MapTransactionItem(DataRow row)
        {
            TransactionReportItem item = new TransactionReportItem();

            if(row != null)
            {
                item.VolunteerName = DataFieldHelper.GetString(row, "VolunteerName");
                item.CreditsEarned = DataFieldHelper.GetDecimal(row, "CreditAmount");
                item.TransactionType = DataFieldHelper.GetString(row, "TransactionType");
                item.TransactionDateTime = DataFieldHelper.GetDateTime(row, "TransactionDateTime");
            }

            return item;
        }
    }
}
