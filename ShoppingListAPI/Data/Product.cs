using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ShoppingListAPI.Data
{
    public class Product
    {
        public int Id { get; set; }

        public int CategoryId { get; set; }

        public Category Category { get; set; }

        public string Name { get; set; }
    }

    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.Property(p => p.Name)
                .IsRequired(true)
                .HasMaxLength(64);

            builder.HasIndex(p => new { p.Name, p.CategoryId })
                .IsUnique(true)
                .IsClustered(false);
                
        }
    }
}
