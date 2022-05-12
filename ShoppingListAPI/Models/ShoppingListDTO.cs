using ShoppingListAPI.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace ShoppingListAPI.Models
{
    public class ShoppingListDTO
    {
        [MaxLength(32)]
        public string Title { get; set; }

        public DateTime? TripDate { get; set; }

        public bool IsCompleted { get; set; }

        public IEnumerable<LineItemDTO> LineItems { get; set; }

        internal ShoppingList AsShoppingList(string userId) => new()
        {
            UserId = userId,
            Title = Title,
            TripDate = TripDate,
            IsCompleted = IsCompleted,
            LineItems = LineItems.Select(p => new LineItem()
            {
                IsCompleted = p.IsCompleted,
                ProductId = p.ProductId,
                Quantity = p.Quantity,
                UnitOfMeasureId = p.UnitOfMeasureId
            }).ToList()
        };

        internal void MapTo(ShoppingList shoppingList)
        {
            if (shoppingList == null)
                return;

            shoppingList.Title = Title;
            shoppingList.TripDate = TripDate;
            shoppingList.IsCompleted = IsCompleted;
        }
    }
}
