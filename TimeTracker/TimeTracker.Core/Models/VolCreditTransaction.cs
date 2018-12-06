using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using TimeTracker.Core.BaseModels;

namespace TimeTracker.Core.Models
{
    public class VolCreditTransaction : BaseCreditTransaction
    {

        public Guid VolunteerId { get; set; }

        public string VolunteerName { get; set; }

    }
}
