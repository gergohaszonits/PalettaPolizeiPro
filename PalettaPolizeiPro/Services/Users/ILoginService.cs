using PalettaPolizeiPro.Data.DataTransferObjects;
using PalettaPolizeiPro.Data.Users;

namespace PalettaPolizeiPro.Services.Users
{
    public interface ILoginService
    {
        User? LogIn(UserCredentialsDTO credentials);
        void LogOut(User user);

    }
}