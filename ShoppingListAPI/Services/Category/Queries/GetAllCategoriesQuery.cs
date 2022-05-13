using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ShoppingListAPI.Data;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ShoppingListAPI.Services.Category.Queries
{
    public class GetAllCategoriesQuery : IRequest<IResult>
    {
        
    }

    public class GetAllCategoriesQueryHandler : IRequestHandler<GetAllCategoriesQuery, IResult>
    {
        private readonly ApplicationDbContext _dbContext;

        private readonly ILogger<GetAllCategoriesQueryHandler> _logger;

        public GetAllCategoriesQueryHandler(ApplicationDbContext dbContext, ILogger<GetAllCategoriesQueryHandler> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<IResult> Handle(GetAllCategoriesQuery request, CancellationToken cancellationToken)
        {
            try
            {
                return Result.Ok(
                    await _dbContext.Categories
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
