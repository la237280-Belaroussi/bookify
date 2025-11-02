using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Bookify.Models
{
    /// <summary>
    /// Classe Gender reéprésentant un genre littéraire dans l'application
    /// </summary>
    public class Gender
    {
        // Attributs de la classe Gender
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "Le nom du genre est obligatoire.")]
        public string Name { get; set; }
        // Navigation property for related books
        public ICollection<Book> Books { get; set; }
    }
}
