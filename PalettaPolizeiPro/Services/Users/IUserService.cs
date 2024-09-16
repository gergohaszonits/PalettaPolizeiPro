using PalettaPolizeiPro.Data.Users;

namespace PalettaPolizeiPro.Services.Users
{
    public interface IUserService
    {
        void AddUser(User user);
        List<User> GetAll();
        User? Get(Func<User, bool> predicate);
        List<User> GetWhere(Func<User, bool> predicate);
        void ModifyUser(User user);
        void RemoveUser(User user);      
    }
}