using System.Collections.Generic;
using TimeTracker.Core.Models;
using TimeTracker.UI.Models;

namespace TimeTracker.UI.Areas.OrgAdmins.ViewModels
{
    public class BaseOrgAdminViewModel
    {
        public BaseOrgAdminViewModel()
        {
            CurrentAdministrator = new Administrator();
            UserMessage = new UserMessage();
            TransactionTypes = new List<TransactionType>();
        }

        public Administrator CurrentAdministrator { get; set; }

        public UserMessage UserMessage { get; set; }

        public List<TransactionType> TransactionTypes { get; set; }
    }
}