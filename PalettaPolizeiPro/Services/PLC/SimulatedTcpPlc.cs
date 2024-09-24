using ProductionLineSimulation.LineCommunication.Services;
using Sharp7;
using ProductionLineSimulation.Communication.Data;
using ProductionLineSimulation.LineCommunication.Data;
using ProductionLineSimulation.Communication;

namespace PalettaPolizeiPro.Services.PLC
{
    public class SimulatedTcpPlc : IPLCLayer
    {
        private static LineClient _client;
        public static void InitSimulation(string ip, int port)
        {
            _client = new LineClient(ip, port);
            _client.Connect();
        }
        public string IP { get; set; }
        public int Rack { get; set; }
        public int Slot { get; set; }

        public bool IsConnected => _IsConnected;

        public bool _IsConnected = false;
        public SimulatedTcpPlc(string iP, int rack, int slot)
        {
            IP = iP;
            Rack = rack;
            Slot = slot;

        }
        public void Connect()
        {
            var repl = _client.SendRequest(new LinePacketConnectionRequest
            {
                Intent = LinePacketIntent.Connection,
                Ip = IP,
                Rack = this.Rack,
                SentTime = DateTime.Now,
                Slot = this.Slot,
                ConnectionIntent = LinePacketConnectionIntent.Connect
            });
            if (repl is not null && repl is LinePacketReplyStationConnection && ((LinePacketReplyStationConnection)repl).State == LinePacketReplyConnectionState.Connected)
            {
                _IsConnected = true;
            }
        }

        public void Disconnect()
        {
            var repl = _client.SendRequest(new LinePacketConnectionRequest
            {
                Intent = LinePacketIntent.Connection,
                Ip = IP,
                Rack = this.Rack,
                SentTime = DateTime.Now,
                Slot = this.Slot,
                ConnectionIntent = LinePacketConnectionIntent.Disconnect
            });
            if (repl is not null && repl is LinePacketReplyStationConnection && ((LinePacketReplyStationConnection)repl).State == LinePacketReplyConnectionState.Disconnected)
            {
                _IsConnected = false;
            }
        }

        public bool GetBit(int db, int index, int bit)
        {
            var repl = _client.SendRequest(new LinePacketReadRequest
            {
                Db = db,
                Index = index,
                Bit = bit,
                Ip = IP,
                Rack = this.Rack,
                SentTime = DateTime.Now,
                Slot = this.Slot,
                Intent = LinePacketIntent.Read
            });
            if (repl is LinePacketReplyRead)
            {
                return ((LinePacketReplyRead)repl).BitValue;
            

            }
            else if (repl is LinePacketReplyError)
            {
                throw new Exception(((LinePacketReplyError)repl).ErrorText);
            }
            else { return false; }
        }

        public byte[] GetBytes(int db, int index, int size)
        {     
            var repl = _client.SendRequest(new LinePacketReadRequest
            {
                Db = db,
                Index = index,
                Size = size,
                Ip = IP,
                Rack = this.Rack,
                SentTime = DateTime.Now,
                Slot = this.Slot,
                Intent = LinePacketIntent.Read
            });
            if (repl is LinePacketReplyRead)
            {
                return ((LinePacketReplyRead)repl).Data;
            }
            else if (repl is LinePacketReplyError)
            {
                throw new Exception(((LinePacketReplyError)repl).ErrorText);
            }
            else { return new byte[0]; }
        }

        public void SetBit(int db, int index, int bit, bool val)
        {
            var repl = _client.SendRequest(new LinePacketWriteRequest
            {
                Db = db,
                Index = index,
                Bit = (byte)bit,
                BitValue = val,
                Ip = IP,
                Rack = this.Rack,
                SentTime = DateTime.Now,
                Slot = this.Slot,
                Intent = LinePacketIntent.Write
            });
            if (repl is LinePacketReplyError)
            {
                throw new Exception(((LinePacketReplyError)repl).ErrorText);
            }
        }

        public void SetBytes(int db, int index, int size, byte[] bytes)
        {
            var repl = _client.SendRequest(new LinePacketWriteRequest
            {
                Db = db,
                Index = index,
                Data = bytes,                
                Ip = IP,
                Rack = this.Rack,
                SentTime = DateTime.Now,
                Slot = this.Slot,
                Intent = LinePacketIntent.Write
            });
            if (repl is LinePacketReplyError)
            {
                throw new Exception(((LinePacketReplyError)repl).ErrorText);
            }
        }
    }
}
