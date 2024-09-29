using PalettaPolizeiPro.Data.Stations;
using PalettaPolizeiPro.Services;

namespace PalettaPolizeiPro.Data.Users
{
    public class Client
    {
        private Guid Guid = Guid.NewGuid();
        public string? Ip { get; set; }
        public bool LoggedInWithEks { get; set; }
        public ClientSessionMemory SessionMemory { get; set; } = new ClientSessionMemory();
        public Client()
        {
            LogService.Log($"Client created: {Guid}", LogLevel.Information);
        }
        public User? User { get; set; }
        public Station? Station { get; set; }
        ~Client()
        {
            LogService.Log($"Client disposed: {Guid}", LogLevel.Information);
        }
    }
}
