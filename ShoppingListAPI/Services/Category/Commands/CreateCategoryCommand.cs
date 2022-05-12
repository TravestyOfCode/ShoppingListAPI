using MediatR;
using Microsoft.Extensions.Logging;
using ShoppingListAPI.Data;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;

namespace ShoppingListAPI.Services.Category.Commands
{
    public class CreateCategoryCommand : IRequest<IResult>
    {
        [Required]
        [MaxLength(32)]
        public string Name { get; set; }

        internal Data.Category AsCategory() => new() { Name = Name };
        
    }

    public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, IResult>
    {
        private readonly ApplicationDbContext _dbContext;

        private readonly ILogger<CreateCategoryCommandHandler> _logger;

        public CreateCategoryCommandHandler(ApplicationDbContext dbContext, ILogger<CreateCategoryCommandHandler> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<IResult> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var entity = _dbContext.Categories.Add(request.AsCategory());

                await _dbContext.SaveChangesAsync(cancellationToken);

                return Result.Ok(entity.Entity.AsCategoryDTO());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error handling request: {request}", request);
                return Result.ServerError();
            }
        }
    }
}
