using ShoppingListAPI.Data;
using System.ComponentModel.DataAnnotations;

namespace ShoppingListAPI.Models
{
    public class UnitOfMeasureDTO
    {
        [Required]
        [MaxLength(6)]
        public string Name { get; set; }

        internal UnitOfMeasure AsUnitOfMeasure() => new UnitOfMeasure() { Name = Name };

        internal void MapTo(UnitOfMeasure unitOfMeasure)
        {
            if (unitOfMeasure == null)
                return;

            unitOfMeasure.Name = Name;
        }
    }
}
