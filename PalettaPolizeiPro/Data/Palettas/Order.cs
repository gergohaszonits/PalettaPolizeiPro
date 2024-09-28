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
        public long UserId { get; set; }
        public  User User { get; set; }
        public string? InfoText { get; set; }
        public  List<Paletta> ScheduledPalettas { get; set; } = new List<Paletta>();
        public List<Paletta> FinishedPalettas { get; set; }  = new List<Paletta> { };
        public List<OrderPalettaScheduled> OrderPalettaSchedules { get; set; } = new List<OrderPalettaScheduled> { };
        public List<OrderPalettaFinished> OrderPalettaFinishes { get; set; } = new List<OrderPalettaFinished> { };
        public DateTime? ScheduledTime { get; set; }
        public DateTime? FinishedTime { get; set; }
        public DateTime? StartSortTime { get; set; }
        public DateTime? EndSortTime { get; set; }
        public long MaximumSort { get; set; }
        public OrderStatus Status { get; set; }
    }
}
