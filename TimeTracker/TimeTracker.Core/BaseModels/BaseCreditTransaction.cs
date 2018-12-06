using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using TimeTracker.Core.Models;

namespace TimeTracker.Core.BaseModels
{
    public class BaseCreditTransaction
    {
        public BaseCreditTransaction()
        {
            TransactionType = new TransactionType();
        }

        public Guid Id { get; set; }

        public TransactionType TransactionType { get; set; }

        [DisplayName("Notes:")]
        public string Notes { get; set; }

        [DisplayName("Credit Amount:*")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please enter a credit amount.")]
        public decimal CreditAmount { get; set; }

        [DisplayName("Transaction Date:")]
        public DateTime CreateDateTime { get; set; }

    }
}
