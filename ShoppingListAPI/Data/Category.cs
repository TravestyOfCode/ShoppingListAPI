using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ShoppingListAPI.Data
{
    public class Category
    {        
        public int Id { get; set; }

        public string Name { get; set; }
    }

    public class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.Property(p => p.Name)
                .IsRequired(true)
                .HasMaxLength(32);

            builder.HasIndex(p => p.Name)
                .IsUnique(true)
                .IsClustered(false);
        }
    }
}
