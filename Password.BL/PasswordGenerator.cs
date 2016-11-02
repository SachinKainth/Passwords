using System;
using Password.DAL;

namespace Password.BL
{
    public class PasswordGenerator
    {
        private readonly IPasswordDAL _passwordDAL;
        private static int _passwordValidityLengthSeconds = 30;

        public PasswordGenerator(IPasswordDAL passwordDAL)
        {
            _passwordDAL = passwordDAL;
        }

        public PasswordGenerator(IPasswordDAL passwordDAL, int passwordValidityLengthSeconds)
            : this(passwordDAL)
        {
            _passwordValidityLengthSeconds = passwordValidityLengthSeconds;
        }

        public string Generate(string userName)
        {
            PerformCommonUserNameValidationChecks(userName);

            var password = Guid.NewGuid().ToString();

            _passwordDAL.AddPassword(userName, password);

            return password;
        }

        public bool Verify(string userName, string password)
        {
            var currentTimeStamp = DateTime.UtcNow;

            var userPasswordRecord = PerformCommonUserNameValidationChecks(userName);

            PerformPasswordValidationChecks(password);

            return userPasswordRecord.Password == password &&
                userPasswordRecord.TimeStamp.AddSeconds(_passwordValidityLengthSeconds) > currentTimeStamp;
        }

        private UserPasswordRecord PerformCommonUserNameValidationChecks(string userName)
        {
            if (string.IsNullOrWhiteSpace(userName))
            {
                throw new BadUserNameException("User name '" + userName + "' is not valid.");
            }

            var userPasswordRecord = _passwordDAL.GetUser(userName);
            if (userPasswordRecord == null)
            {
                throw new BadUserNameException("User name '" + userName + "' does not exist.");
            }

            return userPasswordRecord;
        }

        private static void PerformPasswordValidationChecks(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                throw new BadPasswordException("Password '" + password + "' is not valid.");
            }

            Guid guid;
            if (!Guid.TryParse(password, out guid))
            {
                throw new BadPasswordException("Password '" + password + "' must be a valid Guid.");
            }
        }
    }
}