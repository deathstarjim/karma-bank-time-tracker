using System;
using TimeTracker.Core.BaseModels;

namespace TimeTracker.Core.Models
{
    public class CreditReportItem : BaseReportItem
    {
        public string VolOppName { get; set; }

        public DateTime PunchInDateTime { get; set; }

        public DateTime PunchOutDateTime { get; set; }

        public decimal DurationInMinutes { get; set; }

        public decimal DurationInHours { get; set; }

    }
}
