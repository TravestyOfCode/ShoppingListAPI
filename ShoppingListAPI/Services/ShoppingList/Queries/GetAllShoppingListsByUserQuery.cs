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
    public class GetAllShoppingListsByUserQuery : IRequest<IResult>
    {
        public string UserId { get; set; }
    }

    public class GetAllShoppingListByUserQueryHandler : IRequestHandler<GetAllShoppingListsByUserQuery, IResult>
    {
        private readonly ApplicationDbContext _dbContext;

        private readonly ILogger<GetAllShoppingListByUserQueryHandler> _logger;

        public GetAllShoppingListByUserQueryHandler(ApplicationDbContext dbContext, ILogger<GetAllShoppingListByUserQueryHandler> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<IResult> Handle(GetAllShoppingListsByUserQuery request, CancellationToken cancellationToken)
        {
            try
            {
                return Result.Ok(
                    await _dbContext.ShoppingLists
                                    .Where(p => p.UserId.Equals(request.UserId))
                                    .ProjectToDTO()
                                    .ToListAsync(cancellationToken));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error handling request: {request}", request);
                return Result.ServerError();
            }
        }
    }
}
