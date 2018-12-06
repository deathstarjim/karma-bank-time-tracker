using System;
using System.Collections.Generic;
using TimeTracker.Core.Models;

namespace TimeTracker.UI.Areas.OrgAdmins.ViewModels
{
    public class VolMgmtViewModel : BaseOrgAdminViewModel
    {
        public VolMgmtViewModel()
        {
            CurrentVolunteer = new Volunteer();
            CurrentVolPunch = new VolTimePunch();
            Volunteers = new List<Volunteer>();
            VolTimePunches = new List<VolTimePunch>();
            VolunteerOpportunities = new List<VolunteerOpportunity>();
            CreditTransaction = new VolCreditTransaction();
            TransactionTypes = new List<TransactionType>();
            CreditTransactions = new List<VolCreditTransaction>();
        }

        public Guid SelectedVolunteerOpportunity { get; set; }

        public Guid SelectedTransactionType { get; set; }

        public Volunteer CurrentVolunteer { get; set; }

        public VolTimePunch CurrentVolPunch { get; set; }

        public List<Volunteer> Volunteers { get; set; }

        public List<VolunteerOpportunity> VolunteerOpportunities { get; set; }

        public List<VolTimePunch> VolTimePunches { get; set; }

        public VolCreditTransaction CreditTransaction { get; set; }

        public List<VolCreditTransaction> CreditTransactions { get; set; }

    }
}