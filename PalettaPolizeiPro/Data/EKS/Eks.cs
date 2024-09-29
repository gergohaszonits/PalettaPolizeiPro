using PalettaPolizeiPro.Data.Users;

namespace PalettaPolizeiPro.Data.EKS
{
    public class Eks : EntityObject
    {
        public string? KeyId { get; set; }
        public  string WorkerId { get; set; }
        public User User { get; set; }
    }
}
