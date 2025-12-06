using prg1.Models;

namespace prg1.Repositories
{
    public class HaberRepository : GenericRepository<Haber>
    {
        public HaberRepository(AppDbContext context) : base(context)
        {
        }
    }
}