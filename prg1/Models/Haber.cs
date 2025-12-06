using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace prg1.Models
{
    public class Haber
    {
        [Key]
        public int Id { get; set; }

        public string Baslik { get; set; }
        public string Icerik { get; set; }
        public string? ResimYolu { get; set; }

        public DateTime Created { get; set; } = DateTime.Now;
        public DateTime Updated { get; set; } = DateTime.Now;
        public bool IsActive { get; set; } = true;

        public int CategoryId { get; set; }
        [ForeignKey("CategoryId")]
        public virtual Category Category { get; set; }
    }
}