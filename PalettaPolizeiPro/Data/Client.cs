using PalettaPolizeiPro.Data.Stations;

namespace PalettaPolizeiPro.Data
{
    public class Client
    {
        public User? User { get; set; }
        public Eks? Eks { get; set; }
        public Station? Station { get; set; }

        public string? NameToShow
        {
            get
            {
                if (User is not null)
                {
                    return User.Username;
                }
                else if (Eks is not null)
                {
                    return Eks.WorkerID;
                }
                return null;
            }
        }
    }
}
