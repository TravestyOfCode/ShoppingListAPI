using ShoppingListAPI.Data;

namespace ShoppingListAPI.Models
{
    public class LineItemDTO
    {
        public int ShoppingListId { get; set; }

        public bool IsCompleted { get; set; }

        public int ProductId { get; set; }

        public decimal Quantity { get; set; }

        public int UnitOfMeasureId { get; set; }

        internal LineItem AsLineItem() => new()
        {
            ShoppingListId = ShoppingListId,
            IsCompleted = IsCompleted,
            ProductId = ProductId,
            Quantity = Quantity,
            UnitOfMeasureId = UnitOfMeasureId
        };

        internal void MapTo(LineItem line)
        {
            if (line == null)
                return;

            line.IsCompleted = IsCompleted;
            line.ProductId = ProductId;
            line.Quantity = Quantity;
            line.UnitOfMeasureId = UnitOfMeasureId;
        }
    }
}
