using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ShoppingListAPI.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ShoppingListAPI.Services.LineItem.Queries
{
    public class GetLineItemByIdAndShoppingListIdQuery : IRequest<IResult>
    {
        public int Id { get; set; }

        public int ShoppingListId { get; set; }

        public string UserId { get; set; }
    }

    public class GetLineItemByIdAndShoppingListIdQueryHandler : IRequestHandler<GetLineItemByIdAndShoppingListIdQuery, IResult>
    {
        private readonly ApplicationDbContext _dbContext;

        private readonly ILogger<GetLineItemByIdAndShoppingListIdQueryHandler> _logger;

        public GetLineItemByIdAndShoppingListIdQueryHandler(ApplicationDbContext dbContext, ILogger<GetLineItemByIdAndShoppingListIdQueryHandler> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<IResult> Handle(GetLineItemByIdAndShoppingListIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // TODO: Should we check if the shopping list exists for the user?
                
                var entity = await _dbContext.LineItems
                    .Include(p => p.ShoppingList)
                    .Include(p => p.Product)
                    .ThenInclude(p => p.Category)
                    .Include(p => p.UnitOfMeasure)
                    .Where(p => p.Id.Equals(request.Id) && p.ShoppingListId.Equals(request.ShoppingListId) && p.ShoppingList.UserId.Equals(request.UserId))
                    .ProjectToDTO()
                    .SingleOrDefaultAsync(cancellationToken);

                if (entity == null)
                    return Result.NotFound();

                return Result.Ok(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error handling request: {request}", request);
                return Result.ServerError();
            }
        }
    }
}
