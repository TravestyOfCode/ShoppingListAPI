using MediatR;
using Microsoft.Extensions.Logging;
using ShoppingListAPI.Data;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;

namespace ShoppingListAPI.Services.ShoppingList.Commands
{
    public class CreateShoppingListCommand : IRequest<IResult>
    {
        public string UserId { get; set; }

        [MaxLength(32)]
        public string Title { get; set; }

        public DateTime? TripDate { get; set; }

        public bool IsCompleted { get; set; }

        //public List<LineItem> LineItems { get; set; }

        internal Data.ShoppingList AsShoppingList()
        {
            return new()
            {
                UserId = UserId,
                Title = Title,
                TripDate = TripDate,
                IsCompleted = IsCompleted,
                //LineItems = LineItems.AsLineItems()
            };
        }
    }

    public class CreateShoppingListCommandHandler : IRequestHandler<CreateShoppingListCommand, IResult>
    {
        private readonly ApplicationDbContext _dbContext;

        private readonly ILogger<CreateShoppingListCommandHandler> _logger;

        public CreateShoppingListCommandHandler(ApplicationDbContext dbContext, ILogger<CreateShoppingListCommandHandler> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<IResult> Handle(CreateShoppingListCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var entity = _dbContext.ShoppingLists.Add(request.AsShoppingList());

                await _dbContext.SaveChangesAsync(cancellationToken);

                return Result.Created(entity.Entity.AsShoppingListDTO());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error handling request: {request}", request);
                return Result.ServerError();
            }
        }
    }
}
