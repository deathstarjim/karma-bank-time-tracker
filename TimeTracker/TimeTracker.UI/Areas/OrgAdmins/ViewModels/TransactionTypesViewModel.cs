using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using TimeTracker.Core.Models;

namespace TimeTracker.UI.Areas.OrgAdmins.ViewModels
{
    public class TransactionTypesViewModel : BaseOrgAdminViewModel
    {
        public TransactionTypesViewModel()
        {
            TransactionTypes = new List<TransactionType>();
            CurrentTransactionType = new TransactionType();
        }

        [DisplayName("Select a transaction type:*")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "A transaction type is required.")]
        public TransactionType CurrentTransactionType { get; set; }

        public Guid SelectedTransactionType { get; set; }

        [DisplayName("Select a transaction type to replace the being deleted:*")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "A replacement transaction type is required.")]
        public List<TransactionType> ReplacementTransactionTypes { get; set; }

        
        public Guid ReplacementTransactionType { get; set; }
    }
}