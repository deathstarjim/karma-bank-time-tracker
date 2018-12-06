using System;
using System.Collections.Generic;
using TimeTracker.Core.Models;

namespace TimeTracker.Core.Contracts
{
    public interface IAdministrator
    {
        /// <summary>
        /// Creates a new administrator account
        /// </summary>
        /// <param name="newAdministrator"></param>
        /// <returns></returns>
        Administrator CreateAdministrator(Administrator newAdministrator);

        /// <summary>
        /// Gets the Administrator by user name only
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        Administrator GetAdministratorByUserName(string userName);

        /// <summary>
        /// Gets a list of all administrators in the system
        /// </summary>
        /// <returns></returns>
        List<Administrator> GetAdministrators();

        void UpdateCreditBalance(Guid administratorId);
    }
}
