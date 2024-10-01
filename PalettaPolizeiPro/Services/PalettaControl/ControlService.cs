using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Identity.Client;
using MudBlazor.Extensions;
using PalettaPolizeiPro.Data;
using PalettaPolizeiPro.Data.EKS;
using PalettaPolizeiPro.Data.Palettas;
using PalettaPolizeiPro.Data.Stations;
using PalettaPolizeiPro.Database;
using PalettaPolizeiPro.Services.PLC;
using PalettaPolizeiPro.Services.Stations;
using Sharp7;
using System;
using System.Collections.Concurrent;
using System.Text;
using static System.Collections.Specialized.BitVector32;

namespace PalettaPolizeiPro.Services.PalettaControl
{
    public class ControlService : IControlService
    {
        private ControlService()
        {
            _stationsService.OnStationChange += (s, a) =>
            {
                if (a.State == ChangeState.Added)
                {
                    OnAdd(a.Station);
                }
                else if (a.State == ChangeState.Modified)
                {
                    OnModify(a.Station);
                }
                else if (a.State == ChangeState.Removed)
                {
                    OnRemove(a.Station);
                }
            };
        }

        private static ControlService _instance = new ControlService();
        private ConcurrentDictionary<string, IPLCLayer> _plcs = new ConcurrentDictionary<string, IPLCLayer>();
        private List<Station> _stations = new List<Station>();
        private StationService _stationsService = StationService.GetInstance();
        private object _locker = new object();

        private Dictionary<Station, QueryState?> QueryCache = new Dictionary<Station, QueryState?>();
        private Dictionary<Station, PalettaProperty?> PropertyCache = new Dictionary<Station, PalettaProperty?>();
        private Dictionary<Station, Eks?> EksCache = new Dictionary<Station, Eks?>();



        public static IControlService GetInstance()
        {
            return _instance;
        }
        public void Init(List<Station> stations)
        {
            foreach (var station in stations)
            {
                try
                {
                    AttachStation(station);
                }
                catch (Exception ex) { LogService.LogException(ex); }
            }
        }
        public PalettaProperty? GetProperty(Station station)
        {
            var plc = FindPlcFromStation(station);
            if (!plc.IsConnected)
            {
                PropertyCache[station] = null;
                return null;
            }
            byte[] buffer = plc.GetBytes(station.DB, 0, 16);
            if (AllZero(buffer))
            {
                PropertyCache[station] = null;
                return null;
            }
            string identifier = GetIdentifier(buffer, station);
            byte[] mokanyBytes = plc.GetBytes(station.DB, 240, 9);
            string? engineNumber = null;
            if (!AllZero(mokanyBytes))
            {
                engineNumber = S7.GetCharsAt(mokanyBytes, 0, 9);
            }
            PalettaProperty property = new PalettaProperty
            {
                Identifier = identifier,
                ActualCycle = S7.GetIntAt(buffer, 12),
                PredefiniedCycle = S7.GetIntAt(buffer, 14),
                ReadTime = DateTime.Now,
                EngineNumber = engineNumber,
            };
            PropertyCache[station] = property;
            return property;
        }
        public QueryState? GetQueryState(Station station)
        {
            IPLCLayer plc = FindPlcFromStation(station);
            if (!plc.IsConnected)
            {
                QueryCache[station] = null;
                return null;
            }
            byte[] bytes = plc.GetBytes(station.DB, 0, 11);


            bool fullZero = true;
            for (int i = 2; i < 11; i++)
            {
                if (bytes[i] != 0)
                {
                    fullZero = false;
                    break;
                }
            }

            QueryState query = new QueryState
            {
                OperationStatus = bytes[0],
                ControlFlag = bytes[1],
                PalettaName = fullZero ? null : S7.GetCharsAt(bytes, 2, 9)
            };

            QueryCache[station] = query;

            return query;
        }
        public void PalettaGo(Station station)
        {
            var plc = FindPlcFromStation(station);
           
            if (plc is null) { return; }
            if (!plc.IsConnected)
            {
                return;
            }
            plc.SetBytes(station.DB, 1, 1, [2]);
        }
        public void PalettaOut(Station station)
        {
            var plc = FindPlcFromStation(station);
            if (plc is null) { return; }
            if (!plc.IsConnected)
            {
                return;
            }
            plc.SetBytes(station.DB, 1, 1, [4]);
        }
        public byte GetControlByte(Station station)
        {
            var plc = FindPlcFromStation(station);
            if (plc is null) { return 0; }
            if (!plc.IsConnected)
            {
                return 0;
            }
           return plc.GetBytes(station.DB, 1, 1)[0];
        }
        public byte GetStatusByte(Station station)
        {
            var plc = FindPlcFromStation(station);
            if (plc is null) { return 0; }
            if (!plc.IsConnected)
            {
                return 0;
            }
            return plc.GetBytes(station.DB, 0, 1)[0];
        }
        public void OperationStatusOff(Station station)
        {
            var plc = FindPlcFromStation(station);
            if (plc is null) { return; }
            if (!plc.IsConnected)
            {
                return;
            }
            plc.SetBytes(station.DB, 0, 1, [0]);
        }

        public void OperationStatusOn(Station station)
        {
            var plc = FindPlcFromStation(station);
            if (plc is null) { return; }
            if (!plc.IsConnected)
            {
                return;
            }
            plc.SetBytes(station.DB, 0, 1, [255]);
        }
        private void AttachStation(Station station)
        {
            string key = station.Ip + station.Rack + station.Slot;
            IPLCLayer? plc;
            lock (_locker)
            {
                if (!_plcs.TryGetValue(key, out plc))
                {
                    if (SIMULATION)
                    {
                        plc = new SimulatedTcpPlc(station.Ip, station.Rack, station.Slot);
                    }
                    else
                    {
                        plc = new S7PLC(station.Ip, station.Rack, station.Slot);
                    }

                    _plcs.TryAdd(key, plc);
                }
                _stations.Add(station);
                //plc.Connect();
                if (station.StationType == StationType.Query)
                {
                    GetQueryState(station);
                }
                else if (station.StationType == StationType.Check)
                {
                    GetProperty(station);
                }
            }
        }
        private void DeattachStation(Station station)
        {
            string key = station.Ip + station.Rack + station.Slot;
            lock (_locker)
            {
                IPLCLayer? plc;
                var stations = _stations.Where(x => x.Rack == station.Rack && x.Ip == station.Ip && x.Rack == station.Rack).ToList();
                if (stations.Count == 1)
                {
                    if (_plcs.TryGetValue(key, out plc))
                    {
                        try
                        {
                            plc.Disconnect();
                            _plcs.Remove(key, out plc);
                        }
                        catch (Exception ex) { LogService.LogException(ex); }
                    }
                }
                _stations.Remove(station);
            }
        }
        private IPLCLayer? FindPlcFromStation(Station station)
        {
            string key = station.Ip + station.Rack + station.Slot;
            IPLCLayer? plc;
            if (!_plcs.TryGetValue(key, out plc))
            {
                return null;
            }
            return plc;
        }
        private string GetIdentifier(byte[] bytes, Station station)
        {
            string lNummer = "";
            int lLen = 3 - station.Loop.ToString().Length;
            for (int i = 0; i < lLen; i++)
            {
                lNummer += "0";
            }
            lNummer = lNummer + station.Loop;

            string hex = "";
            string wNummer = "";

            for (int i = 0; i < 2; i++)
            {
                byte[] temp = [bytes.GetByteAt(0 + i)];
                hex += BitConverter.ToString(temp);
            }

            int wLen = 4 - hex.Length;
            for (int i = 0; i < wLen; i++)
            {
                wNummer += "0";
            }
            wNummer = wNummer + hex;

            return "L" + lNummer + "W" + wNummer;
        }

        private void OnAdd(Station station)
        {
            AttachStation(station);
        }
        private void OnRemove(Station station)
        {
            DeattachStation(station);
        }
        private void OnModify(Station station)
        {
            lock (_locker)
            {
                var st = _stations.FirstOrDefault(x => x.Id == station.Id);
                if (st is null)
                {
                    return;
                }
                st.Update(station);
                foreach (Station stat in _stations)
                {
                    Console.WriteLine(stat.Name);
                }
            }
        }

        public List<IPLCLayer> GetPlcs()
        {
            return _plcs.Values.ToList();
        }

        public List<PlcStationGroups> GetPlcStationGroups()
        {
            var groups = new List<PlcStationGroups>();
            foreach (var station in _stations)
            {
                var plc = FindPlcFromStation(station);
                if (plc is null)
                {
                    continue;
                }
               
                var item = groups.FirstOrDefault(x => x.Plc.IP == plc.IP && x.Plc.Rack == plc.Rack && x.Plc.Slot == plc.Slot);
                if (item is null)
                {
                    List<Station> sts = [station];
                    groups.Add(new PlcStationGroups
                    {
                        Plc = plc,
                        Stations = sts
                    });
                }
                else
                {
                    if (item.Stations is null)
                    {
                        item.Stations = new List<Station>();
                    }
                    item.Stations.Add(station);
                }
            }
            return groups;
        }
        public bool IsParentPlcConnected(Station station)
        {
            var plc = FindPlcFromStation(station);
            if (plc is null)
            {
                return false;
            }
            return plc.IsConnected;
        }

        public PalettaProperty? GetCachedProperty(Station station)
        {
            PalettaProperty? p = null;
            bool val = PropertyCache.TryGetValue(station, out p);
            if (!val)
            {
                return null;
            }
            return p;
        }

        public QueryState? GetCachedQueryState(Station station)
        {
            QueryState? q = null;
            bool val = QueryCache.TryGetValue(station, out q);
            if (!val)
            {
                return null;
            }
            return q;
        }

        public List<Paletta> GetPalettas()
        {
            using (var context = new DatabaseContext())
            {
                return context.Palettas.AsNoTracking().Include(x => x.OrderPalettaFinishes).Include(x => x.OrderPalettaSchedules).Include(x => x.OrderPalettaFinishes).Include(x => x.OrderPalettaSchedules).Include(x => x.InFinished).Include(x => x.Properties).Include(x => x.InScheduled).ToList();
            }
        }

        public List<Paletta> GetPalettas(Func<Paletta, bool> predicate)
        {
            using (var context = new DatabaseContext())
            {
                return context.Palettas.AsNoTracking()
                    .Include(x => x.OrderPalettaFinishes).Include(x => x.OrderPalettaSchedules)
                    .Include(x => x.InFinished)
                    .Include(x => x.InScheduled)
                    .Include(x => x.Properties)
                    .Where(predicate)
                    .ToList();
            }
        }

        public List<PalettaProperty> GetPalettaProperties()
        {
            using (var context = new DatabaseContext())
            {
                return context.PalettaProperties.AsNoTracking().OrderByDescending(x => x.ReadTime).ToList();
            }
        }

        public List<PalettaProperty> GetPalettaProperties(Func<PalettaProperty, bool> predicate)
        {
            using (var context = new DatabaseContext())
            {
                return context.PalettaProperties.AsNoTracking().OrderByDescending(x => x.ReadTime).Where(predicate).ToList();
            }
        }

        public List<Paletta> GetPalettasWithLastProperty(Func<Paletta, bool> predicate)
        {
            using (var context = new DatabaseContext())
            {
                return context.Palettas.AsNoTracking()
                    .Include(x => x.InFinished)
                    .Include(x => x.OrderPalettaFinishes).Include(x => x.OrderPalettaSchedules)
                    .Include(x => x.InScheduled)
                    .Select(p => new Paletta
                    {
                        Id = p.Id,
                        Identifier = p.Identifier,
                        InFinished = p.InFinished,
                        InScheduled = p.InScheduled,
                        Loop = p.Loop,
                        Marked = p.Marked,
                        PalettaError = p.PalettaError,
                        ServiceFlag = p.ServiceFlag,
                        Properties = p.Properties
                                      .OrderByDescending(prop => prop.ReadTime)
                                      .Take(1)
                                      .ToList()
                    })
                    .Where(predicate)
                    .ToList();
            }
        }

        public List<Paletta> GetPalettasWithLastProperty()
        {
            using (var context = new DatabaseContext())
            {
                return context.Palettas.AsNoTracking()
                    .Include(x => x.InFinished)
                    .Include(x => x.InScheduled)
                    .Include(x => x.OrderPalettaFinishes).Include(x => x.OrderPalettaSchedules)
                    .Select(p => new Paletta
                    {
                        Id = p.Id,
                        Identifier = p.Identifier,
                        InFinished = p.InFinished,
                        InScheduled = p.InScheduled,
                        Loop = p.Loop,
                        Marked = p.Marked,
                        PalettaError = p.PalettaError,
                        ServiceFlag = p.ServiceFlag,
                        Properties = p.Properties
                                      .OrderByDescending(prop => prop.ReadTime)
                                      .Take(1)
                                      .ToList()
                    })
                    .ToList();
            }
        }
        public void CreatePaletta(Paletta paletta)
        {
            using (var context = new DatabaseContext())
            {
                context.Palettas.Add(paletta);
                context.SaveChanges();
            }
        }

        public Eks? GetEks(Station station)
        {
            var plc = FindPlcFromStation(station);

            if (station.IsStationOn == false) { EksCache[station] = null; return null; }
            if (plc is null) { EksCache[station] = null; return null; }
            if (!plc.IsConnected) {EksCache[station] = null; return null;}

            var bytes = plc.GetBytes(station.DB, 1718,12);


            bool fullZero = true;
            for (int i = 0; i < 12; i++)
            {
                if (bytes[i] != 0)
                { 
                    fullZero = false;
                    break;
                }
            }

            if (fullZero)
            {
                EksCache[station] = null;
                return null;
            }

            string workerid = Encoding.UTF8.GetString(bytes).Trim().Replace("\0", "");

            if (string.IsNullOrWhiteSpace(workerid))
            {
                EksCache[station] = null;
                return null;
            }

            var eks = new Eks { WorkerId = workerid };
            EksCache[station] = eks;
            return eks;
        }
        public Eks? GetCachedEks(Station station)
        {
            Eks? e = null;
            bool val = EksCache.TryGetValue(station, out e);
            if (!val)
            {
                return null;
            }
            return e;
        }

        public List<Eks> GetStoredEksList()
        {
            using (var context = new DatabaseContext())
            { 
                return context.Eks.ToList();
            }
        }

        public List<Eks> GetStoredEksList(Func<Eks, bool> predicate)
        {
            using (var context = new DatabaseContext())
            {
                return context.Eks.Where(predicate).ToList();
            }
        }

        public Eks? EksFirstOrNull(Func<Eks, bool> predicate)
        {
            using (var context = new DatabaseContext())
            {
                return context.Eks.FirstOrDefault(predicate);
            }
        }
    }
}
