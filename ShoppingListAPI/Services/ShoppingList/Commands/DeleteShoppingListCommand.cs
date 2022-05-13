using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ShoppingListAPI.Data;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ShoppingListAPI.Services.ShoppingList.Commands
{
    public class DeleteShoppingListCommand : IRequest<IResult>
    {
        public int Id { get; set; }

        public string UserId { get; set; }
    }

    public class DeleteShoppingListCommandHandler : IRequestHandler<DeleteShoppingListCommand, IResult>
    {
        private readonly ApplicationDbContext _dbContext;

        private readonly ILogger<DeleteShoppingListCommandHandler> _logger;

        public DeleteShoppingListCommandHandler(ApplicationDbContext dbContext, ILogger<DeleteShoppingListCommandHandler> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<IResult> Handle(DeleteShoppingListCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var entity = await _dbContext.ShoppingLists.SingleOrDefaultAsync(p => p.Id.Equals(request.Id) && p.UserId.Equals(request.UserId), cancellationToken);

                if (entity == null)
                    return Result.NotFound();

                _dbContext.ShoppingLists.Remove(entity);

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
