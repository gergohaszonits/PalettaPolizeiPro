using PalettaPolizeiPro.Data;

namespace PalettaPolizeiPro.Services
{
    public interface IUserService
    {
        void AddUser(User user);
        Task AddUserAsync(User user);
        List<User> GetUsers();
        Task<List<User>> GetUsersAsync();
        void UpdateUser(User user);
        Task UpdateUserAsync(User user);
        User? GetUserByUsername(string username);
        List<User> GetUsersByAuth(Role auth);

        Task<User?> GetUserByUsernameAsync(string username);      
    }
}