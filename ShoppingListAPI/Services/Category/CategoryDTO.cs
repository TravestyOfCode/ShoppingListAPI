using System.Linq;

namespace ShoppingListAPI.Services.Category
{
    public class CategoryDTO
    {
        public int Id { get; set; }

        public string Name { get; set; }
    }

    public static class CategoryDTOMappingExtensions
    {
        public static IQueryable<CategoryDTO> ProjectToDTO(this IQueryable<Data.Category> query)
        {
            if (query == null)
                return null;

            return query.Select(p => new CategoryDTO()
            {
                Id = p.Id,
                Name = p.Name
            });
        }

        public static CategoryDTO AsCategoryDTO(this Data.Category category)
        {
            if (category == null)
                return null;

            return new() { Id = category.Id, Name = category.Name };
        }
    }
}
