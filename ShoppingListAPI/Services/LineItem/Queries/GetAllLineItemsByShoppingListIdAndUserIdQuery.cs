using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ShoppingListAPI.Data;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ShoppingListAPI.Services.LineItem.Queries
{
    public class GetAllLineItemsByShoppingListIdAndUserIdQuery : IRequest<IResult>
    {
        public int Id { get; set; }

        public string UserId { get; set; }
    }

    public class GetAllLineItemsByShoppingListIdAndUserIdQueryHandler : IRequestHandler<GetAllLineItemsByShoppingListIdAndUserIdQuery, IResult>
    {
        private readonly ApplicationDbContext _dbContext;

        private readonly ILogger<GetAllLineItemsByShoppingListIdAndUserIdQueryHandler> _logger;

        public GetAllLineItemsByShoppingListIdAndUserIdQueryHandler(ApplicationDbContext dbContext, ILogger<GetAllLineItemsByShoppingListIdAndUserIdQueryHandler> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<IResult> Handle(GetAllLineItemsByShoppingListIdAndUserIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // TODO: Should we check if the shopping list exists for the user first and
                // return an NotFound, or just return empty list as current?
                var entities = await _dbContext.LineItems
                    .Include(p => p.ShoppingList)
                    .Include(p => p.Product)
                    .ThenInclude(p => p.Category)
                    .Include(p => p.UnitOfMeasure)
                    .Where(p => p.ShoppingListId.Equals(request.Id) && p.ShoppingList.UserId.Equals(request.UserId))
                    .ProjectToDTO()
                    .ToListAsync(cancellationToken);

                return Result.Ok(entities);
             
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error handling request: {request}", request);
                return Result.ServerError();
            }
        }
    }
}
