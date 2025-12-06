using prg1.Models;

namespace prg1.Repositories
{
    public class CategoryRepository : GenericRepository<Category>
    {
        public CategoryRepository(AppDbContext context) : base(context)
        {
        }
    }
}