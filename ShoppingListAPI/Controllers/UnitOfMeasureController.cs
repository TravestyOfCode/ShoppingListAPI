using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoppingListAPI.Data.Authentication;
using ShoppingListAPI.Services.UnitOfMeasure;
using ShoppingListAPI.Services.UnitOfMeasure.Commands;
using ShoppingListAPI.Services.UnitOfMeasure.Queries;
using System.Threading;
using System.Threading.Tasks;

namespace ShoppingListAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UnitOfMeasureController : ControllerBase
    {
        public readonly IMediator _mediator;

        public UnitOfMeasureController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [Route("")]
        public async Task<IActionResult> Get(GetAllUnitOfMeasuresQuery request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);

            if (result.IsSuccess)
                return Ok(result.Value);

            return StatusCode(result.StatusCode);
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> Get([FromRoute]int id, GetUnitOfMeasureByIdQuery request, CancellationToken cancellationToken)
        {
            // Map the Id to the request as route values can not be mapped to complex types.
            request.Id = id;

            var result = await _mediator.Send(request, cancellationToken);

            if (result.IsSuccess)
                return Ok(result.Value);

            return StatusCode(result.StatusCode);
        }

        [HttpPost]
        [Route("")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> Create([FromBody]CreateUnitOfMeasureCommand request, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var result = await _mediator.Send(request, cancellationToken);

            if (result.IsSuccess)
            {
                UnitOfMeasureDTO value = (UnitOfMeasureDTO)result.Value;
                return CreatedAtAction(nameof(Get), new { id = value.Id }, value);
            }

            return StatusCode(result.StatusCode);
        }

        [HttpPost]
        [Route("{id}")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> Edit([FromRoute] int id, [FromBody] EditUnitOfMeasureCommand request, CancellationToken cancellationToken)
        {
            // Map the Id to the request as route values can not be mapped to complex types.
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
        public async Task<IActionResult> Delete([FromRoute] int id, DeleteUnitOfMeasureCommand request, CancellationToken cancellationToken)
        {
            // Map the Id to the request as route values can not be mapped to complex types.
            request.Id = id;

            var result = await _mediator.Send(request, cancellationToken);

            if (result.IsSuccess)
                return Ok(result.Value);

            return StatusCode(result.StatusCode);
        }
    }
}
