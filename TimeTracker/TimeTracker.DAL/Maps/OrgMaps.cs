using FDCommonTools.SqlTools;
using System.Data;
using TimeTracker.Core.Models;

namespace TimeTracker.DAL.Maps
{
    public class OrgMaps
    {
        public static Organization MapOrg(DataRow row)
        {
            Organization org = new Organization();

            if(row != null)
            {
                org.Id = DataFieldHelper.GetUniqueIdentifier(row, "OrganizationId");
                org.Name = DataFieldHelper.GetString(row, "OrganizationName");
                org.Description = DataFieldHelper.GetString(row, "OrgDescription");
                org.WebsiteUrl = DataFieldHelper.GetString(row, "WebsiteUrl");
                org.Subdomain = DataFieldHelper.GetString(row, "Subdomain");
            }

            return org;
        }
    }
}
