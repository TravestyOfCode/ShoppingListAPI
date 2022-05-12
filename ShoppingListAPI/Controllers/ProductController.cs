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
    public class ProductController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;

        private readonly ILogger<ProductController> _logger;

        public ProductController(ApplicationDbContext dbContext, ILogger<ProductController> logger)
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
                return Ok(await _dbContext.Products.Include(p => p.Category).ToListAsync(cancellationToken));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error getting Products.");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> Get(int id, CancellationToken cancellationToken)
        {
            try
            {
                var entity = await _dbContext.Products.Include(p => p.Category).SingleOrDefaultAsync(p => p.Id.Equals(id), cancellationToken);

                if (entity == null)
                    return NotFound();

                return Ok(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error getting Product for id: {id}", id);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost]
        [Route("")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> Create([FromBody] ProductDTO product, CancellationToken cancellationToken)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest();

                var entity = _dbContext.Products.Add(product.AsProduct());

                await _dbContext.SaveChangesAsync(cancellationToken);

                // Load the navigation property
                _dbContext.Entry(entity.Entity).Reference(p => p.Category).Load();

                return CreatedAtAction(nameof(Get), new { id = entity.Entity.Id }, entity.Entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error creating Product: {product}", product);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost]
        [Route("{id}")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> Edit([FromRoute] int id, [FromBody] ProductDTO product, CancellationToken cancellationToken)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest();

                var entity = await _dbContext.Products.Include(p => p.Category).SingleOrDefaultAsync(p => p.Id.Equals(id), cancellationToken);

                if (entity == null)
                    return NotFound();

                entity.Name = product.Name;

                await _dbContext.SaveChangesAsync(cancellationToken);

                return Ok(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error editing Product: {id}, {product}", id, product);
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
                var entity = await _dbContext.Products.SingleOrDefaultAsync(p => p.Id.Equals(id), cancellationToken);

                if (entity == null)
                    return NotFound();

                _dbContext.Products.Remove(entity);

                await _dbContext.SaveChangesAsync(cancellationToken);

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error deleting Product for id: {id}", id);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
