using System;
using TimeTracker.Core.BaseModels;

namespace TimeTracker.Core.Models
{
    public class AdminCreditTransaction : BaseCreditTransaction
    {
        public Guid AdministratorId { get; set; }

        public string AdministratorName { get; set; }
    }
}
