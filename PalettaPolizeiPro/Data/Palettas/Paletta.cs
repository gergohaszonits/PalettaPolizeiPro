#nullable disable
using PalettaPolizeiPro;

namespace PalettaPolizeiPro.Data.Palettas
{
    public class Paletta : EntityObject
    {
        public string Identifier { get; set; }
        public int Loop { get; set; }
        public bool ServiceFlag { get; set; }
        public bool PalettaError { get; set; }
        public bool Marked { get; set; }
        public bool IsOut { get; set; }
        public List<Order> InScheduled { get; set; } = new List<Order>();
        public List<Order> InFinished { get; set; } = new List<Order> { };
        public List<OrderPalettaScheduled> OrderPalettaSchedules { get; set; } = new List<OrderPalettaScheduled> { };
        public List<OrderPalettaFinished> OrderPalettaFinishes{ get; set; } = new List<OrderPalettaFinished> { };

        public List<PalettaProperty> Properties { get; set; }
    }
}
