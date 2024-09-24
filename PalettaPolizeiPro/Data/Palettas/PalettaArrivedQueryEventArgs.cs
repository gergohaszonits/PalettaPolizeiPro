using PalettaPolizeiPro.Data.Stations;

namespace PalettaPolizeiPro.Data.Palettas
{
    public class PalettaArrivedQueryEventArgs
    {
        public required Paletta Paletta { get; set; }
        public required QueryState QueryState { get; set; }
        public required Station Station { get; set; }   
    }

}
