using MudBlazor;
using PalettaPolizeiPro.Data;
using PalettaPolizeiPro.Services.PLC;
using System.Drawing;
using System.Runtime.InteropServices;

namespace PalettaPolizeiPro.Services.Simulation
{
    public class SimulationPlcLayer : IPLCLayer
    {
        public string IP { get; private set; }

        public int Rack { get; private set; }

        public int Slot { get; private set; }
        public bool _isConnected = false;
        public SimulationPlcLayer(string ip, int rack, int slot)
        {
            IP = ip;
            Rack = rack;
            Slot = slot;
        }

        public void Connect()
        {
            _isConnected = true;
        }

        public void Disconnect()
        {
            _isConnected = false;
        }
        class Query
        {
            public byte[] Bytes;
            public int DB;
            public Query(byte[] bytes, int dB)
            {
                Bytes = bytes;
                DB = dB;
            }
        }
        object busy = new object();

        List<Query> Queries = new List<Query>();

        public bool IsConnected => true;



        public List<Notification> Notifications { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public byte[] GetBytes(int db, int index, int size)
        {
            lock (busy)
            {
                Query? query = Queries.FirstOrDefault(x => x.DB == db);

                query = Extend(db, index, size, query);

                byte[] final = new byte[size];
                Array.Copy(query.Bytes, index, final, 0, size);

                return final;
            }
        }

        public void SetBytes(int db, int index, int size, byte[] bytes)
        {
            lock (busy)
            {
                Query? query = Queries.FirstOrDefault(x => x.DB == db);

                query = Extend(db, index, size, query);
                
                int j = 0;
                for (int i = index; i < index + size; i++, j++)
                {
                    query.Bytes[i] = bytes[j];
                }
            }
        }
        private Query Extend(int db, int index, int size, Query query)
        {
            if (query == null)
            {
                query = new Query(new byte[index + size], db);
                Queries.Add(query);
            }

            else if (query.Bytes.Length < index + size)
            {
                byte[] mybytes = new byte[index + size];
                Array.Copy(query.Bytes, mybytes, query.Bytes.Length);
                Array.Copy(new byte[index + size - query.Bytes.Length], 0, mybytes, query.Bytes.Length, index + size - query.Bytes.Length);
                query.Bytes = mybytes;
            }
            return query;
        }
        public void SetBit(int db, int index, int bit, bool val)
        {
            byte b = GetBytes(db, index, 1)[0];
            b = (byte)(b | Convert.ToByte(val) << bit);
            SetBytes(db, index, 1, new byte[] { b });
        }
        

        public bool GetBit(int db, int index, int bit)
        {
            byte b = GetBytes(db, index, 1)[0];
            return (b & 1 << bit) != 0;
        }


    }
}
