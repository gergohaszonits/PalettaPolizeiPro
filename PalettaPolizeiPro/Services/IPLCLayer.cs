using PalettaPolizeiPro.Data;
using PalettaPolizeiPro.Data.Palettas;
using PalettaPolizeiPro.Data.Stations;

namespace PalettaPolizeiPro.Services
{
    public interface IPLCLayer
    {
        Eks GetEks(Station station);
        PalettaProperty GetPalettaProperty(Station station);
    }
}
