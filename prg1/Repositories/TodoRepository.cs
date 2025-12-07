using prg1.Models;

namespace prg1.Repositories
{
    public class TodoRepository : GenericRepository<Todo>
    {
        public TodoRepository(AppDbContext context) : base(context)
        {
        }
    }
}
