using PalettaPolizeiPro.Data.Palettas;
using PalettaPolizeiPro.Data.Stations;

namespace PalettaPolizeiPro.Data.LineEvents
{
    public class CheckEventArgs : EntityObject
    {
        public  Station Station { get; set; }
        public long StationId { get; set; }

        public  PalettaProperty Property { get; set; }
        public long PropertyId { get; set; }

        public required DateTime Time { get; set; }
    }
}
