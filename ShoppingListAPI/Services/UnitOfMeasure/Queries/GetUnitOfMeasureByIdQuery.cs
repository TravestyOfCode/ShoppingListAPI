using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ShoppingListAPI.Data;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ShoppingListAPI.Services.UnitOfMeasure.Queries
{
    public class GetUnitOfMeasureByIdQuery : IRequest<IResult>
    {
        public int Id { get; set; }
    }

    public class GetUnitOfMeasureByIdQueryHandler : IRequestHandler<GetUnitOfMeasureByIdQuery, IResult>
    {
        private readonly ApplicationDbContext _dbContext;

        private readonly ILogger<GetUnitOfMeasureByIdQueryHandler> _logger;

        public GetUnitOfMeasureByIdQueryHandler(ApplicationDbContext dbContext, ILogger<GetUnitOfMeasureByIdQueryHandler> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<IResult> Handle(GetUnitOfMeasureByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                return Result.Ok(
                    await _dbContext.UnitOfMeasures
                                    .ProjectToDTO()
                                    .SingleOrDefaultAsync(p => p.Id.Equals(request.Id), cancellationToken));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error handling request: {request}", request);
                return Result.ServerError();
            }
        }
    }
}
