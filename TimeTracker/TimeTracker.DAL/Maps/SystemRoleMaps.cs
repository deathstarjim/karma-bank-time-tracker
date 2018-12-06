using FDCommonTools.SqlTools;
using System.Data;
using TimeTracker.Core.Models;

namespace TimeTracker.DAL.Maps
{
    public class SystemRoleMaps
    {
        public static SystemRole MapSystemRole(DataRow row)
        {
            SystemRole role = new SystemRole();

            if(row != null)
            {
                role.Id = DataFieldHelper.GetUniqueIdentifier(row, "SystemRoleId");
                role.Name = DataFieldHelper.GetString(row, "RoleName");
            }

            return role;
        }
    }
}
