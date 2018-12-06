using System;
using System.Collections.Generic;
using TimeTracker.Core.Models;

namespace TimeTracker.Core.Contracts
{
    public interface IVolTimePunch
    {
        /// <summary>
        /// Creates a volunteer time punch record with just the punch in time
        /// </summary>
        /// <param name="newPunch"></param>
        /// <returns></returns>
        Guid CreateTimePunchIn(VolTimePunch newPunch);

        /// <summary>
        /// Creates a volunteer time punch record with the punch in time and the punch out time
        /// </summary>
        /// <param name="punch"></param>
        /// <param name="creditValue"></param>
        /// <returns></returns>
        VolTimePunch CreateTimePunch(VolTimePunch punch, int creditValue);

        /// <summary>
        /// Updates a volunteer time punch's punch out time and re-calculates the credits earned for that punch
        /// </summary>
        /// <param name="punch"></param>
        /// <param name="creditValue"></param>
        void UpdateTimePunchOut(VolTimePunch punch, int creditValue);

        /// <summary>
        /// Updates both the punch in time and the punch out time and re-calculates the credits earned for that punch
        /// </summary>
        /// <param name="punch"></param>
        /// <param name="creditValue"></param>
        void UpdateTimePunch(VolTimePunch punch, int creditValue);

        /// <summary>
        /// Deletes a volunteer time punch record
        /// </summary>
        /// <param name="punchId"></param>
        void DeleteTimePunch(Guid punchId);

        /// <summary>
        /// Gets the volunteer time punch object by id
        /// </summary>
        /// <param name="timePunchId"></param>
        /// <returns></returns>
        VolTimePunch GetTimePunchById(Guid timePunchId);

        /// <summary>
        /// Get all volunteer time punch records
        /// </summary>
        /// <returns></returns>
        List<VolTimePunch> GetTimePunches();

        /// <summary>
        /// Gets all time punch records for a volunteer
        /// </summary>
        /// <param name="volunteerId"></param>
        /// <returns></returns>
        List<VolTimePunch> GetTimePunchesByVolunteerId(Guid volunteerId);

        /// <summary>
        /// Gets all the volunteer time punch records with no punch out time / date
        /// </summary>
        /// <returns></returns>
        List<VolTimePunch> GetOpenTimePunches();

        /// <summary>
        /// Checks to see if the volunteer is currently clocked in to an opportunity
        /// </summary>
        /// <param name="volunteerId"></param>
        /// <returns></returns>
        bool CheckVolunteerClockedIn(Guid volunteerId);
    }
}
