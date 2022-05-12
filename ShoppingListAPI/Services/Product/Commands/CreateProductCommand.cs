using MediatR;
using Microsoft.Extensions.Logging;
using ShoppingListAPI.Data;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;

namespace ShoppingListAPI.Services.Product.Commands
{
    public class CreateProductCommand : IRequest<IResult>
    {
        public int CategoryId { get; set; }

        [Required]
        [MaxLength(64)]
        public string Name { get; set; }

        internal Data.Product AsProduct() => new() { CategoryId = CategoryId, Name = Name };
    }

    public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, IResult>
    {
        private readonly ApplicationDbContext _dbContext;

        private readonly ILogger<CreateProductCommandHandler> _logger;

        public CreateProductCommandHandler(ApplicationDbContext dbContext, ILogger<CreateProductCommandHandler> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<IResult> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var entity = _dbContext.Products.Add(request.AsProduct());

                await _dbContext.SaveChangesAsync(cancellationToken);
                                
                _dbContext.Entry(entity.Entity).Reference(p => p.Category).Load();

                return Result.Created(entity.Entity.AsProductDTO());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error handling request: {request}", request);
                return Result.ServerError();
            }
        }
    }
}
