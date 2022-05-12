using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShoppingListAPI.Data;
using ShoppingListAPI.Data.Authentication;
using ShoppingListAPI.Models;
using System.Threading;
using System.Threading.Tasks;

namespace ShoppingListAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UnitOfMeasureController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;

        public UnitOfMeasureController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        [Route("")]
        public async Task<IActionResult> Get(CancellationToken cancellationToken)
        {
            return Ok(await _dbContext.UnitOfMeasures.ToListAsync(cancellationToken));
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> Get([FromRoute]int id, CancellationToken cancellationToken)
        {
            var entity = await _dbContext.UnitOfMeasures.SingleOrDefaultAsync(p => p.Id.Equals(id), cancellationToken);

            if (entity == null)
                return NotFound();

            return Ok(entity);
        }

        [HttpPost]
        [Route("")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> Create([FromBody]UnitOfMeasureDTO uom, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var entity = _dbContext.UnitOfMeasures.Add(uom.AsUnitOfMeasure());

            await _dbContext.SaveChangesAsync(cancellationToken);

            return CreatedAtAction(nameof(Get), new { id = entity.Entity.Id }, entity.Entity);
        }

        [HttpPost]
        [Route("{id}")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> Edit([FromRoute] int id, [FromBody] UnitOfMeasureDTO uom, CancellationToken cancellationToken)
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

        [HttpDelete]
        [Route("{id}")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> Delete([FromRoute] int id, CancellationToken cancellationToken)
        {
            var entity = await _dbContext.UnitOfMeasures.SingleOrDefaultAsync(p => p.Id.Equals(id), cancellationToken);

            if (entity == null)
                return NotFound();

            _dbContext.UnitOfMeasures.Remove(entity);

            await _dbContext.SaveChangesAsync(cancellationToken);

            return Ok();
        }
    }
}
