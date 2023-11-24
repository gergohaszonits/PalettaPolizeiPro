using PalettaPolizeiPro.Data.Stations;

namespace PalettaPolizeiPro.Services
{
    public interface IPLCLayer
    {
        bool IsConnected { get; }
        void SetStation(Station station);//construcor should have the same parameter
        void SetBytes(int db, int index, int size, byte[] bytes);
        void SetBit(int db, int index, int bit, bool val);
        byte[] GetBytes(int db, int index, int size);
        bool GetBit(int db, int index, int bit);
        bool Connect();
        bool Disconnect();
    }
}

//help me please
//i need help 
//i want to be happy :(
//will i ever be happy?
// -no
