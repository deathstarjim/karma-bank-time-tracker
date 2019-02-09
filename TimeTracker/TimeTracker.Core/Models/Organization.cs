using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace TimeTracker.Core.Models
{
    public class Organization
    {
        public Organization()
        {
            TaxExemptionFile = new byte[] { };
        }

        public Guid Id { get; set; }

        [DisplayName("What is your organization's name?")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "The organization name is required.")]
        public string Name { get; set; }

        [DisplayName("What is your organziation's website?")]
        public string WebsiteUrl { get; set; }

        [DisplayName("Tell us about your organization:")]
        public string Description { get; set; }

        [DisplayName("Upload your 1023c file:*")]
        public HttpPostedFileBase PostedFile { get; set; }

        public byte[] TaxExemptionFile { get; set; }

        [DisplayName("Enter a sub-domain for your organziation: (i.e. yourorg.biketimetracker.com)")]
        public string Subdomain { get; set; }
    }
}
