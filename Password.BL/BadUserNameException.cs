using System;

namespace Password.BL
{
    public class BadUserNameException : Exception
    {
        public BadUserNameException(string message) : base(message)
        {
        }
    }
}