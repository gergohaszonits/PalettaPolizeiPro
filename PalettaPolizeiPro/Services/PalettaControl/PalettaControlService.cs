using Microsoft.Identity.Client;
using MudBlazor.Extensions;
using PalettaPolizeiPro.Data;
using PalettaPolizeiPro.Data.Palettas;
using PalettaPolizeiPro.Services.PLC;
using Sharp7;

namespace PalettaPolizeiPro.Services.PalettaControl
{
    public class PalettaControlService : IPalettaControlService
    {
        private PalettaControlService() { }
        private static PalettaControlService _instance = new PalettaControlService();
        public static List<IPLCLayer>? PLCs { get; set; }
        private object _plcListLock = new object();
        public static void Init(List<IPLCLayer> plcs)
        {
            PLCs = plcs;
            _instance.ConnectAll();
        }
        private void ConnectAll()
        {
            PLCs!.ForEach((x) =>
            {
                x.Connect();
            });
        }
        public static IPalettaControlService GetInstance()
        {
            return _instance;
        }
        public PalettaProperty GetProperty(Station station)
        {
            var plc = FindPlcFromStation(station);
            byte[] buffer = plc.GetBytes(station.DB, 0, 16);
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
                plc.SetBytes( station.DB, 1, 1, new byte[] { val });
            }

            if (state.OperationStatus != null)
            {
                byte val = (byte)state.OperationStatus;
                plc.SetBytes(station.DB, 0, 1,new byte[] { val });
            }
            if (state.PalettaName != null)
            {
                throw new Exception("You cannot set the PalettaName property");
            }
        }

        private void RegisterPLC(IPLCLayer plc)
        {
            lock (_plcListLock)
            {
                if (PLCs!.FirstOrDefault(x => x.IP == plc.IP && x.Rack == plc.Rack && x.Slot == plc.Slot) is not null)
                { return; }
                PLCs!.Add(plc);
            }
        }
        private IPLCLayer FindPlcFromStation(Station station)
        {
            lock (_plcListLock)
            {
                var plc = PLCs!.FirstOrDefault(x => x.IP == station.IP && x.Rack == station.Rack && x.Slot == station.Slot);
                if (plc is null) { throw new Exception("This PLC is not registered"); }
                return plc;
            }
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
                byte[] temp = new byte[1] { bytes.GetByteAt(0 + i) };
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
        private bool AllZero(byte[] buffer)
        {
            for (int i = 0; i < buffer.Length; i++)
            {
                if (buffer[i] != 0)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
