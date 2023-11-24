using PalettaPolizeiPro.Data.Palettas;
using PalettaPolizeiPro.Data.Stations;

namespace PalettaPolizeiPro.Services
{
    public class ProductionControlService : IProductionControlService
    {
        private IPLCLayer _plcLayer;
        private static ProductionControlService _instance = new ProductionControlService();
        private ProductionControlService() 
        {          
            
        }

        public static IProductionControlService GetInstance()
        {
           return _instance;
        }

        public PalettaProperty GetProperty(Station station)
        {
            throw new NotImplementedException();
        }

        public QueryState GetQueryState(Station station)
        {
            throw new NotImplementedException();
        }

        public void SetQueryState(QueryState state)
        {
            throw new NotImplementedException();
        }
    }
}
