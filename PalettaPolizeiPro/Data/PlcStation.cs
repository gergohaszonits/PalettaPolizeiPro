using PalettaPolizeiPro.Services.PLC;

namespace PalettaPolizeiPro.Data
{
    public class PlcStations
    {
        public required IPLCLayer PLC { get; set; }
        public required List<Station> Stations { get; set; }
    }
}
