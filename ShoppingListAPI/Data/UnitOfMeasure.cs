using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ShoppingListAPI.Data
{
    public class UnitOfMeasure
    {
        public int Id { get; set; }

        public string Name { get; set; }
    }

    public class UnitOfMeasureConfiguration : IEntityTypeConfiguration<UnitOfMeasure>
    {
        public void Configure(EntityTypeBuilder<UnitOfMeasure> builder)
        {
            builder.Property(p => p.Name)
                .IsRequired(true)
                .HasMaxLength(6);

            builder.HasIndex(p => p.Name)
                .IsUnique(true)
                .IsClustered(false);
        }
    }
}
