using ShoppingListAPI.Data;
using System.ComponentModel.DataAnnotations;

namespace ShoppingListAPI.Models
{
    public class CategoryDTO
    {
        [Required]
        [MaxLength(32)]
        public string Name { get; set; }

        internal Category AsCategory() => new Category() { Name = Name };

        internal void MapTo(Category category)
        {
            if (category == null)
                return;

            category.Name = Name;
        }
    }
}
