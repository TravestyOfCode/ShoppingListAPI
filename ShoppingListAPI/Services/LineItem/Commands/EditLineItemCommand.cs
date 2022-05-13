using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ShoppingListAPI.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ShoppingListAPI.Services.LineItem.Commands
{
    public class EditLineItemCommand : IRequest<IResult>
    {
        public int Id { get; set; }

        public string UserId { get; set; }

        public int ShoppingListId { get; set; }

        public bool IsCompleted { get; set; }

        public int ProductId { get; set; }

        public decimal Quantity { get; set; }

        public int UnitOfMeasureId { get; set; }

        internal void MapTo(Data.LineItem lineItem)
        {
            if (lineItem == null)
                return;

            lineItem.IsCompleted = IsCompleted;
            lineItem.ProductId = ProductId;
            lineItem.Quantity = Quantity;
            lineItem.UnitOfMeasureId = UnitOfMeasureId;
        }
    }

    public class EditLineItemCommandHandler : IRequestHandler<EditLineItemCommand, IResult>
    {
        private readonly ApplicationDbContext _dbContext;

        private readonly ILogger<EditLineItemCommandHandler> _logger;

        public EditLineItemCommandHandler(ApplicationDbContext dbContext, ILogger<EditLineItemCommandHandler> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<IResult> Handle(EditLineItemCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var entity = await _dbContext.LineItems.SingleOrDefaultAsync(p => p.Id.Equals(request.Id) &&
                                             p.ShoppingListId.Equals(request.ShoppingListId) &&
                                             p.ShoppingList.UserId.Equals(request.UserId), cancellationToken);
                if (entity == null)
                    return Result.NotFound();

                request.MapTo(entity);

                await _dbContext.SaveChangesAsync(cancellationToken);

                _dbContext.Entry(entity).Reference(p => p.Product).Load();
                _dbContext.Entry(entity).Reference(p => p.UnitOfMeasure).Load();
                _dbContext.Entry(entity.Product).Reference(p => p.Category).Load();

                return Result.Ok(entity.AsLineItemDTO());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error handling request: {request}", request);
                return Result.ServerError();
            }
        }
    }
}
