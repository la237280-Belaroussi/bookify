using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Book> Books { get; set; }
    public DbSet<WishlistItem> Wishlist { get; set; }
    public DbSet<ReadBook> ReadBooks { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseMySql(optionsBuilder.Options.FindExtension<Microsoft.EntityFrameworkCore.MySql.Infrastructure.Internal.MySqlOptionsExtension>().ConnectionString,
            new MySqlServerVersion(new Version(8, 0, 33)));
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<WishlistItem>()
            .HasKey(w => w.WishlistItemId);

        modelBuilder.Entity<ReadBook>()
            .HasKey(r => r.ReadBookId);

        modelBuilder.Entity<WishlistItem>()
            .HasOne(w => w.User)
            .WithMany(u => u.Wishlist)
            .HasForeignKey(w => w.UserId);

        modelBuilder.Entity<WishlistItem>()
            .HasOne(w => w.Book)
            .WithMany(b => b.WishlistItems)
            .HasForeignKey(w => w.BookId);

        modelBuilder.Entity<ReadBook>()
            .HasOne(r => r.User)
            .WithMany(u => u.ReadBooks)
            .HasForeignKey(r => r.UserId);

        modelBuilder.Entity<ReadBook>()
            .HasOne(r => r.Book)
            .WithMany(b => b.ReadBooks)
            .HasForeignKey(r => r.BookId);
    }
}
