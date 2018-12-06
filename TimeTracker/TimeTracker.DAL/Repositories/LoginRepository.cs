using FDCommonTools.SqlTools;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using TimeTracker.Core.Contracts;
using TimeTracker.Core.Models;

namespace TimeTracker.DAL.Repositories
{
    public class LoginRepository : ILogin
    {
        private DataLoadHelper _helper = new DataLoadHelper(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);

        public Administrator LogAdminIn(string userName, string password)
        {
            Administrator admin = new Administrator();

            string sql = @"
                            SELECT 
	                            admins.[AdministratorId]
	                            ,admins.[FirstName]
	                            ,admins.[LastName]
                                ,admins.[FirstName] + ' ' + admins.[LastName] as FullName
	                            ,admins.[EmailAddress]
	                            ,admins.[Username]
	                            ,admins.[PasswordSalt]
	                            ,admins.[PhoneNumber]
	                            ,admins.[Active]
                                ,admins.[CreditBalance]
                            FROM [TimeTracker].[dbo].[Administrators] admins
                            WHERE UserName = @UserName";

            var parameters = new[]
            {
                new SqlParameter("@Username", userName)
            };

            var result = _helper.ExecSqlPullDataTable(sql, parameters);

            if (Tools.DataTableHasRows(result))
                admin = (from DataRow row in result.Rows select Maps.AdministratorMaps.MapAdministrator(row)).FirstOrDefault();

            return admin;
        }
    }
}
