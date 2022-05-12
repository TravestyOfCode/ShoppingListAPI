using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ShoppingListAPI.Data;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;

namespace ShoppingListAPI.Services.Product.Commands
{
    public class EditProductCommand : IRequest<IResult>
    {
        public int Id { get; set; }

        public int CategoryId { get; set; }

        [Required]
        [MaxLength(64)]
        public string Name { get; set; }

        internal void MapTo(Data.Product product)
        {
            if (product == null)
                return;

            product.CategoryId = CategoryId;
            product.Name = Name;
        }
    }

    public class EditProductCommandHandler : IRequestHandler<EditProductCommand, IResult>
    {
        private readonly ApplicationDbContext _dbContext;

        private readonly ILogger<EditProductCommandHandler> _logger;

        public EditProductCommandHandler(ApplicationDbContext dbContext, ILogger<EditProductCommandHandler> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<IResult> Handle(EditProductCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var entity = await _dbContext.Products.SingleOrDefaultAsync(p => p.Id.Equals(request.Id), cancellationToken);

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
