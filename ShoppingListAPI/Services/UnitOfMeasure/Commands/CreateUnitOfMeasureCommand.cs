using MediatR;
using Microsoft.Extensions.Logging;
using ShoppingListAPI.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ShoppingListAPI.Services.UnitOfMeasure.Commands
{
    public class CreateUnitOfMeasureCommand : IRequest<IResult>
    {
        [Required]
        [MaxLength(6)]
        public string Name { get; set; }

        internal Data.UnitOfMeasure AsUnitOfMeasure() => new() { Name = Name };
    }

    public class CreateUnitOfMeasureCommandHandler : IRequestHandler<CreateUnitOfMeasureCommand, IResult>
    {
        private readonly ApplicationDbContext _dbContext;

        private readonly ILogger<CreateUnitOfMeasureCommandHandler> _logger;

        public CreateUnitOfMeasureCommandHandler(ApplicationDbContext dbContext, ILogger<CreateUnitOfMeasureCommandHandler> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<IResult> Handle(CreateUnitOfMeasureCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var entity = _dbContext.UnitOfMeasures.Add(request.AsUnitOfMeasure());

                await _dbContext.SaveChangesAsync(cancellationToken);

                return Result.Created(entity.Entity.AsUnitOfMeasureDTO());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error handling request: {request}", request);
                return Result.ServerError();
            }
        }
    }
}
