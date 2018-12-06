using System;
using System.Collections.Generic;
using TimeTracker.Core.Models;

namespace TimeTracker.Core.Contracts
{
    public interface IVolunteer
    {
        /// <summary>
        /// Creates a new volunteer record
        /// </summary>
        /// <param name="newVolunteer"></param>
        /// <returns></returns>
        Guid CreateVolunteer(Volunteer newVolunteer);

        /// <summary>
        /// Gets a list of all volunteers
        /// </summary>
        /// <returns></returns>
        List<Volunteer> GetVolunteers();

        /// <summary>
        /// Gets a volunteer by their id
        /// </summary>
        /// <param name="volunteerId"></param>
        /// <returns></returns>
        Volunteer GetVolunteerById(Guid volunteerId);

        /// <summary>
        /// Does a wildcard search on the volunteer's name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        List<Volunteer> SearchVolunteersByName(string name);

        /// <summary>
        /// Updates the credit balance for a volunteer by summing the time punch records in the database
        /// </summary>
        /// <param name="volunteerId"></param>
        void UpdateCreditBalance(Guid volunteerId);
    }
}
