﻿using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoppingListAPI.Services.LineItem;
using ShoppingListAPI.Services.LineItem.Commands;
using ShoppingListAPI.Services.LineItem.Queries;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace ShoppingListAPI.Controllers
{
    [ApiController]
    public class LineItemController : ControllerBase
    {
        private readonly IMediator _mediator;

        public LineItemController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [Route("shoppinglist/{id}/lineitem")]
        [Authorize]
        public async Task<IActionResult> Get([FromRoute] int id, GetAllLineItemsByShoppingListIdAndUserIdQuery request,  CancellationToken cancellationToken)
        {
            // Map the Id to the request as route values can not be mapped to complex types.
            request.Id = id;
            request.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = await _mediator.Send(request, cancellationToken);

            if (result.IsSuccess)
                return Ok(result.Value);

            return StatusCode(result.StatusCode);
        }

        [HttpGet]
        [Route("shoppinglist/{id}/lineitem/{lineId}")]
        [Authorize]
        public async Task<IActionResult> Get([FromRoute] int id, [FromRoute] int lineId, GetLineItemByIdAndShoppingListIdQuery request, CancellationToken cancellationToken)
        {
            // Map the Id to the request as route values can not be mapped to complex types.
            request.Id = lineId;
            request.ShoppingListId = id;
            request.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = await _mediator.Send(request, cancellationToken);

            if (result.IsSuccess)
                return Ok(result.Value);

            return StatusCode(result.StatusCode);
        }

        [HttpPost]
        [Route("shoppinglist/{id}/lineitem")]
        [Authorize]
        public async Task<IActionResult> Create([FromRoute] int id, [FromBody]CreateLineItemCommand request, CancellationToken cancellationToken)
        {
            // Map the Id to the request as route values can not be mapped to complex types.
            request.ShoppingListId = id;
            request.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = await _mediator.Send(request, cancellationToken);

            if (result.IsSuccess)
            {
                var lineItem = (LineItemDTO)result.Value;
                return CreatedAtAction(nameof(Get), new { lineItem.Id }, lineItem);
            }

            return StatusCode(result.StatusCode);
        }

        [HttpPost]
        [Route("shoppinglist/{id}/lineitem/{lineId}")]
        [Authorize]
        public async Task<IActionResult> Edit([FromRoute] int id, [FromRoute] int lineId,[FromBody] EditLineItemCommand request, CancellationToken cancellationToken)
        {
            // Map the Id to the request as route values can not be mapped to complex types.
            request.Id = lineId;
            request.ShoppingListId = id;
            request.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = await _mediator.Send(request, cancellationToken);

            if (result.IsSuccess)
                return Ok(result.Value);

            return StatusCode(result.StatusCode);
        }

        [HttpDelete]
        [Route("shoppinglist/{id}/lineitem/{lineId}")]
        [Authorize]
        public async Task<IActionResult> Delete([FromRoute] int id, [FromRoute] int lineId, DeleteLineItemCommand request, CancellationToken cancellationToken)
        {
            // Map the Id to the request as route values can not be mapped to complex types.
            request.Id = lineId;
            request.ShoppingListId = id;
            request.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = await _mediator.Send(request, cancellationToken);

            if (result.IsSuccess)
                return Ok();

            return StatusCode(result.StatusCode);
        }
    }
}
