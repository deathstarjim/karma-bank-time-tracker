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
    public class TransactionTypesRepository : ITransactionType
    {
        private DataLoadHelper _helper = new DataLoadHelper(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);

        private const string _baseQuery = @"
                            SELECT
	                            transtypes.[TransactionTypeId]
	                            ,transtypes.[TransactionType]
                                ,transtypes.[MakeNegative]
                            FROM TransactionTypes transtypes";

        public TransactionType GetTransactionTypeById(Guid transactionTypeId)
        {
            TransactionType transactionType = new TransactionType();

            string sql = _baseQuery + @" WHERE transtypes.TransactionTypeId = @TransTypeId";

            var parameters = new[]
            {
                new SqlParameter("@TransTypeId", transactionTypeId)
            };

            var result = _helper.ExecSqlPullDataTable(sql, parameters);

            if (Tools.DataTableHasRows(result))
                transactionType = (from DataRow row in result.Rows select Maps.TransactionMaps.MapTransactionType(row)).FirstOrDefault();

            return transactionType;
        }

        public List<TransactionType> GetTransactionTypes()
        {
            List<TransactionType> types = new List<TransactionType>();

            var result = _helper.ExecSqlPullDataTable(_baseQuery);

            if (Tools.DataTableHasRows(result))
                types = (from DataRow row in result.Rows select Maps.TransactionMaps.MapTransactionType(row)).ToList();

            return types;
        }

        public TransactionType CreateTransactionType(TransactionType type)
        {
            string sql = @"
                            INSERT INTO [dbo].[TransactionTypes]
                                (
		                            [TransactionType]
		                            ,[MakeNegative]
                                )
                            OUTPUT inserted.TransactionTypeId
                            VALUES
                                (
		                            @TransactionType
		                            ,@MakeNegative
                                )";

            var parameters = new[]
            {
                new SqlParameter("@TransactionType", type.Name),
                new SqlParameter("@MakeNegative", type.MakeNegative.IsChecked)
            };

            var result = (Guid)_helper.ExecScalarSqlPullObject(sql, parameters);

            if (result != Guid.Empty)
                type.Id = result;

            return type;
        }

        public void UpdateTransactionType(TransactionType type)
        {
            string sql = @"
                            UPDATE [dbo].[TransactionTypes]
                            SET [TransactionType] = @TransactionType
                                ,[MakeNegative] = @MakeNegative
                            WHERE TransactionTypeId = @TransTypeId";

            var parameters = new[]
            {
                new SqlParameter("@TransactionType", type.Name),
                new SqlParameter("@MakeNegative", type.MakeNegative.IsChecked),
                new SqlParameter("@TransTypeId", type.Id)
            };

            var result = _helper.ExecNonQuery(sql, parameters);
        }

        public void DeleteTransactionType(Guid transactionTypeId)
        {
            string sql = @"DELETE FROM TransactionTypes WHERE TransactionTypeId = @TransTypeId";

            var parameters = new[]
            {
                new SqlParameter("@TransTypeId", transactionTypeId)
            };

            var result = _helper.ExecNonQuery(sql, parameters);
        }

        public void UpdateVolunteerTransactionTypes(Guid replacementTypeId, Guid deletedTypeId)
        {
            string sql = @"
                            UPDATE CreditTransactions
                            SET TransactionTypeId = @ReplacementTypeId
                            WHERE TransactionTypeId = @DeletedTypeId";

            var parameters = new[]
            {
                new SqlParameter("@ReplacementTypeId", replacementTypeId),
                new SqlParameter("@DeletedTypeId", deletedTypeId)
            };

            var result = _helper.ExecNonQuery(sql, parameters);
        }

        public void UpdateOrgAdminTransactionTypes(Guid replacementTypeId, Guid deletedTypeId)
        {
            string sql = @"
                            UPDATE OrgAdminCreditTransactions
                            SET TransactionTypeId = @ReplacementTypeId
                            WHERE TransactionTypeId = @DeletedTypeId";

            var parameters = new[]
            {
                new SqlParameter("@ReplacementTypeId", replacementTypeId),
                new SqlParameter("@DeletedTypeId", deletedTypeId)
            };

            var result = _helper.ExecSqlPullDataTable(sql, parameters);
        }
    }
}
