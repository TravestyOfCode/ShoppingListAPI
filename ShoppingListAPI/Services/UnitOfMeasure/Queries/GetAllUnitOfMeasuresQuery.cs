using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ShoppingListAPI.Data;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ShoppingListAPI.Services.UnitOfMeasure.Queries
{
    public class GetAllUnitOfMeasuresQuery : IRequest<IResult>
    {
        
    }

    public class GetAllUnitOfMeasuresQueryHandler : IRequestHandler<GetAllUnitOfMeasuresQuery, IResult>
    {
        private readonly ApplicationDbContext _dbContext;

        private readonly ILogger<GetAllUnitOfMeasuresQueryHandler> _logger;

        public GetAllUnitOfMeasuresQueryHandler(ApplicationDbContext dbContext, ILogger<GetAllUnitOfMeasuresQueryHandler> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<IResult> Handle(GetAllUnitOfMeasuresQuery request, CancellationToken cancellationToken)
        {
            try
            {
                return Result.Ok(
                    await _dbContext.UnitOfMeasures
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
