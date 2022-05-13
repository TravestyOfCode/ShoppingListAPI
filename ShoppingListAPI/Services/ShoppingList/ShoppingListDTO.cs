using System;
using System.Linq;

namespace ShoppingListAPI.Services.ShoppingList
{
    public class ShoppingListDTO
    {
        public int Id { get; set; }
        
        public string Title { get; set; }

        public DateTime? TripDate { get; set; }

        public bool IsCompleted { get; set; }

        //public List<LineItemDTO> LineItems { get; set; }
    }

    public static class ShoppingListDTOMappingExtensions
    {
        public static IQueryable<ShoppingListDTO> ProjectToDTO(this IQueryable<Data.ShoppingList> query)
        {
            if (query == null)
                return null;

            return query.Select(p => new ShoppingListDTO()
            {
                Id = p.Id,
                Title = p.Title,
                TripDate = p.TripDate,
                IsCompleted = p.IsCompleted,
                //LineItems = p.LineItems.ProjectToDTO()
            });
        }

        public static ShoppingListDTO AsShoppingListDTO(this Data.ShoppingList shoppingList)
        {
            if (shoppingList == null)
                return null;

            return new()
            {
                Id = shoppingList.Id,
                Title = shoppingList.Title,
                TripDate = shoppingList.TripDate,
                IsCompleted = shoppingList.IsCompleted,
                //LineItems = shoppingList.LineItems.AsLineItemDTO()
            };
        }
    }
}
