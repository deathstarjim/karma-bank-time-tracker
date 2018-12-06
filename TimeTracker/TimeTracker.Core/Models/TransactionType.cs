using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace TimeTracker.Core.Models
{
    public class TransactionType
    {
        public TransactionType()
        {
            MakeNegative = new CheckBoxItem();
        }

        [DisplayName("Transaction Type:*")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please select a transaction type")]
        public Guid Id { get; set; }

        [DisplayName("Transaction Type:*")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please enter a transaction type")]
        public string Name { get; set; }

        [DisplayName("Make the transaction amount negative?")]
        public CheckBoxItem MakeNegative { get; set; }
    }
}
