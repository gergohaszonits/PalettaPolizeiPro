using PalettaPolizeiPro.Data.EKS;
using PalettaPolizeiPro.Data.LineEvents;
using PalettaPolizeiPro.Data.Palettas;
using PalettaPolizeiPro.Data.Stations;
using PalettaPolizeiPro.Services.Events;
using PalettaPolizeiPro.Services.Orders;
using PalettaPolizeiPro.Services.Stations;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PalettaPolizeiPro.Services.PalettaControl
{
    public class LineControlProcess : IUpdatable
    {
        private ControlService _controlService;
        private bool _sentOut = false;


        private LineEventService _lineEventService = LineEventService.GetInstance();

        public event EventHandler<TimeSpan> OnLastTickDuration = delegate { };
        private StationService _stationsService = StationService.GetInstance();
        private OrderService _orderService = new OrderService();

        private List<PlcStationGroups> _groups;
        public DateTime LastUpdated { get; private set; }
        public TimeSpan LastUpdateDuration { get; private set; }

        private bool _reloadTrigger = false;

        private DateTime _lastOrdersCheck = DateTime.MinValue;
        public LineControlProcess(ControlService palettaService)
        {
            _controlService = palettaService;
            _groups = _controlService.GetPlcStationGroups();
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


                if ((DateTime.Now - _lastOrdersCheck) > TimeSpan.FromMinutes(1))
                {
                    _lastOrdersCheck = DateTime.Now;
                    tasks.Add(Task.Run(OrdersCheck));
                }

                await Task.WhenAll(tasks);

                LastUpdateDuration = (DateTime.Now - LastUpdated);
                OnLastTickDuration.Invoke(this, LastUpdateDuration);
                LastUpdated = DateTime.Now;

                if (_reloadTrigger)
                {
                    _reloadTrigger = false;
                    _groups = _controlService.GetPlcStationGroups();
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
            var state = _controlService.GetQueryState(station);
            var cached = _controlService.GetCachedQueryState(station);
            if (cached is not null && state is null)
            {
                _lineEventService.NewQueryEvent(new QueryEventArgs { State = null, Station = station, StationId = station.Id, Time = DateTime.Now });
                return;

            }
            if (state is not null && state.ControlFlag == 1 && state.OperationStatus == 255)
            {

                var orders = _orderService.GetWhere(x => (x.Status == OrderStatus.Sorting
                || x.Status == OrderStatus.Scheduled)
                && x.ScheduledPalettas.FirstOrDefault(x => x.Identifier == state.PalettaName) is not null
                && x.FinishedPalettas.FirstOrDefault(x => x.Identifier == state.PalettaName) is null);


                if (orders.Count > 0)
                {
                    //out
                    _controlService.PalettaOut(station);
                    var paletta = orders[0].ScheduledPalettas.FirstOrDefault(x => x.Identifier == state.PalettaName);
                    foreach (var order in orders)
                    {
                        order.FinishedPalettas.Add(paletta!);
                        if (order.FinishedPalettas.Count == order.ScheduledPalettas.Count)
                        {
                            order.FinishedTime = DateTime.Now;
                            order.Status = OrderStatus.Success;
                        }
                        else if (order.FinishedPalettas.Count == 1 && order.Status == OrderStatus.Scheduled)
                        {
                            order.Status = OrderStatus.Sorting;
                        }
                        _orderService.AddOrUpdate(order);
                    }
                    state.ControlFlag = 4;
                    _lineEventService.NewQueryEvent(new QueryEventArgs { State = state, Station = station, StationId = station.Id, Time = DateTime.Now });

                }
                else
                {
                    //go
                    _controlService.PalettaGo(station);
                    //ha go sikerul akkor a state 2 
                    state.ControlFlag = 2;
                    _lineEventService.NewQueryEvent(new QueryEventArgs { State = state, Station = station, StationId = station.Id, Time = DateTime.Now });
                }
            }

        }
        private void HandleCheck(Station station)
        {
            var prev = _controlService.GetCachedProperty(station);
            var property = _controlService.GetProperty(station);
            if (property != null)
            {
                if (prev is not null && prev.Identifier != property.Identifier)
                {
                    _lineEventService.NewCheckEvent(new CheckEventArgs { Property = property, Station = station, StationId = station.Id, Time = DateTime.Now });
                }
                else if (prev is null)
                {
                    _lineEventService.NewCheckEvent(new CheckEventArgs { Property = property, Station = station, StationId = station.Id, Time = DateTime.Now });
                }
            }
            else if (property is null && prev is not null)
            {
                //_lineEventService.NewCheckEvent(new CheckEventArgs {StationId = station.Id, Time = DateTime.Now });
            }

        }
        private void HandleEks(Station station)
        {
            var prevEks = _controlService.GetCachedEks(station);
            var eks = _controlService.GetEks(station);

            if (prevEks is null && eks is not null)
            {
                _lineEventService.NewEksEvent(new EksEventArgs { State = EksState.In, EksWorkerId = eks.WorkerId, Station = station, StationId = station.Id, Time = DateTime.Now });
            }
            else if (prevEks is not null && eks is null)
            {
                _lineEventService.NewEksEvent(new EksEventArgs { State = EksState.Out, EksWorkerId = prevEks.WorkerId, Station = station, StationId = station.Id, Time = DateTime.Now });
            }

        }
        private void OrdersCheck()
        {
            var orders = _orderService.GetAll();
            foreach (var order in orders)
            {
                if (order.EndSortTime < DateTime.Now && (order.Status == OrderStatus.Scheduled || order.Status == OrderStatus.Sorting))
                {
                    order.Status = OrderStatus.Failed;
                    order.FinishedTime = DateTime.Now;
                    _orderService.AddOrUpdate(order);
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