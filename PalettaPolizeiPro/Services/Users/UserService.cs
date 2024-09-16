using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using PalettaPolizeiPro.Data.Users;
using PalettaPolizeiPro.Database;
using System;

namespace PalettaPolizeiPro.Services.Users
{
    public class UserService : IUserService
    {
        public void AddUser(User user)
        {
            using (var context = new DatabaseContext())
            {
                context.Users.Add(user);
                context.SaveChanges();
            }
        }

        public User? Get(Func<User, bool> predicate)
        {
            using (var context = new DatabaseContext())
            {
                return context.Users.FirstOrDefault(predicate);
            }
        }

        public List<User> GetAll()
        {
            using (var context = new DatabaseContext())
            {
                return context.Users.ToList();
            }
        }

        public List<User> GetWhere(Func<User, bool> predicate)
        {
            using (var context = new DatabaseContext())
            {
                return context.Users.Where(predicate).ToList();
            }
        }

        public void ModifyUser(User user)
        {
            using (var context = new DatabaseContext())
            {
                context.Users.Update(user);
                context.SaveChanges();
            }
        }

        public void RemoveUser(User user)
        {
            using (var context = new DatabaseContext())
            {
                context.Users.Remove(user);
                context.SaveChanges();
            }
        }
    }
}