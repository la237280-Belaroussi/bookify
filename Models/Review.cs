using System.ComponentModel.DataAnnotations;

namespace Bookify.Models
{
    // Classe conten
    public class Review
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int IdLivre { get; set; }

        [Required]
        public int IdUtilisateur { get; set; }

        [Required]
        [Range(0, 5)]
        public int Note { get; set; }  

        [MaxLength(100)]
        public string Titre { get; set; } 

        [Required]
        [MaxLength(1000)]
        public string Avis { get; set; } 
    }
}
