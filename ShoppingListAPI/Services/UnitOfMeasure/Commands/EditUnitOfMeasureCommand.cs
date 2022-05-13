using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ShoppingListAPI.Data;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;

namespace ShoppingListAPI.Services.UnitOfMeasure.Commands
{
    public class EditUnitOfMeasureCommand : IRequest<IResult>
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(6)]
        public string Name { get; set; }

        internal void MapTo(Data.UnitOfMeasure unitOfMeasure)
        {
            if (unitOfMeasure == null)
                return;

            unitOfMeasure.Name = Name;
        }
    }

    public class EditUnitOfMeasureCommandHandler : IRequestHandler<EditUnitOfMeasureCommand, IResult>
    {
        private readonly ApplicationDbContext _dbContext;

        private readonly ILogger<EditUnitOfMeasureCommandHandler> _logger;

        public EditUnitOfMeasureCommandHandler(ApplicationDbContext dbContext, ILogger<EditUnitOfMeasureCommandHandler> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<IResult> Handle(EditUnitOfMeasureCommand request, CancellationToken cancellationToken)
        {

            try
            {
                var entity = await _dbContext.UnitOfMeasures.SingleOrDefaultAsync(p => p.Id.Equals(request.Id), cancellationToken);

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
