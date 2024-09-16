using PalettaPolizeiPro.Services;

namespace PalettaPolizeiPro.Data.Users
{
    public class User : EntityObject
    {
        public required string Username { get; set; }
        public required string Password { get; set; }
        public string? WorkerID { get; set; }
        public required Role Role { get; set; }

    }
}
