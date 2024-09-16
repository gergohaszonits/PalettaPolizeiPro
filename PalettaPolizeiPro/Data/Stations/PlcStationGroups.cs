using PalettaPolizeiPro.Services.PLC;

namespace PalettaPolizeiPro.Data.Stations
{
    public class PlcStationGroups
    {
        public List<Station> Stations { get; set; }
        public IPLCLayer Plc { get; set; }
    }
}
