namespace PalettaPolizeiPro.Data.Users
{
    public class Feedback : EntityObject
    {
        public User User { get; set; }
        public long UserId { get; set; }
        public string Text { get; set; }
        public DateTime Time { get; set; }
    }
}
