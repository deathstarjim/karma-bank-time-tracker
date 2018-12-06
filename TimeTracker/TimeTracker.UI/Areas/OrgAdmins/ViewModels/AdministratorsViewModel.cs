using System;
using System.Collections.Generic;
using TimeTracker.Core.Models;

namespace TimeTracker.UI.Areas.OrgAdmins.ViewModels
{
    public class AdministratorsViewModel : BaseOrgAdminViewModel
    {
        public AdministratorsViewModel()
        {
            Administrators = new List<Administrator>();
            OpenPunches = new List<AdminTimePunch>();
            AdminTimePunches = new List<AdminTimePunch>();
            SelectedAdministrator = new Administrator();
            TransactionTypes = new List<TransactionType>();
            CreditTransaction = new AdminCreditTransaction();
            Transactions = new List<AdminCreditTransaction>();
            CurrentTimePunch = new AdminTimePunch();
            NewAdministrator = new Administrator();
        }

        public List<Administrator> Administrators { get; set; }

        public Administrator NewAdministrator { get; set; }

        /// <summary>
        /// List of time punches with a clock in time and no clock out time
        /// </summary>
        public List<AdminTimePunch> OpenPunches { get; set; }

        /// <summary>
        /// All of the time punches for an Administrator
        /// </summary>
        public List<AdminTimePunch> AdminTimePunches { get; set; }

        public Administrator SelectedAdministrator { get; set; }

        public AdminTimePunch CurrentTimePunch { get; set; }

        public AdminCreditTransaction CreditTransaction { get; set; }

        public List<AdminCreditTransaction> Transactions { get; set; }

        public Guid SelectedTransactionType { get; set; }

    }
}