using System;
using System.Collections.Generic;
using TimeTracker.Core.Models;

namespace TimeTracker.Core.Contracts
{
    public interface IVolTransaction
    {
        List<VolCreditTransaction> GetTransactionsByVolunteerId(Guid volunteerId);

        VolCreditTransaction CreateTransaction(VolCreditTransaction transaction);

        VolCreditTransaction GetTransactionById(Guid transactionId);

        void DeleteTransaction(Guid transactionid);

        void UpdateTransaction(VolCreditTransaction transaction);
    }
}
