using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

        public LineItemController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        [Route("shoppinglist/{id}/lineitem")]
        [Authorize]
        public async Task<IActionResult> Get([FromRoute] int id, CancellationToken cancellationToken)
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

        [HttpGet]
        [Route("shoppinglist/{id}/lineitem/{lineId}")]
        [Authorize]
        public async Task<IActionResult> Get([FromRoute] int id, [FromRoute] int lineId, CancellationToken cancellationToken)
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

        [HttpPost]
        [Route("shoppinglist/{id}/lineitem")]
        [Authorize]
        public async Task<IActionResult> Create([FromRoute] int id, LineItemDTO line, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            line.ShoppingListId = id;
            var entity = _dbContext.LineItems.Add(line.AsLineItem());

            await _dbContext.SaveChangesAsync(cancellationToken);

            return CreatedAtAction(nameof(Get), new { id = entity.Entity.Id }, entity.Entity);
        }

        [HttpPost]
        [Route("shoppinglist/{id}/lineitem/{lineId}")]
        [Authorize]
        public async Task<IActionResult> Edit([FromRoute] int id, [FromRoute] int lineId, LineItemDTO line, CancellationToken cancellationToken)
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

        [HttpDelete]
        [Route("shoppinglist/{id}/lineitem/{lineId}")]
        [Authorize]
        public async Task<IActionResult> Delete([FromRoute] int id, [FromRoute]int lineId, CancellationToken cancellationToken)
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
    }
}
