using PalettaPolizeiPro.Data;
using PalettaPolizeiPro.Database;
using System.Security.Cryptography;
using System.Text;

namespace PalettaPolizeiPro.Services
{
    public class LoginService : DatabaseContext, ILoginService
    {
        private IUserService _userService;

        public LoginService(IUserService userService)
        {
            _userService = userService;
        }
        public User? LogIn(string username, string password)
        {
            var user = _userService.GetUserByUsername(username);
            if (user is null) { return null; }
            string hash = HashString(password);
            return hash != user.Password ? null : user;
        }
        private static string HashString(String value)
        {
            StringBuilder Sb = new StringBuilder();
            using (SHA256 hash = SHA256.Create())
            {
                Byte[] result = hash.ComputeHash(Encoding.UTF8.GetBytes(value));
                foreach (Byte b in result)
                {
                    Sb.Append(b.ToString("x2"));
                }
            }
            return Sb.ToString();
        }
        public void LogOut(User user)
        {
            throw new NotImplementedException();
        }
    }
}
