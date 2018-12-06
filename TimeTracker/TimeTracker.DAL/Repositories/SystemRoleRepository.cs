using FDCommonTools.SqlTools;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using TimeTracker.Core.Contracts;
using TimeTracker.Core.Models;

namespace TimeTracker.DAL.Repositories
{
    public class SystemRoleRepository : ISystemRole
    {
        private DataLoadHelper _helper = new DataLoadHelper(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);

        public List<SystemRole> GetSystemRoles()
        {
            List<SystemRole> roles = new List<SystemRole>();

            string sql = @"
                            SELECT 
	                            [SystemRoleId]
                               ,[RoleName]
                            FROM [dbo].[SystemRoles]";

            var result = _helper.ExecSqlPullDataTable(sql);

            if (result != null)
                roles = (from DataRow row in result.Rows select Maps.SystemRoleMaps.MapSystemRole(row)).ToList();

            return roles;
        }
    }
}
