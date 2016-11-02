using System;

namespace Password.DAL
{
    public class UserPasswordRecord
    {
        public virtual string UserName { get; private set; }
        public virtual string Password { get; private set; }
        public virtual DateTime TimeStamp { get; private set; }

        public UserPasswordRecord()
        {
        }

        public UserPasswordRecord(string userName)
        {
            UserName = userName;
        }

        public void Update(string password)
        {
            Password = password;
            TimeStamp = DateTime.UtcNow;
        }
    }
}