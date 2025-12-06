using System.ComponentModel.DataAnnotations;

namespace prg1.ViewModels
{
    public class CategoryModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Kategori Adı Giriniz")]
        [Display(Name = "Kategori Adı")]
        public string Name { get; set; }

        public bool IsActive { get; set; } = true;
    }
}