using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Bookify.Models;
using Bookify.Data;

namespace Bookify.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LivreLuController : ControllerBase
    {
        private readonly AppDBContext _context;

        public LivreLuController(AppDBContext context)
        {
            _context = context;
        }

        // Récupérer tous les livres lus
        [HttpGet]
        public async Task<IActionResult> GetLivreLus()
        {
            var livresLus = await _context.LivreLu
                .Include(r => r.Book)
                .Select(r => new
                {
                    r.Id,
                    r.BookId,
                    r.Book.Title,
                    r.Book.Author,
                    r.Book.ISBN,
                    r.Book.Price,
                    r.Book.Publisher,
                    r.DateLu
                })
                .ToListAsync();

            return Ok(livresLus);
        }

        // Ajouter un livre à la liste des livres lus
        [HttpPost("{bookId:int}")]
        public async Task<IActionResult> AddLivreLu(int bookId)
        {
            var bookExists = await _context.Books.AnyAsync(b => b.Id == bookId);
            if (!bookExists)
                return NotFound(new { Message = "Livre introuvable." });

            var alreadyInList = await _context.LivreLu.AnyAsync(r => r.BookId == bookId);
            if (alreadyInList)
                return BadRequest(new { Message = "Ce livre est déjà marqué comme lu." });

            var livreLu = new LivreLu { BookId = bookId };
            _context.LivreLu.Add(livreLu);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Livre ajouté avec succès à la liste des livres lus." });
        }

        // Supprimer un livre de la liste des livres lus
        [HttpDelete("{bookId:int}")]
        public async Task<IActionResult> RemoveLivreLu(int bookId)
        {
            var item = await _context.LivreLu.FirstOrDefaultAsync(r => r.BookId == bookId);
            if (item == null)
                return NotFound(new { Message = "Livre non présent dans la liste des livres lus." });

            _context.LivreLu.Remove(item);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Livre retiré avec succès de la liste des livres lus." });
        }
    }
}
