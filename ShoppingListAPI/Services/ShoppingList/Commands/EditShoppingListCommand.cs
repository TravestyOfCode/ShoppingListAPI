using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ShoppingListAPI.Data;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;

namespace ShoppingListAPI.Services.ShoppingList.Commands
{
    public class EditShoppingListCommand : IRequest<IResult>
    {
        public int Id { get; set; }

        public string UserId { get; set; }

        [MaxLength(32)]
        public string Title { get; set; }

        public DateTime? TripDate { get; set; }

        public bool IsCompleted { get; set; }

        //public List<LineItem> LineItems { get; set; }

        internal void MapTo(Data.ShoppingList shoppingList)
        {
            if (shoppingList == null)
                return;

            shoppingList.Title = Title;
            shoppingList.TripDate = TripDate;
            shoppingList.IsCompleted = IsCompleted;
            //LineItems.MapTo(shoppingList.LineItems);
        }
    }

    public class EditShoppingListCommandHandler : IRequestHandler<EditShoppingListCommand, IResult>
    {
        private readonly ApplicationDbContext _dbContext;

        private readonly ILogger<EditShoppingListCommandHandler> _logger;

        public EditShoppingListCommandHandler(ApplicationDbContext dbContext, ILogger<EditShoppingListCommandHandler> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<IResult> Handle(EditShoppingListCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var entity = await _dbContext.ShoppingLists.SingleOrDefaultAsync(p => p.Id.Equals(request.Id) && p.UserId.Equals(request.UserId), cancellationToken);

                if (entity == null)
                    return Result.NotFound();

                request.MapTo(entity);

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
