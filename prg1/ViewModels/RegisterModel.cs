using System.ComponentModel.DataAnnotations;

namespace prg1.ViewModels
{
    public class RegisterModel
    {
        [Display(Name = "Ad Soyad")]
        [Required(ErrorMessage = "Ad Soyad Giriniz!")]
        public string FullName { get; set; }

        [Display(Name = "Kullanıcı Adı")]
        [Required(ErrorMessage = "Kullanıcı Adı Giriniz!")]
        public string UserName { get; set; }

        [Display(Name = "E-Posta")]
        [Required(ErrorMessage = "E-Posta Giriniz!")]
        [EmailAddress]
        public string Email { get; set; }

        [Display(Name = "Parola")]
        [Required(ErrorMessage = "Parola Giriniz!")]
        public string Password { get; set; }

        [Display(Name = "Parola Tekrar")]
        [Compare("Password", ErrorMessage = "Parolalar eşleşmiyor!")]
        public string PasswordConfirm { get; set; }

        
        public IFormFile? PhotoFile { get; set; }
    }
}