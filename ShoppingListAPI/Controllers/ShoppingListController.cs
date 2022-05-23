using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoppingListAPI.Services.ShoppingList;
using ShoppingListAPI.Services.ShoppingList.Commands;
using ShoppingListAPI.Services.ShoppingList.Queries;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace ShoppingListAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class ShoppingListController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ShoppingListController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [Route("")]
        public async Task<IActionResult> Get( CancellationToken cancellationToken)
        {
            GetAllShoppingListsByUserQuery request = new GetAllShoppingListsByUserQuery()
            {
                UserId = User.FindFirstValue(ClaimTypes.NameIdentifier)
            };

            var result = await _mediator.Send(request, cancellationToken);

            if (result.IsSuccess)
                return Ok(result.Value);

            return StatusCode(result.StatusCode);
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> Get([FromRoute] int id, CancellationToken cancellationToken)
        {
            GetShoppingListByUserAndIdQuery request = new GetShoppingListByUserAndIdQuery()
            {
                Id = id,
                UserId = User.FindFirstValue(ClaimTypes.NameIdentifier)
            };

            var result = await _mediator.Send(request, cancellationToken);

            if (result.IsSuccess)
                return Ok(result.Value);

            return StatusCode(result.StatusCode);
        }

        [HttpPost]
        [Route("")]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] CreateShoppingListCommand request, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            request.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = await _mediator.Send(request, cancellationToken);

            if(result.IsSuccess)
            {
                var shoppingList = (ShoppingListDTO)result.Value;
                return CreatedAtAction(nameof(Get), new { shoppingList.Id }, shoppingList);
            }

            return StatusCode(result.StatusCode);
        }

        [HttpPost]
        [Route("{id}")]
        [Authorize]
        public async Task<IActionResult> Edit([FromRoute] int id, [FromBody]EditShoppingListCommand request, CancellationToken cancellationToken)
        {
            // Map the Id to the request as route values can not be mapped to complex types.
            request.Id = id;
            request.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = await _mediator.Send(request, cancellationToken);

            if (result.IsSuccess)
                return Ok(result.Value);

            return StatusCode(result.StatusCode);
        }

        [HttpDelete]
        [Route("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete([FromRoute] int id, CancellationToken cancellationToken)
        {
            DeleteShoppingListCommand request = new DeleteShoppingListCommand()
            {

                Id = id,
                UserId = User.FindFirstValue(ClaimTypes.NameIdentifier)
            };

            var result = await _mediator.Send(request, cancellationToken);

            if (result.IsSuccess)
                return Ok(result.Value);

            return StatusCode(result.StatusCode);
        }

    }
}
