using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ShoppingListAPI.Data
{
    public class LineItem
    {
        public int Id { get; set; }

        public int ShoppingListId { get; set; }

        public ShoppingList ShoppingList { get; set; }

        public bool IsCompleted { get; set; }

        public int ProductId { get; set; }

        public Product Product { get; set; }

        public decimal Quantity { get; set; }

        public int UnitOfMeasureId { get; set; }

        public UnitOfMeasure UnitOfMeasure { get; set; }
    }

    public class LineItemConfiguration : IEntityTypeConfiguration<LineItem>
    {
        public void Configure(EntityTypeBuilder<LineItem> builder)
        {
            builder.Property(p => p.Quantity)
                .HasColumnType("decimal(9,3");
        }
    }
}
