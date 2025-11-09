using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bookify.Models
{
    public class Wishlist
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Book")]
        public int BookId { get; set; }
        public Book Book { get; set; } = null!;

        public DateTime DateAdded { get; set; } = DateTime.Now;
    }
}
