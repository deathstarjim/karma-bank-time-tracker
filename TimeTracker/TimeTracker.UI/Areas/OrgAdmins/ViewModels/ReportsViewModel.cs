using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TimeTracker.Core.Models;

namespace TimeTracker.UI.Areas.OrgAdmins.ViewModels
{
    public class ReportsViewModel : BaseOrgAdminViewModel
    {
        public ReportsViewModel()
        {
            CreditResults = new List<CreditReportItem>();
            TransactionResults = new List<TransactionReportItem>();
        }

        public List<CreditReportItem> CreditResults { get; set; }

        public List<TransactionReportItem> TransactionResults { get; set; }

        public string CreditDateRange { get; set; }

        public string TransactionDateRange { get; set; }

        public string VolSummaryDateRange { get; set; }
    }
}