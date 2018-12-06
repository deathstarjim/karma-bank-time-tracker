using System;
using TimeTracker.Core.BaseModels;

namespace TimeTracker.Core.Models
{
    public class TransactionReportItem : BaseReportItem
    {
        public DateTime TransactionDateTime { get; set; }

        public string TransactionType { get; set; }
    }
}
