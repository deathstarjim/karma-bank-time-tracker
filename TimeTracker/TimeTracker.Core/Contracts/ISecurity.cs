namespace TimeTracker.Core.Contracts
{
    public interface ISecurity
    {
        /// <summary>
        /// Converts the plain string password to a Base64 string
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        string EncryptPassword(string password);

        /// <summary>
        /// Uses BCrypt to hash the password with automatically generated salt
        /// </summary>
        /// <param name="userPassword"></param>
        /// <returns></returns>
        string HashPassword(string userPassword);

        /// <summary>
        /// Uses BCrypt to hash the password with manually created salt
        /// </summary>
        /// <param name="userPassword"></param>
        /// <param name="salt"></param>
        /// <returns></returns>
        string HashPassword(string userPassword, string salt);

        /// <summary>
        /// Manually generates salt to be added to a hashed value
        /// </summary>
        /// <returns></returns>
        string GenerateSalt();

        /// <summary>
        /// Confirms the password submitted matches the hashed and encrypted password in the database
        /// </summary>
        /// <param name="submittedPassword"></param>
        /// <returns></returns>
        bool PasswordIsValid(string submittedPassword, string retrievedPassword);
    }
}
