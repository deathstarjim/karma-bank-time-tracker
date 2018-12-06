using System;
using TimeTracker.Core.BaseModels;

namespace TimeTracker.Core.Models
{
    public class AdminTimePunch : BasePunch
    {
        public Guid AdministratorId { get; set; }

        public string FullName { get; set; }

    }
}
