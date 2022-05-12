using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ShoppingListAPI.Data;
using ShoppingListAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace ShoppingListAPI.Controllers
{
    [ApiController]
    public class LineItemController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;

        private readonly ILogger<LineItemController> _logger;

        public LineItemController(ApplicationDbContext dbContext, ILogger<LineItemController> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        [HttpGet]
        [Route("shoppinglist/{id}/lineitem")]
        [Authorize]
        public async Task<IActionResult> Get([FromRoute] int id, CancellationToken cancellationToken)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                // Get the shopping list to ensure this user created it
                var entities = await _dbContext.ShoppingLists
                    .Where(p => p.Id.Equals(id) && p.UserId.Equals(userId))
                    .Include(p => p.LineItems)
                    .Select(p => p.LineItems)
                    .SingleOrDefaultAsync(cancellationToken);

                if (entities == null)
                    return NotFound();

                return Ok(entities);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error getting LineItems for id: {id}", id);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet]
        [Route("shoppinglist/{id}/lineitem/{lineId}")]
        [Authorize]
        public async Task<IActionResult> Get([FromRoute] int id, [FromRoute] int lineId, CancellationToken cancellationToken)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                // Get the shopping list to ensure this user created it
                var entities = await _dbContext.ShoppingLists
                    .Where(p => p.Id.Equals(id) && p.UserId.Equals(userId))
                    .Include(p => p.LineItems)
                    .Select(p => p.LineItems.SingleOrDefault(p => p.Id.Equals(lineId)))
                    .SingleOrDefaultAsync(cancellationToken);

                if (entities == null)
                    return NotFound();

                return Ok(entities);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error getting LineItem for id: {id}", id);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost]
        [Route("shoppinglist/{id}/lineitem")]
        [Authorize]
        public async Task<IActionResult> Create([FromRoute] int id, LineItemDTO line, CancellationToken cancellationToken)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest();

                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                line.ShoppingListId = id;
                var entity = _dbContext.LineItems.Add(line.AsLineItem());

                await _dbContext.SaveChangesAsync(cancellationToken);

                return CreatedAtAction(nameof(Get), new { id = entity.Entity.Id }, entity.Entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error creating LineItem: {id}, {line}", id, line);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost]
        [Route("shoppinglist/{id}/lineitem/{lineId}")]
        [Authorize]
        public async Task<IActionResult> Edit([FromRoute] int id, [FromRoute] int lineId, LineItemDTO line, CancellationToken cancellationToken)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest();

                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                // Get via ShoppingList to verify the user can modify the line
                var entity = await _dbContext.ShoppingLists
                    .Include(p => p.LineItems)
                    .Where(p => p.Id.Equals(id) && p.UserId.Equals(userId))
                    .Select(p => p.LineItems.SingleOrDefault(p => p.Id.Equals(lineId)))
                    .SingleOrDefaultAsync(cancellationToken);

                if (entity == null)
                    return NotFound();

                line.MapTo(entity);

                await _dbContext.SaveChangesAsync(cancellationToken);

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error editing LineItem: {id}, {line}", id, line);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpDelete]
        [Route("shoppinglist/{id}/lineitem/{lineId}")]
        [Authorize]
        public async Task<IActionResult> Delete([FromRoute] int id, [FromRoute]int lineId, CancellationToken cancellationToken)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                var entity = await _dbContext.ShoppingLists
                    .Include(p => p.LineItems)
                    .Where(p => p.Id.Equals(id) && p.UserId.Equals(userId))
                    .Select(p => p.LineItems.SingleOrDefault(p => p.Id.Equals(lineId)))
                    .SingleOrDefaultAsync(cancellationToken);

                if (entity == null)
                    return NotFound();

                _dbContext.LineItems.Remove(entity);

                await _dbContext.SaveChangesAsync(cancellationToken);

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error deleting LineItem for id: {id}", id);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
