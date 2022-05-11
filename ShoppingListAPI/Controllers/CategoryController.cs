using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShoppingListAPI.Data;
using ShoppingListAPI.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ShoppingListAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CategoryController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;

        public CategoryController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        [Route("")]
        public async Task<IActionResult> Get(CancellationToken cancellationToken)
        {
            return Ok(await _dbContext.Categories.ToListAsync(cancellationToken));
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> Get(int id, CancellationToken cancellationToken)
        {
            var entity = await _dbContext.Categories.SingleOrDefaultAsync(p => p.Id.Equals(id), cancellationToken);

            if (entity == null)
                return NotFound();

            return Ok(entity);
        }

        [HttpPost]
        [Route("")]
        public async Task<IActionResult> Create([FromBody] CategoryDTO category, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var entity = _dbContext.Categories.Add(category.AsCategory());
            try
            {
                await _dbContext.SaveChangesAsync(cancellationToken);

                return CreatedAtAction(nameof(Get), new { id = entity.Entity.Id }, entity.Entity);
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }

        [HttpPost]
        [Route("{id}")]
        public async Task<IActionResult> Edit([FromRoute]int id, [FromBody] CategoryDTO category, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var entity = await _dbContext.Categories.SingleOrDefaultAsync(p => p.Id.Equals(id), cancellationToken);

            if (entity == null)
                return NotFound();

            entity.Name = category.Name;

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

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id, CancellationToken cancellationToken)
        {
            var entity = await _dbContext.Categories.SingleOrDefaultAsync(p => p.Id.Equals(id), cancellationToken);

            if (entity == null)
                return NotFound();

            _dbContext.Categories.Remove(entity);

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
