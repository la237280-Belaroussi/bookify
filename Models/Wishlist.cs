using System.Collections.Generic;

public class User
{
    public int UserId { get; set; }
    public string Username { get; set; }
    public string PasswordHash { get; set; }

    public ICollection<WishlistItem> Wishlist { get; set; }
    public ICollection<ReadBook> ReadBooks { get; set; }
}

public class Book
{
    public int BookId { get; set; }
    public string Title { get; set; }
    public string Author { get; set; }

    public ICollection<WishlistItem> WishlistItems { get; set; }
    public ICollection<ReadBook> ReadBooks { get; set; }
}

public class WishlistItem
{
    public int WishlistItemId { get; set; }
    public int UserId { get; set; }
    public int BookId { get; set; }

    public User User { get; set; }
    public Book Book { get; set; }
}

public class ReadBook
{
    public int ReadBookId { get; set; }
    public int UserId { get; set; }
    public int BookId { get; set; }

    public User User { get; set; }
    public Book Book { get; set; }
}
