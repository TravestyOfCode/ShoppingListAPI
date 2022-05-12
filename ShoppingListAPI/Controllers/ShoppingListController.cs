using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShoppingListAPI.Data;
using ShoppingListAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ShoppingListAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class ShoppingListController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;

        //private readonly UserManager<IdentityUser> _userManager;

        public ShoppingListController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        [Route("")]
        public async Task<IActionResult> Get(CancellationToken cancellationToken)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var shoppingLists = await _dbContext.ShoppingLists
                    .Include(p => p.LineItems)
                    .Where(p => p.UserId.Equals(userId))
                    .ToListAsync(cancellationToken);

            return Ok(shoppingLists);
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> Get([FromRoute] int id, CancellationToken cancellationToken)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var entity = await _dbContext.ShoppingLists
                .Include(p => p.LineItems)
                .SingleOrDefaultAsync(p => p.Id.Equals(id) && p.UserId.Equals(userId), cancellationToken);

            if (entity == null)
                return NotFound();

            return Ok(entity);
        }

        [HttpPost]
        [Route("")]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] ShoppingListDTO shoppingList, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var entity = _dbContext.ShoppingLists.Add(shoppingList.AsShoppingList(userId));

            await _dbContext.SaveChangesAsync(cancellationToken);

            // Load the navigation properties
            _dbContext.Entry(entity.Entity).Collection(p => p.LineItems).Load();

            foreach (var lineItem in entity.Entity.LineItems)
            {
                _dbContext.Entry(lineItem).Reference(p => p.Product).Load();
                _dbContext.Entry(lineItem.Product).Reference(p => p.Category).Load();
                _dbContext.Entry(lineItem).Reference(p => p.UnitOfMeasure).Load();
            }

            return CreatedAtAction(nameof(Get), new { id = entity.Entity.Id }, entity.Entity);
        }

        [HttpPost]
        [Route("{id}")]
        [Authorize]
        public async Task<IActionResult> Edit([FromRoute] int id, [FromBody] ShoppingListDTO shoppingList, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var entity = await _dbContext.ShoppingLists.SingleOrDefaultAsync(p => p.Id.Equals(id) && p.UserId.Equals(userId), cancellationToken);

            if (entity == null)
                return NotFound();

            shoppingList.MapTo(entity);

            await _dbContext.SaveChangesAsync(cancellationToken);

            return Ok();
        }

        [HttpDelete]
        [Route("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete([FromRoute] int id, CancellationToken cancellationToken)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var entity = await _dbContext.ShoppingLists.SingleOrDefaultAsync(p => p.Id.Equals(id) && p.UserId.Equals(userId), cancellationToken);

            if (entity == null)
                return NotFound();

            _dbContext.ShoppingLists.Remove(entity);

            await _dbContext.SaveChangesAsync(cancellationToken);

            return Ok();
        }
        
    }
}
