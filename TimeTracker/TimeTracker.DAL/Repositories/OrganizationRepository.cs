using FDCommonTools.SqlTools;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeTracker.Core.Contracts;
using TimeTracker.Core.Models;

namespace TimeTracker.DAL.Repositories
{
    public class OrganizationRepository : IOrganization
    {
        private DataLoadHelper _helper = new DataLoadHelper(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);

        public Organization CreateOrganization(Organization newOrg)
        {
            throw new NotImplementedException();
        }

        public Organization GetOrgById(Guid orgId)
        {
            throw new NotImplementedException();
        }

        public void UpdateOrganization(Organization org)
        {
            throw new NotImplementedException();
        }
    }
}
