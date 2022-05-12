namespace ShoppingListAPI.Models
{
    public class LineItemDTO
    {
        public int ShoppingListId { get; set; }

        public bool IsCompleted { get; set; }

        public int ProductId { get; set; }

        public decimal Quantity { get; set; }

        public int UnitOfMeasureId { get; set; }
    }
}
