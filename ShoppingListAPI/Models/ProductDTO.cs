using ShoppingListAPI.Data;

namespace ShoppingListAPI.Models
{
    public class ProductDTO
    {
        public int CategoryId { get; set; }

        public string Name { get; set; }

        internal Product AsProduct() => new() { Name = Name, CategoryId = CategoryId };

        internal void MapTo(Product product)
        {
            if (product == null)
                return;

            product.Name = Name;
            product.CategoryId = CategoryId;
        }
    }
}
