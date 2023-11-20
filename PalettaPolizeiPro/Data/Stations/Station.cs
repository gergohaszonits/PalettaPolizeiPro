#nullable disable
using PalettaPolizeiPro;

namespace PalettaPolizeiPro.Data.Stations
{
    public class Station
    {
        public string Alias { get; init; }
        public string Name { get; init; }
        public string IP { get; init; }
        public int Rack { get; init; }
        public int Slot { get; init; }
        public int DB { get; init; }
        public int Loop { get; init; }
    }
}
