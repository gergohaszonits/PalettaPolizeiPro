using PalettaPolizeiPro.Data;
using PalettaPolizeiPro.Data.DataTransferObjects;
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
            Console.WriteLine("LoginService created");
        }
        public User? LogIn(UserCredentialsDTO cred)
        {
            if (cred is null || cred.Username is null || cred.Username == String.Empty || cred.Password is null || cred.Password == String.Empty)
            {
                throw new Exception("A belépéshez add meg az adatokat.");
            }
            var user = _userService.GetUserByUsername(cred.Username);

            if (user is null)
            {
                return null;
            }
            string hash = HashString(cred.Password);
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
