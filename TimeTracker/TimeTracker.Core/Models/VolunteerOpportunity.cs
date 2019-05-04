using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web;
using TimeTracker.Core.Helpers;

namespace TimeTracker.Core.Models
{
    public class VolunteerOpportunity
    {
        public VolunteerOpportunity()
        {
            IsOffsite = new CheckBoxItem();
            IsVisibleToVolunteer = new CheckBoxItem();
            Image = new byte[] { };
        }

        public Guid Id { get; set; }

        [DisplayName("Volunteer Opportunity Name:")]
        public string Name { get; set; }

        [DisplayName("Is the event offsite?")]
        public CheckBoxItem IsOffsite { get; set; }

        [DisplayName("Credit Value:")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Credit value is required.")]
        public int CreditValue { get; set; }

        [DisplayName("Should a volunteer see this opportunity?")]
        public CheckBoxItem IsVisibleToVolunteer { get; set; }

        public string Description { get; set; }

        [DisplayName("Start / End Date & Time:")]
        public string DateRange { get; set; }

        [DisplayName("Opportunity Start Date / Time:")]
        public DateTime? StartDateTime { get; set; }

        [DisplayName("Opportunity End Date / Time:")]
        public DateTime? EndDateTime { get; set; }

        [DisplayName("Max Number of Volunteers (0 = unlimited):")]
        public int VolunteerLimit { get; set; }

        [DisplayName("Select Image:")]
        [AllowFileSize(ErrorMessage = "Maximum file size allowed is 4 MB", FileSize = 4 * 1024 * 1024)]
        public HttpPostedFileBase PostedFile { get; set; }

        public byte[] Image { get; set; }

    }
}
