using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
    public class ProductController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;

        public ProductController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        [Route("")]
        public async Task<IActionResult> Get(CancellationToken cancellationToken)
        {
            return Ok(await _dbContext.Products.Include(p => p.Category).ToListAsync(cancellationToken));
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> Get(int id, CancellationToken cancellationToken)
        {
            var entity = await _dbContext.Products.Include(p => p.Category).SingleOrDefaultAsync(p => p.Id.Equals(id), cancellationToken);

            if (entity == null)
                return NotFound();

            return Ok(entity);
        }

        [HttpPost]
        [Route("")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> Create([FromBody] ProductDTO product, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var entity = _dbContext.Products.Add(product.AsProduct());
            try
            {
                await _dbContext.SaveChangesAsync(cancellationToken);

                // Load the navigation property
                _dbContext.Entry(entity.Entity).Reference(p => p.Category).Load();

                return CreatedAtAction(nameof(Get), new { id = entity.Entity.Id }, entity.Entity);
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }

        [HttpPost]
        [Route("{id}")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> Edit([FromRoute] int id, [FromBody] ProductDTO product, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var entity = await _dbContext.Products.Include(p => p.Category).SingleOrDefaultAsync(p => p.Id.Equals(id), cancellationToken);

            if (entity == null)
                return NotFound();

            entity.Name = product.Name;

            try
            {
                await _dbContext.SaveChangesAsync(cancellationToken);

                return Ok(entity);
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }

        [HttpDelete]
        [Route("{id}")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> Delete([FromRoute] int id, CancellationToken cancellationToken)
        {
            var entity = await _dbContext.Products.SingleOrDefaultAsync(p => p.Id.Equals(id), cancellationToken);

            if (entity == null)
                return NotFound();

            _dbContext.Products.Remove(entity);

            try
            {
                await _dbContext.SaveChangesAsync(cancellationToken);

                return Ok();
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }
    }
}
