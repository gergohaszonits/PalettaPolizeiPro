using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using PalettaPolizeiPro.Data;
using PalettaPolizeiPro.Database;

namespace PalettaPolizeiPro.Services
{
    public class UserService : DatabaseContext, IUserService
    {
        public UserService()
        {
            Console.WriteLine("UserService created");

        }
        public List<User> GetUsers()
        {
            return Users.AsNoTracking().ToList();
        }
        public Task<List<User>> GetUsersAsync()
        {
            return Task.Run(() => GetUsers());
        }
        public User? GetUserByUsername(string username)
        {
            var item =  Users.AsNoTracking().FirstOrDefault(x => x.Username == username);
            return item;

        }
        public List<User> GetUsersByAuth(Authorization auth)
        {
            var users = Users.AsNoTracking().Where(x => x.Authorizations.Contains(auth)).ToList();
            return users;

        }
        public Task<User?> GetUserByUsernameAsync(string username)
        {
            return Users.AsNoTracking().FirstOrDefaultAsync(x => x.Username == username);
        }
        public void AddUser(User user)
        {
            Users.Add(user);
        }
        public Task AddUserAsync(User user)
        {
            return Task.Run(() => AddUser(user));
        }
        public void UpdateUser(User user)
        {
            if (user.Id == 0)
            {
                throw new Exception("This user does not exist");
            }
            var existingUser = Users.Find(user.Id);
            if (existingUser != null)
            {
                Entry(existingUser).CurrentValues.SetValues(user);
                SaveChanges();
            }
        }
        public Task UpdateUserAsync(User user)
        {
            return Task.Run(() => UpdateUser(user));
        }
        public void Save()
        {
            SaveChanges();
        }
        public async Task SaveAsync()
        {
            await SaveChangesAsync();
        }
    }
}
