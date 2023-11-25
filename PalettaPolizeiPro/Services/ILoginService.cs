using PalettaPolizeiPro.Data;
using PalettaPolizeiPro.Data.DataTransferObjects;

namespace PalettaPolizeiPro.Services
{
    public interface ILoginService
    {
        User? LogIn(UserCredentialsDTO credentials);
        void LogOut(User user);

    }
}