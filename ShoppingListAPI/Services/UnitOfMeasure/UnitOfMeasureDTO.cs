using System.Linq;

namespace ShoppingListAPI.Services.UnitOfMeasure
{
    public class UnitOfMeasureDTO
    {
        public int Id { get; set; }

        public string Name { get; set; }
    }

    public static class UnitOfMeasureDTOMappingExtensions
    {
        public static IQueryable<UnitOfMeasureDTO> ProjectToDTO(this IQueryable<Data.UnitOfMeasure> query)
        {
            if (query == null)
                return null;

            return query.Select(p => new UnitOfMeasureDTO()
            {
                Id = p.Id,
                Name = p.Name
            });            
        }

        public static UnitOfMeasureDTO AsUnitOfMeasureDTO(this Data.UnitOfMeasure unitOfMeasure)
        {
            if (unitOfMeasure == null)
                return null;

            return new() { Id = unitOfMeasure.Id, Name = unitOfMeasure.Name };
        }
    }
}
