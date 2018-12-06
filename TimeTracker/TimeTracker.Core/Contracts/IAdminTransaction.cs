using System;
using System.Collections.Generic;
using TimeTracker.Core.Models;

namespace TimeTracker.Core.Contracts
{
    public interface IAdminTransaction
    {
        List<AdminCreditTransaction> GetTransactionsByAdminId(Guid administratorId);

        AdminCreditTransaction CreateTransaction(AdminCreditTransaction transaction);

        AdminCreditTransaction GetTransactionById(Guid transactionId);

        void UpdateTransaction(AdminCreditTransaction transaction);

        void DeleteTransaction(Guid transactionId);
    }
}
