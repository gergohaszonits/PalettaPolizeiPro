#nullable disable

namespace PalettaPolizeiPro.Data
{
    public class User : EntityObject
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string WorkerID { get; set; }
        public List<Authorization> Authorizations { get; set; }
    }
}
