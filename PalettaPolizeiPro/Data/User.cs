namespace PalettaPolizeiPro.Data
{
    public class User : EntityObject
    {
        public required string Username { get; set; }
        public required string Password { get; set; }
        public string? WorkerID { get; set; }
        public required Role Role { get; set; }
    }
}
