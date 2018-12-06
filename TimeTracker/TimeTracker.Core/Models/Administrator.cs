using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using TimeTracker.Core.BaseModels;

namespace TimeTracker.Core.Models
{
    public class Administrator : BaseUser
    {
        public Administrator()
        {
            Role = new SystemRole();
        }

        [DisplayName("First Name:*")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "First name is required.")]
        public string FirstName { get; set; }

        [DisplayName("Last Name:*")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Last name is required.")]
        public string LastName { get; set; }

        public string FullName { get; set; }

        [DisplayName("User Name:*")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "User name is a required field.")]
        public string UserName { get; set; }

        [DisplayName("Password:*")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Password is a required field.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public string PasswordSalt { get; set; }

        [DisplayName("Email Address:*")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Email address is required.")]
        public string EmailAddress { get; set; }

        [DisplayName("Active:")]
        public bool Active { get; set; }

    }
}
