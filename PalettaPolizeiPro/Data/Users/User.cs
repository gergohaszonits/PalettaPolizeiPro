using PalettaPolizeiPro.Services;
using PalettaPolizeiPro.Data.EKS;

namespace PalettaPolizeiPro.Data.Users
{
    public class User : EntityObject
    {
        public required string Username { get; set; }
        public required string Password { get; set; }
        public required Role Role { get; set; }
        public long? EksId { get; set; }
        public Eks? Eks { get; set; }

    }
}
