using PalettaPolizeiPro.Data.Palettas;
using PalettaPolizeiPro.Data.Users;

namespace PalettaPolizeiPro.Data
{
    public enum NotificationLevel
    {
        Error,
        Warning,
        Information,
        Acknowledgement
    }
    public class Notification : EntityObject
    {
        public required string Title { get; set; }
        public required NotificationLevel Level { get; set; }
        public string? Body { get; set; }
    }
    public class PalettaNotification : Notification
    { 
        public required Paletta Paletta { get; set; }
        public long PalettaId { get; set; } 

    }
    public class QueryNotification : Notification
    {
        public required QueryState QueryState { get; set; }
        public long QueryStateId { get; set; }
    }
    public class UserNotification : Notification
    {
        public required User User { get; set; }
        public long UserId { get; set; }
    }
}
