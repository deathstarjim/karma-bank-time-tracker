using FDCommonTools.SqlTools;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using TimeTracker.Core.Contracts;
using TimeTracker.Core.Models;
using System.Linq;

namespace TimeTracker.DAL.Repositories
{
    public class OrganizationRepository : IOrganization
    {
        private DataLoadHelper _helper = new DataLoadHelper(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);

        public Organization CreateOrganization(Organization newOrg)
        {
            string sql = @"
                            INSERT INTO [dbo].[Organizations]
	                            (
		                            [OrganizationName]
		                            ,[WebsiteUrl]
                                    ,[EmailAddress]
                                    ,[TaxExemptionFile]
                                    ,[Subdomain]
	                            )
                            OUTPUT inserted.OrganizationId
                            VALUES
	                            (
		                            @OrganizationName
		                            ,@WebsiteUrl
                                    ,@EmailAddress
                                    ,@TaxExemptionFile
                                    ,@Subdomain
	                            )";

            var parameters = new[]
            {
                new SqlParameter("@OrganizationName", newOrg.Name),
                new SqlParameter("@WebsiteUrl", newOrg.WebsiteUrl),
                new SqlParameter("@EmailAddress", newOrg.EmailAddress ?? ""),
                new SqlParameter("@Subdomain", newOrg.Subdomain),
                new SqlParameter("@TaxExemptionFile", newOrg.TaxExemptionFile)
            };

            var result = (Guid)_helper.ExecScalarSqlPullObject(sql, parameters);

            if (result != Guid.Empty)
                newOrg.Id = result;

            return newOrg;
        }

        public Organization GetOrgById(Guid orgId)
        {
            Organization org = new Organization();

            string sql = @"
                            SELECT 
	                            [OrganizationId]
                                ,[OrganizationName]
                                ,[WebsiteUrl]
                                ,[OrgDescription]
                                ,[Subdomain]
                            FROM [dbo].[Organizations] orgs
                            WHERE orgs.OrganizationId = @OrgId";

            var parameters = new[]
            {
                new SqlParameter("@OrgId", orgId)
            };

            var result = _helper.ExecSqlPullDataTable(sql, parameters);

            if (Tools.DataTableHasRows(result))
                org = (from DataRow row in result.Rows select Maps.OrgMaps.MapOrg(row)).FirstOrDefault();

            return org;
        }

        public void UpdateOrganization(Organization org)
        {
            string sql = @"
                            UPDATE [dbo].[Organizations]
                            SET [OrganizationName] = @OrganizationName
                                ,[WebsiteUrl] = @WebsiteUrl
                                ,[OrgDescription] = @OrgDescription
                            WHERE OrganizationId = @OrgId";

            var parameters = new[]
            {
                new SqlParameter("@OrganizationName", org.Name),
                new SqlParameter("OrgDescription", org.Description),
                new SqlParameter("@WebsiteUrl", org.WebsiteUrl),
                new SqlParameter("@OrgId", org.Id)
            };

            var result = _helper.ExecNonQuery(sql, parameters);
        }
    }
}
