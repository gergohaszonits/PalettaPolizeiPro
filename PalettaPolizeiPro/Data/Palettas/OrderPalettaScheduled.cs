namespace PalettaPolizeiPro.Data.Palettas
{
    public class OrderPalettaScheduled: EntityObject
    {
        public Paletta Paletta { get; set; }
        public long PalettaId  { get; set; }
        public Order Order { get; set; }
        public long OrderId { get; set; }
    }
}
