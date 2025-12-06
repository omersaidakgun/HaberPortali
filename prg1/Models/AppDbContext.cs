using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace prg1.Models
{
    
    public class AppDbContext : IdentityDbContext<AppUser, AppRole, string>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        
        public DbSet<Category> Categories { get; set; } 
        public DbSet<News> News { get; set; }
        public DbSet<Todo> Todos { get; set; }
    }
}