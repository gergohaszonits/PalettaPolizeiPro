using PalettaPolizeiPro.Data.Palettas;
using PalettaPolizeiPro.Data.Stations;

namespace PalettaPolizeiPro.Data.LineEvents
{
    public class QueryEventArgs : EntityObject
    {
        public long StationId { get; set; }
        public  Station Station { get; set; }
        public required QueryState State { get; set; }
        public required DateTime Time { get; set; }
    }
}
