using PalettaPolizeiPro.Data.EKS;
using PalettaPolizeiPro.Data.Events;
using PalettaPolizeiPro.Data.Palettas;
using PalettaPolizeiPro.Data.Stations;
using PalettaPolizeiPro.Services.Events;
using PalettaPolizeiPro.Services.Stations;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PalettaPolizeiPro.Services.PalettaControl
{
    public class LineControlProcess : IUpdatable
    {
        private PalettaControlService _palettaService;
        private bool _sentOut = false;


        private LineEventService _lineEventService = LineEventService.GetInstance();

        public event EventHandler<TimeSpan> OnLastTickDuration = delegate { };
        private StationService _stationsService = StationService.GetInstance();

        private List<PlcStationGroups> _groups;
        public DateTime LastUpdated { get; private set; }
        public TimeSpan LastUpdateDuration { get; private set; }

        private bool _reloadTrigger = false;
        public LineControlProcess(PalettaControlService palettaService)
        {
            _palettaService = palettaService;
            _groups = _palettaService.GetPlcStationGroups();
            _stationsService.OnStationChange += (o, s) => { _reloadTrigger = true; };
        }

        public async Task Update()
        {
            try
            {
                List<Task> tasks = new List<Task>();

                foreach (var group in _groups)
                {

                    tasks.Add(Task.Run(() =>
                    {
                        foreach (var station in group.Stations)
                        {
                            var plc = group.Plc;
                            if (!plc.IsConnected) { continue; }
                            try
                            {
                                HandleStation(station);
                            }
                            catch (Exception ex) { LogService.LogException(ex); }
                        }
                    }));
                }

                await Task.WhenAll(tasks);

                LastUpdateDuration = (DateTime.Now - LastUpdated);
                OnLastTickDuration.Invoke(this, LastUpdateDuration);
                LastUpdated = DateTime.Now;

                if (_reloadTrigger)
                {
                    _reloadTrigger = false;
                    _groups = _palettaService.GetPlcStationGroups();
                }
            }
            catch (Exception ex) { LogService.LogException(ex); }
        }
        private void HandleStation(Station station)
        {
            if (station.StationType == StationType.Query)
            {
                HandleQuery(station);
            }
            else if (station.StationType == StationType.Check)
            {
                HandleCheck(station);

            }
            else if (station.StationType == StationType.Eks)
            {
                HandleEks(station);

            }
        }
        private void HandleQuery(Station station)
        {
            var state = _palettaService.GetQueryState(station);
            var cached = _palettaService.GetCachedQueryState(station);
            if (cached is not null && state is null)
            {
                _lineEventService.NewQueryEvent(new QueryEventArgs { State = null, StationId = station.Id, Time = DateTime.Now });
                Console.WriteLine("ures paletta triggered");
                return;

            }
            if (state is not null  && state.ControlFlag == 1 && state.OperationStatus == 255)
            {
                _palettaService.PalettaGo(station);
                //ha go sikerul akkor a state 2 
                state.ControlFlag = 2;
                _lineEventService.NewQueryEvent(new QueryEventArgs { State = state, StationId = station.Id, Time = DateTime.Now });
            }
            
        }
        private void HandleCheck(Station station)
        {
            var prev = _palettaService.GetCachedProperty(station);
            var property = _palettaService.GetProperty(station);
            if (property != null)
            {
                if (prev is not null && prev.Identifier != property.Identifier)
                {
                    _lineEventService.NewCheckEvent(new CheckEventArgs { Property = property, StationId = station.Id, Time = DateTime.Now });
                }
                else if (prev is null)
                {
                    _lineEventService.NewCheckEvent(new CheckEventArgs { Property = property, StationId = station.Id, Time = DateTime.Now });
                }
            }
            else if (property is null && prev is not null)
            {
                //_lineEventService.NewCheckEvent(new CheckEventArgs {StationId = station.Id, Time = DateTime.Now });
            }

        }
        private void HandleEks(Station station)
        {

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