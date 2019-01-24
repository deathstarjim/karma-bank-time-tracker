using FDCommonTools.SqlTools;
using System.Data;
using TimeTracker.Core.Models;

namespace TimeTracker.DAL.Maps
{
    public class AdministratorMaps
    {
        public static Administrator MapAdministrator(DataRow row)
        {
            Administrator admin = new Administrator();

            if(row != null)
            {
                admin.Id = DataFieldHelper.GetUniqueIdentifier(row, "AdministratorId");
                admin.OrganizationId = DataFieldHelper.GetUniqueIdentifier(row, "OrganizationId");
                admin.FirstName = DataFieldHelper.GetString(row, "FirstName");
                admin.LastName = DataFieldHelper.GetString(row, "LastName");
                admin.FullName = DataFieldHelper.GetString(row, "FullName");
                admin.EmailAddress = DataFieldHelper.GetString(row, "EmailAddress");
                admin.UserName = DataFieldHelper.GetString(row, "UserName");
                admin.Password = DataFieldHelper.GetString(row, "UserPassword");
                admin.PasswordSalt = DataFieldHelper.GetString(row, "PasswordSalt");
                admin.PhoneNumber = DataFieldHelper.GetString(row, "PhoneNumber");
                admin.Active = DataFieldHelper.GetBit(row, "Active");
                admin.CreditBalance = DataFieldHelper.GetDecimal(row, "CreditBalance");
            }

            return admin;
        }

        public static AdminTimePunch MapAdminTimePunch(DataRow row)
        {
            AdminTimePunch punch = new AdminTimePunch();

            if(row != null)
            {
                punch.Id = DataFieldHelper.GetUniqueIdentifier(row, "AdminTimePunchId");
                punch.AdministratorId = DataFieldHelper.GetUniqueIdentifier(row, "AdministratorId");
                punch.FullName = DataFieldHelper.GetString(row, "FullName");
                punch.PunchInDateTime = DataFieldHelper.GetDateTime(row, "PunchInDateTime");
                punch.PunchOutDateTime = DataFieldHelper.GetDateTime(row, "PunchOutDateTime");
                punch.CreditsEarned = DataFieldHelper.GetDecimal(row, "CreditsEarned");
            }

            return punch;
        }

        public static AdminCreditTransaction MapAdminTransaction(DataRow row)
        {
            AdminCreditTransaction transaction = new AdminCreditTransaction();

            if(row != null)
            {
                transaction.Id = DataFieldHelper.GetUniqueIdentifier(row, "TransactionId");
                transaction.AdministratorId = DataFieldHelper.GetUniqueIdentifier(row, "AdministratorId");
                transaction.CreditAmount = DataFieldHelper.GetDecimal(row, "CreditAmount");
                transaction.Notes = DataFieldHelper.GetString(row, "TransactionNotes");
                transaction.TransactionType = TransactionMaps.MapTransactionType(row);
                transaction.CreateDateTime = DataFieldHelper.GetDateTime(row, "TransactionDateTime");
                transaction.AdministratorName = DataFieldHelper.GetString(row, "AdministratorName");
            }

            return transaction;
        }
    }
}
