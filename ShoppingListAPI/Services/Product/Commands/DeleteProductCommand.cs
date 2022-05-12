using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ShoppingListAPI.Data;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ShoppingListAPI.Services.Product.Commands
{
    public class DeleteProductCommand : IRequest<IResult>
    {
        public int Id { get; set; }
    }

    public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand, IResult>
    {
        private readonly ApplicationDbContext _dbContext;

        private readonly ILogger<DeleteProductCommandHandler> _logger;

        public DeleteProductCommandHandler(ApplicationDbContext dbContext, ILogger<DeleteProductCommandHandler> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<IResult> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var entity = await _dbContext.Products.SingleOrDefaultAsync(p => p.Id.Equals(request.Id), cancellationToken);

                if (entity == null)
                    return Result.NotFound();

                _dbContext.Products.Remove(entity);

                await _dbContext.SaveChangesAsync(cancellationToken);

                return Result.Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error handling request: {request}", request);
                return Result.ServerError();
            }
        }
    }
}
