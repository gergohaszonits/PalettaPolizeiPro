#nullable disable
using PalettaPolizeiPro;

namespace PalettaPolizeiPro.Data.Palettas
{
    public enum OrderStatus
    {
        Success,
        Failed,
        Scheduled,
        Cancelled,
        Sorting,
    }
    public class Order
    {
        public User User { get; set; }
        public List<Paletta> ScheduledPalettas { get; set; }
        public List<Paletta> FinishedPalettas { get; set; }
        public DateTime? Scheduled { get; set; }
        public DateTime? StartSort { get; set; }
        public DateTime? EndSort { get; set; }
        public long MaximumSort { get; set; }
        public OrderStatus Status { get; set; }
    }
}
