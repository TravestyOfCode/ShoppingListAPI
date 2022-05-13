using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ShoppingListAPI.Data;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ShoppingListAPI.Services.LineItem.Commands
{
    public class DeleteLineItemCommand : IRequest<IResult>
    {
        public int Id { get; set; }

        public int ShoppingListId { get; set; }

        public string UserId { get; set; }
    }

    public class DeleteLineItemCommandHandler : IRequestHandler<DeleteLineItemCommand, IResult>
    {
        private readonly ApplicationDbContext _dbContext;

        private readonly ILogger<DeleteLineItemCommandHandler> _logger;

        public DeleteLineItemCommandHandler(ApplicationDbContext dbContext, ILogger<DeleteLineItemCommandHandler> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<IResult> Handle(DeleteLineItemCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var entity = await _dbContext.LineItems.SingleOrDefaultAsync(p => p.Id.Equals(request.Id) &&
                                            p.ShoppingListId.Equals(request.ShoppingListId) &&
                                            p.ShoppingList.UserId.Equals(request.UserId), cancellationToken);

                if (entity == null)
                    return Result.NotFound();

                _dbContext.LineItems.Remove(entity);

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
