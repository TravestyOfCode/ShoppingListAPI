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
    public class CategoryController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;

        private readonly ILogger<CategoryController> _logger;

        public CategoryController(ApplicationDbContext dbContext, ILogger<CategoryController> logger)
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
                return Ok(await _dbContext.Categories.ToListAsync(cancellationToken));
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Unexpected error getting Categories.");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> Get(int id, CancellationToken cancellationToken)
        {
            try
            {
                var entity = await _dbContext.Categories.SingleOrDefaultAsync(p => p.Id.Equals(id), cancellationToken);

                if (entity == null)
                    return NotFound();

                return Ok(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error getting Categories for id: {id}", id);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost]
        [Route("")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> Create([FromBody] CategoryDTO category, CancellationToken cancellationToken)
        {
            try
            { 
            if (!ModelState.IsValid)
                return BadRequest();

            var entity = _dbContext.Categories.Add(category.AsCategory());

                await _dbContext.SaveChangesAsync(cancellationToken);

                return CreatedAtAction(nameof(Get), new { id = entity.Entity.Id }, entity.Entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error creating Category: {category}", category);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost]
        [Route("{id}")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> Edit([FromRoute]int id, [FromBody] CategoryDTO category, CancellationToken cancellationToken)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest();

                var entity = await _dbContext.Categories.SingleOrDefaultAsync(p => p.Id.Equals(id), cancellationToken);

                if (entity == null)
                    return NotFound();

                entity.Name = category.Name;

                await _dbContext.SaveChangesAsync(cancellationToken);

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error editing Category: {id}, {category}", id, category);
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
                var entity = await _dbContext.Categories.SingleOrDefaultAsync(p => p.Id.Equals(id), cancellationToken);

                if (entity == null)
                    return NotFound();

                _dbContext.Categories.Remove(entity);

                await _dbContext.SaveChangesAsync(cancellationToken);

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error deleting Category for id: {id}", id);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
    
}
