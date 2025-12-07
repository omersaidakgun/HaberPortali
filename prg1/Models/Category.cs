using System.ComponentModel.DataAnnotations;

namespace prg1.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Kategori adı zorunludur")]
        public string Name { get; set; }

        public bool IsActive { get; set; }

        
        public DateTime Created { get; set; } = DateTime.Now;
        public DateTime Updated { get; set; } = DateTime.Now;

        
        public virtual ICollection<Haber>? Haberler { get; set; }
    }
}