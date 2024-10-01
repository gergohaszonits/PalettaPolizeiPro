using PalettaPolizeiPro.Services.PLC;

namespace ProductionLineSimulation;

public class SimulationPlcLayer : IPLCLayer
{
    public string IP { get;  set; }

    public int Rack { get;  set; }

    public int Slot { get;  set; }
    public bool RemoteConnected = false;
    public SimulationPlcLayer(string ip, int rack, int slot)
    {
        IP = ip;
        Rack = rack;
        Slot = slot;
    }
    public SimulationPlcLayer()
    {
       
    }

    public void Connect()
    {
        RemoteConnected = true;
    }

    public void Disconnect()
    {
        RemoteConnected = false;
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
    object _lock = new object();

    List<Query> Queries = new List<Query>();

    public bool IsConnected => true;

    public byte[] GetBytes(int db, int index, int size)
    {
        lock (_lock)
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
        lock (_lock)
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
    private Query Extend(int db, int index, int size, Query? query)
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

