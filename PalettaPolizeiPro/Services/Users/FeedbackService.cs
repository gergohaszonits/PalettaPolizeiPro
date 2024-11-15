using Microsoft.EntityFrameworkCore;
using PalettaPolizeiPro.Data.Users;
using PalettaPolizeiPro.Database;

namespace PalettaPolizeiPro.Services.Users
{
    public class FeedbackService : IFeedbackService
    {
        public void Add(Feedback feedback)
        {
            using (var context = new DatabaseContext())
            { 
                context.Feedbacks.Add(feedback);
                context.SaveChanges();
            }
        }

        public List<Feedback> GetAll()
        {
            using (var context = new DatabaseContext())
            {
                return context.Feedbacks.Include(x=>x.User).OrderBy(x=>x.Time).ToList();
            }
        }
        
        public List<Feedback> GetWhere(Func<Feedback, bool> predicate)
        {
            using (var context = new DatabaseContext())
            {
                return context.Feedbacks.Include(x => x.User).Where(predicate).ToList();
            }
        }
    }
}
