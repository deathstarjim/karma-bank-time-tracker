using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeTracker.Core.Models;

namespace TimeTracker.Core.Contracts
{
    public interface ISystemRole
    {
        List<SystemRole> GetSystemRoles();
    }
}
