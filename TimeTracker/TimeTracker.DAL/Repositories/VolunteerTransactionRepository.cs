using FDCommonTools.SqlTools;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using TimeTracker.Core.Contracts;
using TimeTracker.Core.Models;

namespace TimeTracker.DAL.Repositories
{
    public class VolunteerTransactionRepository : IVolTransaction
    {
        private DataLoadHelper _helper = new DataLoadHelper(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);

        #region Base Query

        private const string _baseQuery = @"
                            SELECT
	                            trans.TransactionId
	                            ,vols.VolunteerId
	                            ,trans.CreditAmount
	                            ,trans.TransactionNotes
                                ,transtypes.TransactionType
                                ,transtypes.TransactionTypeId
                                ,transtypes.MakeNegative
                                ,trans.CreateDateTime as TransactionDateTime
	                            ,vols.FullName as VolunteerName
                            FROM CreditTransactions trans
	                            INNER JOIN TransactionTypes transtypes
		                            ON trans.TransactionTypeId = transtypes.TransactionTypeId
	                            INNER JOIN Volunteers vols
		                            ON trans.VolunteerId = vols.VolunteerId"; 
        #endregion

        public VolCreditTransaction CreateTransaction(VolCreditTransaction transaction)
        {
            string sql = @"
                            INSERT INTO [dbo].[CreditTransactions]
                                (
                                    [TransactionTypeId]
		                            ,[VolunteerId]
		                            ,[TransactionNotes]
		                            ,[CreditAmount]
                                )
                            OUTPUT inserted.TransactionId
                            VALUES
                                (
		                            @TransactionTypeId
                                    ,@VolunteerId
		                            ,@TransactionNotes
		                            ,@CreditAmount
                                )";

            var parameters = new[]
            {
                new SqlParameter("@TransactionTypeId", transaction.TransactionType.Id),
                new SqlParameter("@VolunteerId", transaction.VolunteerId),
                new SqlParameter("@TransactionNotes", transaction.Notes ?? ""),
                new SqlParameter("@CreditAmount", transaction.CreditAmount)
            };

            var result = (Guid)_helper.ExecScalarSqlPullObject(sql, parameters);

            if (result != Guid.Empty)
                transaction.Id = result;

            return transaction;
        }

        public void DeleteTransaction(Guid transactionid)
        {
            string sql = @"DELETE FROM [dbo].[CreditTransactions] WHERE TransactionId = @TransactionId";

            var parameters = new[]
            {
                new SqlParameter("@TransactionId", transactionid)
            };

            var result = _helper.ExecNonQuery(sql, parameters);
        }

        public VolCreditTransaction GetTransactionById(Guid transactionId)
        {
            VolCreditTransaction transaction = new VolCreditTransaction();

            string sql = _baseQuery + @" WHERE TransactionId = @TransactionId";

            var parameters = new[]
            {
                new SqlParameter("@TransactionId", transactionId)
            };

            var result = _helper.ExecSqlPullDataTable(sql, parameters);

            if (Tools.DataTableHasRows(result))
                transaction = (from DataRow row in result.Rows select Maps.TransactionMaps.MapTransaction(row)).FirstOrDefault();

            return transaction;
        }

        public List<VolCreditTransaction> GetTransactionsByVolunteerId(Guid volunteerId)
        {
            List<VolCreditTransaction> transactions = new List<VolCreditTransaction>();

            string sql = _baseQuery + @" WHERE vols.VolunteerId = @VolunteerId";

            var parameters = new[]
            {
                new SqlParameter("@VolunteerId", volunteerId)
            };

            var result = _helper.ExecSqlPullDataTable(sql, parameters);

            if (Tools.DataTableHasRows(result))
                transactions = (from DataRow row in result.Rows select Maps.TransactionMaps.MapTransaction(row)).ToList();

            return transactions;
        }



        public void UpdateTransaction(VolCreditTransaction transaction)
        {
            string sql = @"
                            UPDATE [dbo].[CreditTransactions]
                            SET [TransactionTypeId] = @TransactionTypeId
	                            ,[TransactionNotes] = @Notes
	                            ,[CreditAmount] = @CreditAmount
                            WHERE TransactionId = @TransactionId";

            var parameters = new[]
            {
                new SqlParameter("@TransactionTypeId", transaction.TransactionType.Id),
                new SqlParameter("@Notes", transaction.Notes),
                new SqlParameter("@CreditAmount", transaction.CreditAmount),
                new SqlParameter("@TransactionId", transaction.Id)
            };

            var result = _helper.ExecNonQuery(sql, parameters);
        }
    }
}
