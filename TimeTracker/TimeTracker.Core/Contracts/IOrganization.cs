using System;
using TimeTracker.Core.Models;

namespace TimeTracker.Core.Contracts
{
    public interface IOrganization
    {
        Organization GetOrgById(Guid orgId);

        Organization CreateOrganization(Organization newOrg);

        void UpdateOrganization(Organization org);
    }
}
