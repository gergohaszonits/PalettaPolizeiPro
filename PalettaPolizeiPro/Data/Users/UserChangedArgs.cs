namespace PalettaPolizeiPro.Data.Users
{
    public class UserChangedArgs
    {
        public required User User { get; set; } 
        public ChangeState State{ get; set; }
    }
}
