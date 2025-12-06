using System.ComponentModel.DataAnnotations;

namespace prg1.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }

        public virtual ICollection<Haber> Haberler { get; set; }
    }
}