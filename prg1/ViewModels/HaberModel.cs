using System.ComponentModel.DataAnnotations;

namespace prg1.ViewModels
{
    public class HaberModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Başlık Giriniz")]
        [Display(Name = "Haber Başlığı")]
        public string Baslik { get; set; }

        [Required(ErrorMessage = "İçerik Giriniz")]
        [Display(Name = "Haber İçeriği")]
        public string Icerik { get; set; }

        [Display(Name = "Haber Görseli")]
        public string? ResimYolu { get; set; }

        [Display(Name = "Kategori Seçiniz")]
        [Required(ErrorMessage = "Kategori Seçimi Zorunludur")]
        public int CategoryId { get; set; }

        public bool IsActive { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
    }
}