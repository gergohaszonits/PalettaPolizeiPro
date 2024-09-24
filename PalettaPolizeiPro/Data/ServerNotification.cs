using PalettaPolizeiPro.Data.Palettas;
using PalettaPolizeiPro.Data.Users;

namespace PalettaPolizeiPro.Data
{
    public enum NotificationType
    {
        Error,
        Warning,
        Information,
        Success,
    }
    public class ServerNotification : EntityObject
    {
        public required string Title { get; set; }
        public string? Body { get; set; }
        public required NotificationType Type { get; set; }
    }
}
