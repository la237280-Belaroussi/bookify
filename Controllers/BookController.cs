using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Bookify.Models;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Bookify.Controllers
{
    /// <summary>
    /// Controller pour gérer les opérations liées aux livres
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class BookController : ControllerBase
    {
        private readonly ApplicationDb _context;
        public BookController(ApplicationDb context)
        {
            _context = context;
        }

        /// <summary>
        /// Action pour récupérer tous les livres
        /// </summary>
        /// <returns>Une liste de livres</returns>
        [HttpGet] // GET: api/Books
        public async Task<IActionResult> GetAllBooks()
        {
            var books = await _context.Books.Include(b => b.GenderId).ToListAsync();
            return new JsonResult(books);
        }

        /// <summary>
        /// Action pour récupérer un livre par son ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Le livre voulu</returns>
        [HttpGet("{id}")] // GET: api/Books/5
        public async Task<IActionResult> GetBookById(int id)
        {
            var book = await _context.Books.Include(b => b.GenderId).FirstOrDefaultAsync(b => b.Id == id);
            if (book == null) return NotFound(new
            {
                Message = "Book not found"
            });
            return new JsonResult(book);
        }

        /// <summary>
        /// Action pour créer un nouveau livre
        /// </summary>
        /// <returns>Le livre créé avec son ID</returns>
        public async Task<IActionResult> Create([FromBody] Book book)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            _context.Books.Add(book);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetBookById), new { id = book.Id }, book);
        }

        /// <summary>
        /// Action pour mettre à jour un livre existant
        /// </summary>
        /// <returns></returns>
        [HttpPut("{id}")] // PUT: api/Books/5
        public async Task<IActionResult> Update(int id, [FromBody] Book book)
        {
            if (id != book.Id) return BadRequest(new { Message = "ID mismatch" });
            if (!ModelState.IsValid) return BadRequest(ModelState);
            _context.Entry(book).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Books.Any(b => b.Id == id))
                {
                    return NotFound(new { Message = "Book not found" });
                }
                else
                {
                    throw;
                }
            }
            return NoContent();
        }

        /// <summary>
        /// Action pour supprimer un livre
        /// </summary>
        [HttpDelete("{id}")] // DELETE: api/Books/5
        public async Task<IActionResult> Delete(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null) return NotFound(new { Message = "Book not found" });
            _context.Books.Remove(book);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
