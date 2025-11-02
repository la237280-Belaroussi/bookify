using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Bookify.Models
{
    public class ApplicationDb : DbContext
    {
        public ApplicationDb(DbContextOptions<ApplicationDb> options)
            : base(options)
        {
        }
     
        public DbSet<Review> Reviews { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                foreach (var property in entityType.GetProperties())
                {
                    if (!property.IsPrimaryKey() && property.ClrType == typeof(string))
                    {
                        property.IsNullable = false;
                    }
                }
            }
        }
    }
}