using PalettaPolizeiPro.Data;

namespace PalettaPolizeiPro.Services
{
    public interface ILoginService
    {
        User? LogIn(string username, string password);
        void LogOut(User user);

    }
}