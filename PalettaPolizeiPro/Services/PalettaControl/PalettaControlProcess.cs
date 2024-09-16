using PalettaPolizeiPro.Data.Palettas;
using PalettaPolizeiPro.Data.Stations;

namespace PalettaPolizeiPro.Services.PalettaControl
{
    public class PalettaControlProcess : IUpdatable
    {
        private PalettaControlService _palettaService;
        private bool _sentOut = false;
        public PalettaControlProcess(PalettaControlService palettaService)
        {
            _palettaService = palettaService;
        }

        public void Update()
        {
            var groups = _palettaService.GetPlcStationGroups();
            foreach (var group in groups)
            {
                foreach (var station in group.Stations)
                {
                    
                }
            }
        }
    }
}


// teszt 
/*
 
             var groups = _palettaService.GetPlcStationGroups();
            foreach (var group in groups)
            {
                foreach (var station in group.Stations)
                {
                    if (station.StationType == StationType.Query)
                    {
                        var state = _palettaService.GetQueryState(station);
                        if (state.OperationStatus == 255 && state.ControlFlag == 1)
                        {
                            if (state.PalettaName == "L001W0002" && !_sentOut)
                            {
                                _sentOut = true;
                                _palettaService.SetQueryState(new QueryState
                                {
                                    ControlFlag = 4,
                                }, station);
                            }
                            else
                            {
                                _palettaService.SetQueryState(new QueryState
                                {
                                    ControlFlag = 2,
                                }, station);
                            }
                        }

                    }
                }
            }
 */