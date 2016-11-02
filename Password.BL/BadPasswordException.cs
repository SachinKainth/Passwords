using System;

namespace Password.BL
{
    public class BadPasswordException : Exception
    {
        public BadPasswordException(string message) : base(message)
        {
        }
    }
}