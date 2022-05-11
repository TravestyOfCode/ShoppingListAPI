using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace ShoppingListAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Category> Categories { get; set; }

        public DbSet<LineItem> LineItems { get; set; }

        public DbSet<Product> Products { get; set; }

        public DbSet<ShoppingList> ShoppingLists { get; set; }

        public DbSet<UnitOfMeasure> UnitOfMeasures { get; set; }

        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {            
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}
