using System;
using System.Collections.Generic;
using TimeTracker.Core.Models;

namespace TimeTracker.Core.Contracts
{
    public interface IAdminTimePunch
    {
        List<AdminTimePunch> OpenTimePunches();

        AdminTimePunch PunchAdminIn(AdminTimePunch newPunch);

        AdminTimePunch GetTimePunchById(Guid punchId);

        void PunchAdminOut(AdminTimePunch punch);

        List<AdminTimePunch> GetTimePunchesByAdminId(Guid administratorId);

        void UpdateTimePunch(AdminTimePunch punch);

        void DeleteTimePunch(Guid timePunchId, Guid administratorId);

        bool CheckAdminClockedIn(Guid adminId);
    }
}
