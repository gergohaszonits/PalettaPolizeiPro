using PalettaPolizeiPro.Data;

namespace PalettaPolizeiPro.Services
{
    public interface IPLCLayer
    {
        string IP {get;}
        int Rack {get;}
        int Slot {get;}
        bool IsConnected { get; }
        void SetBytes(int db, int index, int size, byte[] bytes);
        void SetBit(int db, int index, int bit, bool val);
        byte[] GetBytes(int db, int index, int size);
        bool GetBit(int db, int index, int bit);
        void Connect();
        void Disconnect();
    }
}
