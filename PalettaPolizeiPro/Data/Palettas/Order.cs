using PalettaPolizeiPro.Data.Users;

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
    public class Order : EntityObject
    {
        public required User User { get; set; }
        public string? InfoText { get; set; }
        public required List<Paletta> ScheduledPalettas { get; set; }
        public required List<Paletta> FinishedPalettas { get; set; }
        public DateTime? ScheduledTime { get; set; }
        public DateTime? FinishedTime { get; set; }
        public DateTime? StartSortTime { get; set; }
        public DateTime? EndSortTime { get; set; }
        public long MaximumSort { get; set; }
        public OrderStatus Status { get; set; }
    }
}
