using PalettaPolizeiPro.Data.Stations;

namespace PalettaPolizeiPro.Data.Palettas
{
    public class PalettaArrivedCheckEventArgs
    {
        public required Paletta Paletta { get; set; }
        public required Station Station { get; set; }   
        public required PalettaProperty PalettaProperty { get; set; }
    }
}
