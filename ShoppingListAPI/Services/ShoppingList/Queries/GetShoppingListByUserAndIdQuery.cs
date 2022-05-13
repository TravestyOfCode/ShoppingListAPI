using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ShoppingListAPI.Data;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ShoppingListAPI.Services.ShoppingList.Queries
{
    public class GetShoppingListByUserAndIdQuery : IRequest<IResult>
    {
        public string UserId { get; set; }

        public int Id { get; set; }
    }

    public class GetShoppingListByUserAndIdQueryHandler : IRequestHandler<GetShoppingListByUserAndIdQuery, IResult>
    {
        private readonly ApplicationDbContext _dbContext;

        private readonly ILogger<GetShoppingListByUserAndIdQueryHandler> _logger;

        public GetShoppingListByUserAndIdQueryHandler(ApplicationDbContext dbContext, ILogger<GetShoppingListByUserAndIdQueryHandler> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<IResult> Handle(GetShoppingListByUserAndIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var entity = await _dbContext.ShoppingLists
                                             .Where(p => p.Id.Equals(request.Id) && p.UserId.Equals(request.UserId))
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
