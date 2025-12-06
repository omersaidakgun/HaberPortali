using System.ComponentModel.DataAnnotations;

namespace prg1.ViewModels
{
    public class UserModel
    {
        public string Id { get; set; }

        [Display(Name = "Ad Soyad")]
        [Required(ErrorMessage = "Ad Soyad Giriniz!")]
        public string FullName { get; set; }

        [Display(Name = "Kullanıcı Adı")]
        [Required(ErrorMessage = "Kullanıcı Adı Giriniz!")]
        public string UserName { get; set; }

        [Display(Name = "E-Posta")]
        [Required(ErrorMessage = "E-Posta Giriniz!")]
        [EmailAddress(ErrorMessage = "Geçerli bir mail giriniz")]
        public string Email { get; set; }

        [Display(Name = "Parola")]
        
        public string Password { get; set; }

        public string Role { get; set; }
        public string? ImagePath { get; set; }
    }
}