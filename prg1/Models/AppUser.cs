using Microsoft.AspNetCore.Identity;

namespace prg1.Models
{
    public class AppUser : IdentityUser
    {
        public string FirstName { get; set; } // İsim
        public string LastName { get; set; }  // Soyisim

        public string? ImagePath { get; set; } // Profil Resmi Yolu
    }
}