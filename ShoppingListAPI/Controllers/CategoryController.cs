using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoppingListAPI.Services.Category;
using ShoppingListAPI.Services.Category.Commands;
using ShoppingListAPI.Services.Category.Queries;
using System.Threading;
using System.Threading.Tasks;
using static ShoppingListAPI.Models.Strings;

namespace ShoppingListAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CategoryController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CategoryController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [Route("")]
        public async Task<IActionResult> Get(CancellationToken cancellationToken)
        {
            GetAllCategoriesQuery request = new GetAllCategoriesQuery();

            var result = await _mediator.Send(request, cancellationToken);

            if (result.IsSuccess)
                return Ok(result.Value);

            return StatusCode(result.StatusCode);
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> Get([FromRoute]GetCategoryByIdQuery request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);

            if (result.IsSuccess)
                return Ok(result.Value);

            return StatusCode(result.StatusCode);
        }

        [HttpPost]
        [Route("")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> Create([FromBody] CreateCategoryCommand request, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var result = await _mediator.Send(request, cancellationToken);

            if (result.IsSuccess)
            {
                CategoryDTO value = (CategoryDTO)result.Value;
                return CreatedAtAction(nameof(Get), new { id = value.Id }, value);
            }

            return StatusCode(result.StatusCode);
        }

        [HttpPost]
        [Route("{id}")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> Edit([FromRoute]int id, [FromBody]EditCategoryCommand request, CancellationToken cancellationToken)
        {
            // Bind the Id as route values cannot bind into complex type
            request.Id = id;

            if (!ModelState.IsValid)
                return BadRequest();

            var result = await _mediator.Send(request, cancellationToken);

            if (result.IsSuccess)
                return Ok(result.Value);

            return StatusCode(result.StatusCode);

        }

        [HttpDelete]
        [Route("{id}")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> Delete([FromRoute] int id, DeleteCategoryCommand request, CancellationToken cancellationToken)
        {
            // Bind the Id as route values cannot bind into complex type
            request.Id = id;

            if (!ModelState.IsValid)
                return BadRequest();

            var result = await _mediator.Send(request, cancellationToken);

            if (result.IsSuccess)
                return Ok(result.Value);

            return StatusCode(result.StatusCode);
        }
    }

}
