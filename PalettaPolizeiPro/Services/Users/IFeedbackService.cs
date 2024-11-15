using PalettaPolizeiPro.Data.Users;

namespace PalettaPolizeiPro.Services.Users
{
    public interface IFeedbackService
    {
        public void Add(Feedback feedback);
        public List<Feedback> GetAll();
        public List<Feedback> GetWhere(Func<Feedback, bool> predicate);
    }
}
