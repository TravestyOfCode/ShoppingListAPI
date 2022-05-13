using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ShoppingListAPI.Data;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ShoppingListAPI.Services.Product.Queries
{
    public class GetProductByIdQuery : IRequest<IResult>
    {
        public int Id { get; set; }
    }

    public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, IResult>
    {
        private readonly ApplicationDbContext _dbContext;

        private readonly ILogger<GetProductByIdQueryHandler> _logger;

        public GetProductByIdQueryHandler(ApplicationDbContext dbContext, ILogger<GetProductByIdQueryHandler> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<IResult> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var entity = await _dbContext.Products
                                             .ProjectToDTO()
                                             .SingleOrDefaultAsync(p => p.Id.Equals(request.Id), cancellationToken);

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
