using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace TimeTracker.Core.BaseModels
{
    public class BasePunch
    {
        public Guid Id { get; set; }

        [DisplayName("Punch In Time:*")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Punch in time is a required field.")]
        public DateTime PunchInDateTime { get; set; }

        [DisplayName("Punch Out Time:*")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Punch out time is a required field.")]
        public DateTime PunchOutDateTime { get; set; }

        [DisplayName("Credits Earned:")]
        public decimal CreditsEarned { get; set; }

    }
}
