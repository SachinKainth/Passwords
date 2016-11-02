namespace Password.DAL
{
    public interface IPasswordDAL
    {
        UserPasswordRecord GetUser(string userName);
        void AddPassword(string userName, string password);
    }
}