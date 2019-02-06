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
    public class AdministratorsRepository : IAdministrator
    {
        private DataLoadHelper _helper = new DataLoadHelper(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);

        #region "Base Query"
        private readonly string _baseQuery = @"
                            SELECT 
	                            admins.[AdministratorId]
	                            ,admins.[FirstName]
	                            ,admins.[LastName]
                                ,admins.[FirstName] + ' ' + admins.[LastName] as FullName
	                            ,admins.[EmailAddress]
	                            ,admins.[Username]
                                ,admins.[UserPassword]
	                            ,admins.[PasswordSalt]
	                            ,admins.[PhoneNumber]
	                            ,admins.[Active]
                                ,admins.[CreditBalance]
                            FROM [Administrators] admins";
        #endregion

        public Administrator CreateAdministrator(Administrator newAdministrator)
        {
            string sql = @"
                            INSERT INTO [dbo].[Administrators]
		                            (
			                            [SystemRoleId]
			                            ,[FirstName]
			                            ,[LastName]
                                        ,[FullName]
			                            ,[EmailAddress]
			                            ,[Username]
			                            ,[UserPassword]
                                        ,[PasswordSalt]
			                            ,[PhoneNumber]
		                            )
                            OUTPUT inserted.AdministratorId
                            VALUES
		                            (
			                            @SystemRoleId
			                            ,@FirstName
			                            ,@LastName
                                        ,@FullName
			                            ,@EmailAddress
			                            ,@Username
			                            ,@UserPassword
                                        ,@PasswordSalt
			                            ,@PhoneNumber
		                            )";

            var parameters = new[]
            {
                new SqlParameter("@SystemRoleId", newAdministrator.Role.Id),
                new SqlParameter("@FirstName", newAdministrator.FirstName),
                new SqlParameter("@LastName", newAdministrator.LastName),
                new SqlParameter("@FullName", newAdministrator.FirstName + ' ' + newAdministrator.LastName),
                new SqlParameter("@EmailAddress", newAdministrator.Email),
                new SqlParameter("@UserName", newAdministrator.UserName),
                new SqlParameter("@UserPassword", newAdministrator.Password),
                new SqlParameter("@PasswordSalt", newAdministrator.PasswordSalt),
                new SqlParameter("@PhoneNumber", newAdministrator.PhoneNumber)
            };

            var result = (Guid)_helper.ExecScalarSqlPullObject(sql, parameters);

            if (result != Guid.Empty)
                newAdministrator.Id = result;

            return newAdministrator;
        }

        public Administrator GetAdministratorByUserName(string userName)
        {
            Administrator admin = new Administrator();

            string sql = _baseQuery + @" WHERE admins.UserName = @UserName";

            var parameters = new[]
            {
                new SqlParameter("@Username", userName)
            };

            var result = _helper.ExecSqlPullDataTable(sql, parameters);

            if (Tools.DataTableHasRows(result))
                admin = (from DataRow row in result.Rows select Maps.AdministratorMaps.MapAdministrator(row)).FirstOrDefault();

            return admin;
        }

        public List<Administrator> GetAdministrators()
        {
            List<Administrator> admins = new List<Administrator>();

            string sql = _baseQuery;

            var result = _helper.ExecSqlPullDataTable(sql);

            if (Tools.DataTableHasRows(result))
                admins = (from DataRow row in result.Rows select Maps.AdministratorMaps.MapAdministrator(row)).ToList();

            return admins;
        }

        public void UpdateCreditBalance(Guid administratorId)
        {
            string sql = @"
                            UPDATE admins
                            SET CreditBalance = ISNULL(CreditSum.TotalCredits, 0) + ISNULL(CreditTrans.TotalTransCredits, 0)
                            FROM Administrators admins
	                            LEFT OUTER JOIN 
	                            (
		                            SELECT
			                            punches.AdministratorId
			                            ,SUM(punches.CreditsEarned) as TotalCredits
		                            FROM OrgAdminTimePunches punches
		                            WHERE punches.AdministratorId = @AdminId
		                            GROUP BY punches.AdministratorId
	                            ) as CreditSum
		                            ON admins.AdministratorId = CreditSum.AdministratorId
	                            LEFT OUTER JOIN
	                            (
		                            SELECT
			                            trans.AdministratorId
			                            ,SUM(trans.CreditAmount) as TotalTransCredits
		                            FROM OrgAdminCreditTransactions trans
		                            WHERE trans.AdministratorId = @AdminId
		                            GROUP BY trans.AdministratorId
	                            ) as CreditTrans
		                            ON CreditTrans.AdministratorId = admins.AdministratorId
                            WHERE admins.AdministratorId = @AdminId";

            var parameters = new[]
            {
                new SqlParameter("@AdminId", administratorId)
            };

            var result = _helper.ExecNonQuery(sql, parameters);
        }

        public Administrator GetAdministratorById(Guid administratorId)
        {
            Administrator admin = new Administrator();

            string sql = _baseQuery + @" WHERE admins.AdministratorId = @AdminId";

            var parameters = new[]
            {
                new SqlParameter("@AdminId", administratorId)
            };

            var result = _helper.ExecSqlPullDataTable(sql, parameters);

            if (Tools.DataTableHasRows(result))
                admin = (from DataRow row in result.Rows select Maps.AdministratorMaps.MapAdministrator(row)).FirstOrDefault();

            return admin;
        }

        public void UpdateAdministratorInformation(Administrator admin)
        {
            string sql = @"
                            UPDATE [dbo].[Administrators]
                            SET [FirstName] = @FirstName
	                            ,[LastName] = @LastName
                                ,[FullName] = @FirstName + ' ' + @LastName
	                            ,[EmailAddress] = @EmailAddress
	                            ,[Username] = @Username
	                            ,[PhoneNumber] = @PhoneNumber
	                            ,[Active] = @Active
                            WHERE AdministratorId = @AdminId";

            var parameters = new[]
            {
                new SqlParameter("@FirstName", admin.FirstName),
                new SqlParameter("@LastName", admin.LastName),
                new SqlParameter("@EmailAddress", admin.EmailAddress),
                new SqlParameter("@Username", admin.UserName),
                new SqlParameter("@PhoneNumber", admin.PhoneNumber),
                new SqlParameter("@Active", admin.Active),
                new SqlParameter("@AdminId", admin.Id)
            };

            var result = _helper.ExecNonQuery(sql, parameters);
        }

        public void UpdateAdministratorPassword(Administrator admin)
        {
            string sql = @"
                            UPDATE [dbo].[Administrators]
                            SET [UserPassword] = @Password
	                            ,[PasswordSalt] = @Salt
                            WHERE AdministratorId = @AdminId";

            var parameters = new[]
            {
                new SqlParameter("@Password", admin.Password),
                new SqlParameter("@Salt", admin.PasswordSalt),
                new SqlParameter("@AdminId", admin.Id)
            };

            var result = _helper.ExecNonQuery(sql, parameters);
        }
    }
}
