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
        public List<Order> InScheduled { get; set; }
        public List<Order> InFinished { get; set; }
        public List<PalettaProperty> Properties { get; set; }
    }
}
