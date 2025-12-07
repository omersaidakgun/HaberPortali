using System.ComponentModel.DataAnnotations; 

namespace prg1.Models
{
    public class Todo : BaseEntity
    {
        
        [Required(ErrorMessage = "Lütfen bir başlık giriniz!")]
        public string Title { get; set; }

        
        public string? Description { get; set; }

        public int IsOK { get; set; }
    }
}