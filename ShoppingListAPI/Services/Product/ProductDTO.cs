using System.Linq;

namespace ShoppingListAPI.Services.Product
{
    public class ProductDTO
    {
        public int Id { get; set; }

        public int CategoryId { get; set; }

        public string CategoryName { get; set; }

        public string Name { get; set; }
    }

    public static class ProductDTOMappingExtensions
    {
        public static IQueryable<ProductDTO> ProjectToDTO(this IQueryable<Data.Product> query)
        {
            if (query == null)
                return null;

            return query.Select(p => new ProductDTO()
            {
                Id = p.Id,
                CategoryId = p.CategoryId,
                CategoryName = p.Category == null ? null : p.Category.Name,
                Name = p.Name
            });
        }

        public static ProductDTO AsProductDTO(this Data.Product product)
        {
            if (product == null)
                return null;

            return new()
            {
                Id = product.Id,
                CategoryId = product.CategoryId,
                CategoryName = product.Category == null ? null : product.Category.Name,
                Name = product.Name
            };
        }
    }
}
