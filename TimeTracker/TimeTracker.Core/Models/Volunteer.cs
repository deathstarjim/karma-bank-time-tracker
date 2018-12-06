using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using TimeTracker.Core.BaseModels;

namespace TimeTracker.Core.Models
{
    public class Volunteer : BaseUser
    {
        public Volunteer()
        {
            Role = new SystemRole();
        }

        [DisplayName("First Name:*")]
        [Required(AllowEmptyStrings = true, ErrorMessage = "First name is required.")]
        public string FirstName { get; set; }

        [DisplayName("Last Name:*")]
        [Required(AllowEmptyStrings = true, ErrorMessage = "Last name is required.")]
        public string LastName { get; set; }

        [DisplayName("Nickname / Phrase:*")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Security word or phrase is requied.")]

        public string SecurityPhrase { get; set; }

        [DisplayName("Volunteer Name:")]
        public string FullName { get; set; }

    }
}
