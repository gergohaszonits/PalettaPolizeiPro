using PalettaPolizeiPro.Data;
using PalettaPolizeiPro.Data.DataTransferObjects;
using PalettaPolizeiPro.Database;
using System.Security.Cryptography;
using System.Text;

namespace PalettaPolizeiPro.Services
{
    public class LoginService :  ILoginService
    {
        private IUserService _userService;

        public LoginService(IUserService userService)
        {
            _userService = userService;
            Console.WriteLine("LoginService created");
        }
        public User? LogIn(UserCredentialsDTO cred)
        {
            if (cred is null) 
            {
                LogService.Log("UserCredentialsDTO null értéket kapott",LogLevel.Warning);
                return null;
            }
            if (cred is null || cred.Username is null || cred.Username == String.Empty || cred.Password is null || cred.Password == String.Empty)
            {
                throw new Exception("A belépéshez add meg az adatokat.");
            }
            var user = _userService.GetUserByUsername(cred.Username);

            if (user is null)
            {
                LogService.Log(cred.Username + " nem létező felhasználó", LogLevel.Warning);

                return null;
            }
            string hash = HashString(cred.Password);
            var u = hash != user.Password ? null : user;
            if (u is not null)
            {
                LogService.Log(u.Username + " bejelentkezett", LogLevel.Information);
            }
            else
            {
                LogService.Log(user.Username + " rossz jelszó", LogLevel.Warning);
            }
            return u;
        }
        public void LogOut(User user)
        {
            throw new NotImplementedException();
        }


    }
}
