#nullable disable
using PalettaPolizeiPro;
using PalettaPolizeiPro.Data;
using Sharp7;

namespace PalettaPolizeiPro.Services.PLC
{
    public class S7PLC : IPLCLayer
    {
        public string IP { get; private set; }
        public int Rack { get; private set; }
        public int Slot { get; private set; }

        private S7Client _client;
        public S7PLC(string iP, int rack, int slot)
        {
            IP = iP;
            Rack = rack;
            Slot = slot;
            _client = new S7Client();
        }
        ~S7PLC() 
        {
            Disconnect();
        }

        public bool IsConnected => _client.Connected;

        public void Connect()
        {
            int res = _client.ConnectTo(IP, Rack, Slot);
            if (res != 0)
            {
                throw new Exception(_client.ErrorText(res));
            }
        }
        public void Disconnect()
        {
            _client.Disconnect();
        }

        public bool GetBit(int db, int index, int bit)
        {
            bool val = false;
            byte[] b = new byte[1];
            lock (_client)
            {   
                int error = 0;
                error = _client.ReadArea(S7Area.DB, db, index * 8 + bit, 1, S7WordLength.Bit, b);
                if (error != 0)
                {
                    throw new Exception(_client.ErrorText(error));
                }
            }
            val = b[0] == 1 ? true : false;
            return val;
        }

        public byte[] GetBytes(int db, int index, int size)
        {
            byte[] bytes = new byte[size];
            lock (_client)
            {
                int error = 0;
                error = _client.DBRead(db, index, size, bytes);
                if (error != 0)
                {
                    throw new Exception(_client.ErrorText(error));
                }
            }
            return bytes;
        }

        public void SetBit(int db, int index, int bit, bool val)
        {
            lock (_client)
            {
                byte b = (byte)(val == true ? 1 : 0);
                int error = 0;
                error = _client.WriteArea(S7Area.DB, db, index * 8 + bit, 1, S7WordLength.Bit, new byte[] { b });
                if (error != 0)
                {
                    throw new Exception(_client.ErrorText(error));
                }
            }
        }

        public void SetBytes(int db, int index, int size, byte[] bytes)
        {
            lock (_client)
            {
                int error = 0;
                error = _client.DBWrite(db, index, size, bytes);
                if (error != 0)
                {
                    throw new Exception(_client.ErrorText(error));
                }
            }
        }
    }
}
