using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Bookify.Models;

namespace Bookify.Controllers
{
    [ApiController]
    [Route("api/[controller]")] 
    public class ReviewsController : ControllerBase
    {
        private readonly ApplicationDb _context;

        // Injecte le contexte de base de données
        public ReviewsController(ApplicationDb context)
        {
            _context = context;
        }

        // GET: api/Reviews
        // Récupère toutes les reviews
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Review>>> GetReviews()
        {
            return await _context.Reviews.ToListAsync();
        }

        // GET: api/Reviews/{id}
        // Récupère une review par son identifiant
        [HttpGet("{id}")]
        public async Task<ActionResult<Review>> GetReview(int id)
        {
            var review = await _context.Reviews.FindAsync(id);

            if (review == null)
                return NotFound(); // 404 si la review n'existe pas

            return review;
        }

        // POST: api/Reviews
        // Crée une nouvelle review
        [HttpPost]
        public async Task<ActionResult<Review>> PostReview(Review review)
        {
            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();

            // Renvoie 201 Created avec l'URL de la nouvelle review
            return CreatedAtAction(nameof(GetReview), new { id = review.Id }, review);
        }

        // PUT: api/Reviews/{id}
        // Met à jour une review existante
        [HttpPut("{id}")]
        public async Task<IActionResult> PutReview(int id, Review review)
        {
            if (id != review.Id)
                return BadRequest(); // 400 si l'ID ne correspond pas

            _context.Entry(review).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Reviews.Any(r => r.Id == id))
                    return NotFound(); 
                else
                    throw;
            }

            return NoContent(); 
        }

        // DELETE: api/Reviews/{id}
        // Supprime une review
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReview(int id)
        {
            var review = await _context.Reviews.FindAsync(id);

            if (review == null)
                return NotFound();

            _context.Reviews.Remove(review);
            await _context.SaveChangesAsync();

            return NoContent(); 
        }
    }
}
