using System;
using System.Collections.Generic;
using TimeTracker.Core.Models;

namespace TimeTracker.Core.Contracts
{
    public interface ITransactionType
    {
        List<TransactionType> GetTransactionTypes();

        TransactionType GetTransactionTypeById(Guid transactionTypeId);

        TransactionType CreateTransactionType(TransactionType type);

        void UpdateTransactionType(TransactionType type);

        void DeleteTransactionType(Guid transactionTypeId);

        void UpdateVolunteerTransactionTypes(Guid replacementTypeId, Guid deletedTypeId);

        void UpdateOrgAdminTransactionTypes(Guid replacementTypeId, Guid deletedTypeId);

    }
}
