using Microsoft.AspNetCore.Identity;

namespace prg1.Models
{
    public class AppUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }

        
        public string ImagePath { get; set; } = "/img/default-user.png";
    }
}