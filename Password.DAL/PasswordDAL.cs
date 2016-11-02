using System.Collections.Generic;
using System.Linq;

namespace Password.DAL
{
    public class PasswordDAL : IPasswordDAL
    {
        private readonly IList<UserPasswordRecord> _users = new List<UserPasswordRecord>();

        public UserPasswordRecord GetUser(string userName)
        {
            return _users.SingleOrDefault(r => r.UserName.Equals(userName));
        }

        public void AddUser(string userName)
        {
            var user = _users.SingleOrDefault(r => r.UserName.Equals(userName));
            if (user == null)
            {
                _users.Add(new UserPasswordRecord(userName));
            }
        }

        public void AddPassword(string userName, string password)
        {
            var user = _users.SingleOrDefault(r => r.UserName.Equals(userName));
            user?.Update(password);
        }
    }
}