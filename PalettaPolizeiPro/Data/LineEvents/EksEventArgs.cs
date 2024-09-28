using PalettaPolizeiPro.Data.EKS;
using PalettaPolizeiPro.Data.Stations;

namespace PalettaPolizeiPro.Data.LineEvents
{
    public class EksEventArgs : EntityObject
    {
        public required EksState State { get; set; }
        public Eks Eks { get; set; }
        public long EksId { get; set; }
        public Station Station { get; set; }
        public long StationId { get; set; }
        public required DateTime Time { get; set; }

    }
}
