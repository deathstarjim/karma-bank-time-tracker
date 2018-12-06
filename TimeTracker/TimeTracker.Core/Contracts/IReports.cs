using System;
using System.Collections.Generic;
using System.Data;
using TimeTracker.Core.Models;

namespace TimeTracker.Core.Contracts
{
    public interface IReports
    {
        List<CreditReportItem> GetCreditReportByVolunteerId(Guid volunteerId);

        List<CreditReportItem> GetCreditReportByDateRange(DateTime startDate, DateTime endDate);

        List<TransactionReportItem> GetTransactionsByDateRange(DateTime startDate, DateTime endDate);

        DataTable VolunteerSummaryByDateRange(DateTime startDate, DateTime endDate);
    }
}
