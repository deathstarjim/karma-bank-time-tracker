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
    public class AdministratorTransactionRepository : IAdminTransaction
    {
        private DataLoadHelper _helper = new DataLoadHelper(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);

        #region
        private const string _baseQuery = @"
                                            SELECT
	                                            trans.TransactionId
	                                            ,admins.AdministratorId
	                                            ,trans.CreditAmount
	                                            ,trans.TransactionNotes
                                                ,transtypes.TransactionType
                                                ,transtypes.TransactionTypeId
                                                ,transtypes.MakeNegative
                                                ,trans.CreateDateTime as TransactionDateTime
	                                            ,admins.FullName as AdministratorName
                                            FROM OrgAdminCreditTransactions trans
	                                            INNER JOIN TransactionTypes transtypes
		                                            ON trans.TransactionTypeId = transtypes.TransactionTypeId
	                                            INNER JOIN Administrators admins
		                                            ON trans.AdministratorId = admins.AdministratorId";
        #endregion

        public AdminCreditTransaction CreateTransaction(AdminCreditTransaction transaction)
        {
            string sql = @"
                            INSERT INTO [dbo].[OrgAdminCreditTransactions]
	                            (
		                            [AdministratorId]
		                            ,[TransactionTypeId]
		                            ,[TransactionNotes]
		                            ,[CreditAmount]
	                            )
	                            OUTPUT inserted.TransactionId
	                            VALUES
	                            (
		                            @AdministratorId
		                            ,@TransactionTypeId
		                            ,@TransactionNotes
		                            ,@CreditAmount
	                            )";

            var parameters = new[]
            {
                new SqlParameter("@AdministratorId", transaction.AdministratorId),
                new SqlParameter("@TransactionTypeId", transaction.TransactionType.Id),
                new SqlParameter("@TransactionNotes", transaction.Notes),
                new SqlParameter("@CreditAmount", transaction.CreditAmount)
            };

            var result = (Guid)_helper.ExecScalarSqlPullObject(sql, parameters);

            if (result != Guid.Empty)
                transaction.Id = result;

            return transaction;
        }

        public List<AdminCreditTransaction> GetTransactionsByAdminId(Guid administratorId)
        {
            List<AdminCreditTransaction> transactions = new List<AdminCreditTransaction>();

            string sql = _baseQuery + @" WHERE admins.AdministratorId = @AdminId";

            var parameters = new[]
            {
                new SqlParameter("@AdminId", administratorId)
            };

            var result = _helper.ExecSqlPullDataTable(sql, parameters);

            if (Tools.DataTableHasRows(result))
                transactions = (from DataRow row in result.Rows select Maps.AdministratorMaps.MapAdminTransaction(row)).ToList();

            return transactions;
        }

        public AdminCreditTransaction GetTransactionById(Guid transactionId)
        {
            AdminCreditTransaction transaction = new AdminCreditTransaction();

            string sql = _baseQuery + @" WHERE TransactionId = @TransactionId";

            var parameters = new[]
            {
                new SqlParameter("@TransactionId", transactionId)
            };

            var result = _helper.ExecSqlPullDataTable(sql, parameters);

            if (Tools.DataTableHasRows(result))
                transaction = (from DataRow row in result.Rows select Maps.AdministratorMaps.MapAdminTransaction(row)).FirstOrDefault();

            return transaction;
        }

        public void UpdateTransaction(AdminCreditTransaction transaction)
        {
            string sql = @"
                            UPDATE [dbo].[OrgAdminCreditTransactions]
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

        public void DeleteTransaction(Guid transactionId)
        {
            string sql = @"DELETE FROM [dbo].[OrgAdminCreditTransactions] WHERE TransactionId = @TransactionId";

            var parameters = new[]
            {
                new SqlParameter("@TransactionId", transactionId)
            };

            var result = _helper.ExecNonQuery(sql, parameters);
        }
    }
}
