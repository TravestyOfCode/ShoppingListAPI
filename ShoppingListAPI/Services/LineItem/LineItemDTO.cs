using System.Collections.Generic;
using System.Linq;

namespace ShoppingListAPI.Services.LineItem
{
    public class LineItemDTO
    {
        public int Id { get; set; }

        public int ShoppingListId { get; set; }

        public bool IsCompleted { get; set; }

        public int ProductId { get; set; }

        public string ProductName { get; set; }

        public string ProductCategoryName { get; set; }

        public decimal Quantity { get; set; }

        public int UnitOfMeasureId { get; set; }

        public string UnitOfMeasureName { get; set; }
    }

    public static class LineItemDTOMappingExtensions
    {
        public static IQueryable<LineItemDTO> ProjectToDTO(this IQueryable<Data.LineItem> query)
        {
            if (query == null)
                return null;

            return query.Select(p => new LineItemDTO()
            {
                Id = p.Id,
                ShoppingListId = p.ShoppingListId,
                IsCompleted = p.IsCompleted,
                ProductId = p.ProductId,
                ProductName = p.Product != null ? p.Product.Name : null,
                ProductCategoryName = p.Product != null && p.Product.Category != null ? p.Product.Category.Name : null,
                Quantity = p.Quantity,
                UnitOfMeasureId = p.UnitOfMeasureId,
                UnitOfMeasureName = p.UnitOfMeasure != null ? p.UnitOfMeasure.Name : null
            });
        }

        public static LineItemDTO AsLineItemDTO(this Data.LineItem lineItem)
        {
            if (lineItem == null)
                return null;

            return new()
            {
                Id = lineItem.Id,
                ShoppingListId = lineItem.ShoppingListId,
                IsCompleted = lineItem.IsCompleted,
                ProductId = lineItem.ProductId,
                ProductName = lineItem.Product?.Name,
                ProductCategoryName = lineItem.Product?.Category?.Name,
                Quantity = lineItem.Quantity,
                UnitOfMeasureId = lineItem.UnitOfMeasureId,
                UnitOfMeasureName = lineItem.UnitOfMeasure?.Name
            };
        }
    }
}
