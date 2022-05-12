using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;

namespace ShoppingListAPI.Data
{
    public class ShoppingList
    {
        public int Id { get; set; }

        public string UserId { get; set; }

        public IdentityUser User { get; set; }

        public string Title { get; set; }

        public DateTime? TripDate { get; set; }

        public bool IsCompleted { get; set; }

        public List<LineItem> LineItems { get; set; }
    }

    public class ShoppingListConfiguration : IEntityTypeConfiguration<ShoppingList>
    {
        public void Configure(EntityTypeBuilder<ShoppingList> builder)
        {
            builder.Property(p => p.Title)
                .IsRequired(false)
                .HasMaxLength(32);
        }
    }
}
