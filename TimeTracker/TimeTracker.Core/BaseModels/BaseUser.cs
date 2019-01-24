using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using TimeTracker.Core.Models;

namespace TimeTracker.Core.BaseModels
{
    public class BaseUser
    {
        public BaseUser()
        {
            Role = new SystemRole();
        }

        public Guid Id { get; set; }

        public Guid OrganizationId { get; set; }

        public SystemRole Role { get; set; }

        [DisplayName("Email Address:")]
        public string Email { get; set; }

        [DisplayName("Primary Phone Number:")]
        public string PhoneNumber { get; set; }

        [DisplayName("Emergency Contact Number:*")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Emergency contact phone number is required.")]
        public string EmergencyContactPhone { get; set; }

        [DisplayName("Credit Balance:")]
        public decimal CreditBalance { get; set; }

    }
}
