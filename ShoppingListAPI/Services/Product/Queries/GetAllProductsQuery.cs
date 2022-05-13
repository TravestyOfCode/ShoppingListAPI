using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ShoppingListAPI.Data;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ShoppingListAPI.Services.Product.Queries
{
    public class GetAllProductsQuery : IRequest<IResult>
    {
        
    }

    public class GetAllProductsQueryHandler : IRequestHandler<GetAllProductsQuery, IResult>
    {
        private readonly ApplicationDbContext _dbContext;

        private readonly ILogger<GetAllProductsQueryHandler> _logger;

        public GetAllProductsQueryHandler(ApplicationDbContext dbContext, ILogger<GetAllProductsQueryHandler> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<IResult> Handle(GetAllProductsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                return Result.Ok(
                    await _dbContext.Products
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
