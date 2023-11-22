using PalettaPolizeiPro.Data;
using PalettaPolizeiPro.Database;
using System.Reflection.Metadata.Ecma335;
using System.Security.Cryptography;
using System.Text;

namespace PalettaPolizeiPro.Services
{
    public class LoginService : DatabaseContext
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
                Encoding enc = Encoding.UTF8;
                Byte[] result = hash.ComputeHash(enc.GetBytes(value));

                foreach (Byte b in result)
                {
                    Sb.Append(b.ToString("x2"));
                }
            }
            return Sb.ToString();
        }
    }
}
