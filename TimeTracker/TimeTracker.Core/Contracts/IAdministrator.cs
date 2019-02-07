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
        /// Gets the administrator record by the Id
        /// </summary>
        /// <param name="administratorId"></param>
        /// <returns></returns>
        Administrator GetAdministratorById(Guid administratorId);

        /// <summary>
        /// Gets a list of all administrators in the system
        /// </summary>
        /// <returns></returns>
        List<Administrator> GetAdministrators();

        /// <summary>
        /// Updates the administrator information record including the password.
        /// </summary>
        /// <param name="admin"></param>
        void UpdateAdministratorInformation(Administrator admin);

        /// <summary>
        /// Updates the Administrator objects password and password salt
        /// </summary>
        /// <param name="admin"></param>
        void UpdateAdministratorPassword(Administrator admin);

        void UpdateCreditBalance(Guid administratorId);
    }
}
