using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Identity.Client;
using MudBlazor.Extensions;
using PalettaPolizeiPro.Data;
using PalettaPolizeiPro.Data.Palettas;
using PalettaPolizeiPro.Data.Stations;
using PalettaPolizeiPro.Services.PLC;
using PalettaPolizeiPro.Services.Stations;
using Sharp7;
using System.Collections.Concurrent;

namespace PalettaPolizeiPro.Services.PalettaControl
{
    public class PalettaControlService : IPalettaControlService
    {
        private PalettaControlService()
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

        private static PalettaControlService _instance = new PalettaControlService();
        private ConcurrentDictionary<string, IPLCLayer> _plcs = new ConcurrentDictionary<string, IPLCLayer>();
        private List<Station> _stations = new List<Station>();
        private StationService _stationsService = StationService.GetInstance();
        private object _locker = new object();

        public static IPalettaControlService GetInstance()
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
                catch (Exception ex){ LogService.LogException(ex); }
            }
        }
        public PalettaProperty? GetProperty(Station station)
        {
            var plc = FindPlcFromStation(station);
            byte[] buffer = plc.GetBytes(station.DB, 0, 16);
            if (AllZero(buffer))
            { 
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
            return property;
        }
        public QueryState GetQueryState(Station station)
        {
            IPLCLayer plc = FindPlcFromStation(station);
            byte[] bytes = plc.GetBytes(station.DB, 0, 11);
            QueryState query = new QueryState
            {
                OperationStatus = bytes[0],
                ControlFlag = bytes[1],
                PalettaName = S7.GetCharsAt(bytes, 2, 9)
            };
            return query;
        }
        public void SetQueryState(QueryState state, Station station)
        {
            var plc = FindPlcFromStation(station);
            if (state.ControlFlag != null)
            {
                byte val = (byte)state.ControlFlag;
                plc.SetBytes(station.DB, 1, 1, new byte[] { val });
            }

            if (state.OperationStatus != null)
            {
                byte val = (byte)state.OperationStatus;
                plc.SetBytes(station.DB, 0, 1, new byte[] { val });
            }
            if (state.PalettaName != null)
            {
                throw new Exception("You cannot set the PalettaName property");
            }
        }

        private void AttachStation(Station station)
        {
            string key = station.IP + station.Rack + station.Slot;
            IPLCLayer? plc;
            lock (_locker)
            {
                if (!_plcs.TryGetValue(key, out plc))
                {
                    if (SIMULATION)
                    {
                        plc = new SimulatedTcpPlc(station.IP, station.Rack, station.Slot);
                    }
                    else
                    {
                        plc = new S7PLC(station.IP, station.Rack, station.Slot);
                    }

                    _plcs.TryAdd(key, plc);
                }
                _stations.Add(station);
                plc.Connect();
            }
        }
        private void DeattachStation(Station station)
        {
            string key = station.IP + station.Rack + station.Slot;
            lock (_locker)
            {
                IPLCLayer? plc;
                var stations = _stations.Where(x => x.Rack == station.Rack && x.IP == station.IP && x.Rack == station.Rack).ToList();
                if (stations.Count == 1)
                {
                    if (_plcs.TryGetValue(key, out plc))
                    {
                        try
                        {
                            plc.Disconnect();
                        }
                        catch (Exception ex) { LogService.LogException(ex); }
                    }
                }
                _stations.Remove(station);
                _plcs.Remove(key, out plc);
            }
        }
        private IPLCLayer? FindPlcFromStation(Station station)
        {
            string key = station.IP + station.Rack + station.Slot;
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
                byte[] temp = [ bytes.GetByteAt(0 + i) ];
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
                st = station;
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
    }
}
