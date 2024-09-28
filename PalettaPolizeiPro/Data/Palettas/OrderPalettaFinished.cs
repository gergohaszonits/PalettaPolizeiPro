namespace PalettaPolizeiPro.Data.Palettas
{
    public class OrderPalettaFinished : EntityObject
    {
        public Paletta Paletta { get; set; }
        public long PalettaId  { get; set; }
        public Order Order { get; set; }
        public long OrderId { get; set; }
    }
}
