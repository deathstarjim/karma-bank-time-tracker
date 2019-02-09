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

        [DisplayName("Nickname / Phrase:*")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Security word or phrase is requied.")]

        public string SecurityPhrase { get; set; }

        [DisplayName("Volunteer Name:")]
        public string FullName { get; set; }

        [DisplayName("Email Address:")]
        public string Email { get; set; }

    }
}
