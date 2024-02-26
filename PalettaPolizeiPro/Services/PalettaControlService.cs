using Microsoft.Identity.Client;
using PalettaPolizeiPro.Data;
using PalettaPolizeiPro.Data.Palettas;
using Sharp7;

namespace PalettaPolizeiPro.Services
{
    public class PalettaControlService : IPalettaControlService
    {
        private PalettaControlService() { }
        private static PalettaControlService _instance = new PalettaControlService();
        public static List<IPLCLayer>? PLCs { get ; set ; }

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
            byte[] buffer = plc.GetBytes(station.DB,0,16);
            string identifier = GetIdentifier(buffer, station);

            PalettaProperty property = new PalettaProperty
            {
                 
            };
            return property;

            /*
                var bytes =  Client.GetBytes(station.IP, station.Rack, station.Slot, station.DB, 0, 16,10000,10000);
                if (!HasInformation(bytes)) { return null; }
                string identifier = GetIdentifier(bytes,station);
                var mokanyBytes = Client.GetBytes(station.IP, station.Rack, station.Slot, station.DB, 240, 9,10000,10000);
             */
        }
        public QueryState GetQueryState(Station station)
        {
            IPLCLayer plc = FindPlcFromStation(station);
            byte[] bytes = plc.GetBytes(10,10,10);
            QueryState query = new QueryState
            {
               
            };
            return query;
            
        }
        public void SetQueryState(QueryState state, Station station)
        {
            throw new NotImplementedException();
        }

        private void RegisterPLC(IPLCLayer plc)
        {
            if (PLCs!.FirstOrDefault(x => x.IP == plc.IP && x.Rack == plc.Rack && x.Slot == plc.Slot) is not null)
            { return; }

            plc.Connect();
            PLCs!.Add(plc);
        }
        private IPLCLayer FindPlcFromStation(Station station)
        {
            var plc = PLCs!.FirstOrDefault(x => x.IP == station.IP && x.Rack == station.Rack && x.Slot == station.Slot);
            if (plc is null) { throw new Exception("This PLC is not registered"); }
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
                byte[] temp = new byte[1] { S7.GetByteAt(bytes, 0 + i) };
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
    }
}
