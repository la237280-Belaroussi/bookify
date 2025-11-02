using Microsoft.EntityFrameworkCore;

public class UserBookService
{
	private readonly AppDbContext _context;

	public UserBookService(AppDbContext context)
	{
		_context = context;
	}

	// Ajout à la wishlist
	public async Task AddToWishlist(int userId, int bookId)
	{
		if (!await _context.Wishlist.AnyAsync(w => w.UserId == userId && w.BookId == bookId))
		{
			_context.Wishlist.Add(new WishlistItem { UserId = userId, BookId = bookId });
			await _context.SaveChangesAsync();
		}
	}

	// Suppression de la wishlist
	public async Task RemoveFromWishlist(int userId, int bookId)
	{
		var item = await _context.Wishlist.FirstOrDefaultAsync(w => w.UserId == userId && w.BookId == bookId);
		if (item != null)
		{
			_context.Wishlist.Remove(item);
			await _context.SaveChangesAsync();
		}
	}

	// Récupérer la wishlist
	public async Task<List<Book>> GetWishlist(int userId)
	{
		return await _context.Wishlist
			.Where(w => w.UserId == userId)
			.Include(w => w.Book)
			.Select(w => w.Book)
			.ToListAsync();
	}

	// Ajouter livres lus
	public async Task AddToReadBooks(int userId, int bookId)
	{
		if (!await _context.ReadBooks.AnyAsync(r => r.UserId == userId && r.BookId == bookId))
		{
			_context.ReadBooks.Add(new ReadBook { UserId = userId, BookId = bookId });
			await _context.SaveChangesAsync();
		}
	}

	//Supprimer livres lus
	public async Task RemoveFromReadBooks(int userId, int bookId)
	{
		var item = await _context.ReadBooks.FirstOrDefaultAsync(r => r.UserId == userId && r.BookId == bookId);
		if (item != null)
		{
			_context.ReadBooks.Remove(item);
			await _context.SaveChangesAsync();
		}
	}

	//Récupérer livres lus
	public async Task<List<Book>> GetReadBooks(int userId)
	{
		return await _context.ReadBooks
			.Where(r => r.UserId == userId)
			.Include(r => r.Book)
			.Select(r => r.Book)
			.ToListAsync();
	}
}
