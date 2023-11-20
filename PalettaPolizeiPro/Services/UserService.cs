using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using PalettaPolizeiPro.Data;
using PalettaPolizeiPro.Database;

namespace PalettaPolizeiPro.Services
{
    public class UserService
    {
        private DatabaseContext _database;
        public UserService(DatabaseContext context)
        {
            _database = context;
        }
        public List<User> GetUsers()
        {
            return _database.Users.AsNoTracking().ToList();
        }
        public void AddUser(User user)
        {
            _database.Users.Add(user);
        }
        public void UpdateUser(User user)
        {
            if (user.Id == 0)
            {
                throw new Exception("This user does not exist");
            }
            var existingUser = _database.Users.Find(user.Id);
            if (existingUser != null)
            {
                _database.Entry(existingUser).CurrentValues.SetValues(user);
                _database.SaveChanges();
            }
        }
        public void Save()
        {
            _database.SaveChanges();
        }
        public async Task SaveAsync()
        {
           await _database.SaveChangesAsync();
        }
    }
}
