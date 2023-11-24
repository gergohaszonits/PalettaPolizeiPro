using PalettaPolizeiPro.Data.Palettas;
using PalettaPolizeiPro.Data.Stations;

namespace PalettaPolizeiPro.Services
{
    public interface IProductionControlService
    {
        PalettaProperty GetProperty(Station station);
        QueryState GetQueryState(Station station);
        void SetQueryState(QueryState state);
        public static abstract IProductionControlService GetInstance();

    }
}
