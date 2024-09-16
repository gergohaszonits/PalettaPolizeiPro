using PalettaPolizeiPro.Services.PLC;

namespace PalettaPolizeiPro.Data.Stations
{
    public enum ConnectionState
    { 
        Connected,
        Disconnected,
    }
    public class PlcConnectionEventArgs
    {
        public required DateTime Time { get; set; }
        public required IPLCLayer Plc { get; set; }
        public ConnectionState State { get; set; }
    }
}
