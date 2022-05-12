using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ShoppingListAPI.Data;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;

namespace ShoppingListAPI.Services.Category.Commands
{
    public class EditCategoryCommand : IRequest<IResult>
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(32)]
        public string Name { get; set; }

        internal void MapTo(Data.Category category)
        {
            if (category == null)
                return;

            category.Name = Name;
        }
    }

    public class EditCategoryCommandHandler : IRequestHandler<EditCategoryCommand, IResult>
    {
        private readonly ApplicationDbContext _dbContext;

        private readonly ILogger<EditCategoryCommandHandler> _logger;

        public EditCategoryCommandHandler(ApplicationDbContext dbContext, ILogger<EditCategoryCommandHandler> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<IResult> Handle(EditCategoryCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var entity = await _dbContext.Categories.SingleOrDefaultAsync(p => p.Id.Equals(request.Id), cancellationToken);

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
