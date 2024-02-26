using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using PalettaPolizeiPro.Data;
using PalettaPolizeiPro.Database;

namespace PalettaPolizeiPro.Services
{
    public class UserService : IUserService
    {
        public UserService()
        {

        }
        public List<User> GetUsers()
        {
            using (var context = new DatabaseContext())
            {
                return context.Users.AsNoTracking().ToList();
            }
        }
        public Task<List<User>> GetUsersAsync()
        {
            return Task.Run(() => GetUsers());
        }
        public User? GetUserByUsername(string username)
        {
            using (var context = new DatabaseContext())
            {
                var item = context.Users.AsNoTracking().FirstOrDefault(x => x.Username == username);
                return item;
            }

        }
        public List<User> GetUsersByAuth(Authorization auth)
        {
            using (var context = new DatabaseContext())
            {
                var users = context.Users.AsNoTracking().Where(x => x.Authorizations.Contains(auth)).ToList();
                return users;
            }

        }
        public Task<User?> GetUserByUsernameAsync(string username)
        {
            using (var context = new DatabaseContext())
            {
                return context.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Username == username);
            }
        }
        public void AddUser(User user)
        {
            using (var context = new DatabaseContext())
            {
                context.Users.Add(user);
            }
        }
        public Task AddUserAsync(User user)
        {
            return Task.Run(() => AddUser(user));
        }
        public void UpdateUser(User user)
        {
            using (var context = new DatabaseContext())
            {
                if (user.Id == 0)
                {
                    throw new Exception("This user does not exist");
                }
                var existingUser = context.Users.Find(user.Id);
                if (existingUser != null)
                {
                    context.Entry(existingUser).CurrentValues.SetValues(user);
                    context.SaveChanges();
                }
            }
        }
        public Task UpdateUserAsync(User user)
        {
            return Task.Run(() => UpdateUser(user));
        }
    }
}