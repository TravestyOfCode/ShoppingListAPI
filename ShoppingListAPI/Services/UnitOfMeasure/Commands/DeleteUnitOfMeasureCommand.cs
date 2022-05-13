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

namespace ShoppingListAPI.Services.UnitOfMeasure.Commands
{
    public class DeleteUnitOfMeasureCommand : IRequest<IResult>
    {
        public int Id { get; set; }
    }

    public class DeleteUnitOfMeasureCommandHandler : IRequestHandler<DeleteUnitOfMeasureCommand, IResult>
    {
        private readonly ApplicationDbContext _dbContext;

        private readonly ILogger<DeleteUnitOfMeasureCommandHandler> _logger;

        public DeleteUnitOfMeasureCommandHandler(ApplicationDbContext dbContext, ILogger<DeleteUnitOfMeasureCommandHandler> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<IResult> Handle(DeleteUnitOfMeasureCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var entity = await _dbContext.UnitOfMeasures.SingleOrDefaultAsync(p => p.Id.Equals(request.Id), cancellationToken);

                if (entity == null)
                    return Result.NotFound();

                _dbContext.UnitOfMeasures.Remove(entity);

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
