using Microsoft.EntityFrameworkCore;
using prg1.Models;

namespace prg1.Repositories
{
    
    public class HaberRepository : GenericRepository<Haber>
    {
        private readonly AppDbContext _context;

        public HaberRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        
        public async Task<List<Haber>> GetHaberlerWithKategoriAsync()
        {
            return await _context.News
                                 .Include(x => x.Category) 
                                 .OrderByDescending(x => x.Created) 
                                 .ToListAsync();
        }
    }
}