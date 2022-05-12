using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ShoppingListAPI.Data;
using ShoppingListAPI.Data.Authentication;
using ShoppingListAPI.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ShoppingListAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UnitOfMeasureController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;

        private readonly ILogger<UnitOfMeasureController> _logger;

        public UnitOfMeasureController(ApplicationDbContext dbContext, ILogger<UnitOfMeasureController> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        [HttpGet]
        [Route("")]
        public async Task<IActionResult> Get(CancellationToken cancellationToken)
        {
            try
            {
                return Ok(await _dbContext.UnitOfMeasures.ToListAsync(cancellationToken));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error getting UnitOfMeasures.");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> Get([FromRoute]int id, CancellationToken cancellationToken)
        {
            try
            {
                var entity = await _dbContext.UnitOfMeasures.SingleOrDefaultAsync(p => p.Id.Equals(id), cancellationToken);

                if (entity == null)
                    return NotFound();

                return Ok(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error getting UnitOfMeasure for id: {id}.", id);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost]
        [Route("")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> Create([FromBody]UnitOfMeasureDTO uom, CancellationToken cancellationToken)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest();

                var entity = _dbContext.UnitOfMeasures.Add(uom.AsUnitOfMeasure());

                await _dbContext.SaveChangesAsync(cancellationToken);

                return CreatedAtAction(nameof(Get), new { id = entity.Entity.Id }, entity.Entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error creating UnitOfMeasure: {uom}.", uom);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost]
        [Route("{id}")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> Edit([FromRoute] int id, [FromBody] UnitOfMeasureDTO uom, CancellationToken cancellationToken)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest();

                var entity = await _dbContext.UnitOfMeasures.SingleOrDefaultAsync(p => p.Id.Equals(id), cancellationToken);

                if (entity == null)
                    return NotFound();

                uom.MapTo(entity);

                await _dbContext.SaveChangesAsync(cancellationToken);

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error editing UnitOfMeasure: {id}, {uom}.", id, uom);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpDelete]
        [Route("{id}")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> Delete([FromRoute] int id, CancellationToken cancellationToken)
        {
            try
            {
                var entity = await _dbContext.UnitOfMeasures.SingleOrDefaultAsync(p => p.Id.Equals(id), cancellationToken);

                if (entity == null)
                    return NotFound();

                _dbContext.UnitOfMeasures.Remove(entity);

                await _dbContext.SaveChangesAsync(cancellationToken);

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error deleting UnitOfMeasure for id: {id}.", id);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
