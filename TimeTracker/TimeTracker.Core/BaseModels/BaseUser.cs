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

        [DisplayName("First Name:*")]
        [Required(AllowEmptyStrings = true, ErrorMessage = "First name is required.")]
        public string FirstName { get; set; }

        [DisplayName("Last Name:*")]
        [Required(AllowEmptyStrings = true, ErrorMessage = "Last name is required.")]
        public string LastName { get; set; }

        [DisplayName("Primary Phone Number:")]
        public string PhoneNumber { get; set; }

        [DisplayName("Emergency Contact Number:*")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Emergency contact phone number is required.")]
        public string EmergencyContactPhone { get; set; }

        [DisplayName("Credit Balance:")]
        public decimal CreditBalance { get; set; }

    }
}
