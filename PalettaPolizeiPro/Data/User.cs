#nullable disable

namespace PalettaPolizeiPro.Data
{
    public class User : EntityObject
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public bool IsAdmin { get; set; }
    }
}
