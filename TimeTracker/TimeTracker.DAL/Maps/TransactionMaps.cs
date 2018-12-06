using FDCommonTools.SqlTools;
using System.Data;
using TimeTracker.Core.Models;

namespace TimeTracker.DAL.Maps
{
    public class TransactionMaps
    {
        public static TransactionType MapTransactionType(DataRow row)
        {
            TransactionType type = new TransactionType();

            if (row != null)
            {
                type.Id = DataFieldHelper.GetUniqueIdentifier(row, "TransactionTypeId");
                type.Name = DataFieldHelper.GetString(row, "TransactionType");
                type.MakeNegative.IsChecked = DataFieldHelper.GetBit(row, "MakeNegative");
            }

            return type;
        }

        public static VolCreditTransaction MapTransaction(DataRow row)
        {
            VolCreditTransaction transaction = new VolCreditTransaction();

            if(row != null)
            {
                transaction.Id = DataFieldHelper.GetUniqueIdentifier(row, "TransactionId");                
                transaction.VolunteerId = DataFieldHelper.GetUniqueIdentifier(row, "VolunteerId");
                transaction.CreditAmount = DataFieldHelper.GetDecimal(row, "CreditAmount");
                transaction.Notes = DataFieldHelper.GetString(row, "TransactionNotes");
                transaction.CreateDateTime = DataFieldHelper.GetDateTime(row, "TransactionDateTime");
                transaction.VolunteerName = DataFieldHelper.GetString(row, "VolunteerName");

                transaction.TransactionType = MapTransactionType(row);
            }

            return transaction;
        }
    }
}
