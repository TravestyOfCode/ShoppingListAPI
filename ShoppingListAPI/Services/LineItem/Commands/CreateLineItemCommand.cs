using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ShoppingListAPI.Data;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ShoppingListAPI.Services.LineItem.Commands
{
    public class CreateLineItemCommand : IRequest<IResult>
    {
        public string UserId { get; set; }

        public int ShoppingListId { get; set; }

        public bool IsCompleted { get; set; }

        public int ProductId { get; set; }

        public decimal Quantity { get; set; }

        public int UnitOfMeasureId { get; set; }

        internal Data.LineItem AsLineItem()
        {
            return new()
            {
                ShoppingListId = ShoppingListId,
                IsCompleted = IsCompleted,
                ProductId = ProductId,
                Quantity = Quantity,
                UnitOfMeasureId = UnitOfMeasureId
            };
        }
    }

    public class CreateLineItemCommandHandler : IRequestHandler<CreateLineItemCommand, IResult>
    {
        private readonly ApplicationDbContext _dbContext;

        private readonly ILogger<CreateLineItemCommandHandler> _logger;

        public CreateLineItemCommandHandler(ApplicationDbContext dbContext, ILogger<CreateLineItemCommandHandler> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<IResult> Handle(CreateLineItemCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Ensure the user is able to modify this shoppinglist
                var shoppinglist = await _dbContext.ShoppingLists
                                .SingleOrDefaultAsync(p => p.Id.Equals(request.ShoppingListId) && p.UserId.Equals(request.UserId), cancellationToken);

                if (shoppinglist == null)
                    return Result.NotFound();

                var entity = _dbContext.LineItems.Add(request.AsLineItem());

                await _dbContext.SaveChangesAsync(cancellationToken);

                _dbContext.Entry(entity.Entity).Reference(p => p.Product).Load();
                _dbContext.Entry(entity.Entity).Reference(p => p.UnitOfMeasure).Load();
                _dbContext.Entry(entity.Entity.Product).Reference(p => p.Category).Load();

                return Result.Created(entity.Entity.AsLineItemDTO());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error handling request: {request}", request);
                return Result.ServerError();
            }
        }
    }
}
