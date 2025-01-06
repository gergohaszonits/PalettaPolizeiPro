using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using PalettaPolizeiPro.Data.EKS;
using PalettaPolizeiPro.Data.LineEvents;
using PalettaPolizeiPro.Data.Palettas;
using PalettaPolizeiPro.Data.Stations;
using PalettaPolizeiPro.Database;
using PalettaPolizeiPro.Services.Events;
using PalettaPolizeiPro.Services.Orders;
using PalettaPolizeiPro.Services.Stations;
using System.Data;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PalettaPolizeiPro.Services.PalettaControl;

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
    private DateTime _lastDeleteCheck = DateTime.MinValue;

    private TimeSpan _keepDataTime = TimeSpan.FromDays(7);
    private TimeSpan _deleteDataCheckTime = TimeSpan.FromMinutes(5);

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

            if ((DateTime.Now - _lastDeleteCheck) > _deleteDataCheckTime)
            {
                _lastDeleteCheck = DateTime.Now;
                tasks.Add(Task.Run(DeleteCheck));
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
            && DateTime.Now > x.StartSortTime && DateTime.Now < x.EndSortTime
            && x.ScheduledPalettas.FirstOrDefault(x => x.Identifier == state.PalettaName) is not null
            && x.FinishedPalettas.FirstOrDefault(x => x.Identifier == state.PalettaName) is null);


            if (orders.Count > 0)
            {
            //out
            outagain:
                _controlService.PalettaOut(station);
                if (_controlService.GetControlByte(station) == 1)
                {
                    goto outagain;
                }
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
            goagain:
                _controlService.PalettaGo(station);
                if (_controlService.GetControlByte(station) == 1)
                {
                    goto goagain;
                }
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
    private void DeleteCheck()
    {
        //paletta query events
        using (var context = new DatabaseContext())
        {
            var groups = context.QueryEvents
                .Where(x => (x.Time + _keepDataTime) < DateTime.Now)
                .GroupBy(x => x.State.PalettaName)
                .ToList();

            foreach (var group in groups)
            {
                if (group.Count() > 1)
                {
                    var itemsToRemove = group.Skip(1);
                    context.QueryEvents.RemoveRange(itemsToRemove);
                }
            }
            context.SaveChanges();
        }
        //remove orders
        using (var context = new DatabaseContext())
        {
            var range = context.Orders.Where(x => x.FinishedTime != null && (x.FinishedTime + _keepDataTime) < (DateTime.Now));
            if (range.Count() > 0)
            {
                foreach (var order in range)
                {
                    _orderService.Notify(new OrderEventArgs { Order = order, Time = DateTime.Now, State = Data.ChangeState.Removed });
                }
                context.Orders.RemoveRange(range);
                context.SaveChanges();
            }
        }

        //palettaproperties
        using (var context = new DatabaseContext())
        {
            var groups = context.PalettaProperties
                .Where(x => (x.ReadTime + _keepDataTime) < DateTime.Now )
                .GroupBy(x => x.Identifier)
                .ToList();

            foreach (var group in groups)
            {

                if (group.Count() > 1)
                {
                    var itemsToRemove = group.Skip(1);
                    context.PalettaProperties.RemoveRange(itemsToRemove);
                }
            }

            context.SaveChanges();
        }
        //paletta check events
        using (var context = new DatabaseContext())
        {
            var groups = context.CheckEvents
                .Where(x => (x.Time + _keepDataTime) < DateTime.Now)
                .GroupBy(x => x.Property.Identifier)
                .ToList();

            foreach (var group in groups)
            {

                if (group.Count() > 1)
                {
                    var itemsToRemove = group.Skip(1);
                    context.CheckEvents.RemoveRange(itemsToRemove);
                }
            }

            context.SaveChanges();
        }

        RemoveNullPaletta();
       
    }

    private async Task RemoveNullPaletta()
    {
        var dbContext = new DatabaseContext();
        using (var command = dbContext.Database.GetDbConnection().CreateCommand())
        {
            command.CommandText = "DELETE FROM \"Palettas\" WHERE \"Id\" IN (SELECT \"Palettas\".\"Id\" FROM \"Palettas\" FULL OUtER JOIN \"PalettaProperties\" ON \"Palettas\".\"Id\" = \"PalettaProperties\".\"PalettaId\" WHERE \"PalettaProperties\".\"Id\" IS NULL OR \"Palettas\".\"Id\" IS NULL)";
            await dbContext.Database.OpenConnectionAsync();
            await command.ExecuteNonQueryAsync();     
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