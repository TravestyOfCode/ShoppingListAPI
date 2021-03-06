using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoppingListAPI.Services.Product;
using ShoppingListAPI.Services.Product.Commands;
using ShoppingListAPI.Services.Product.Queries;
using System.Threading;
using System.Threading.Tasks;
using static ShoppingListAPI.Models.Strings;

namespace ShoppingListAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ProductController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [Route("")]
        public async Task<IActionResult> Get(CancellationToken cancellationToken)
        {
            GetAllProductsQuery request = new GetAllProductsQuery();

            var result = await _mediator.Send(request, cancellationToken);

            if (result.IsSuccess)
                return Ok(result.Value);

            return StatusCode(result.StatusCode);
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> Get([FromRoute]int id, CancellationToken cancellationToken)
        {
            GetProductByIdQuery request = new GetProductByIdQuery()
            {
                Id = id
            };
            
            var result = await _mediator.Send(request, cancellationToken);

            if (result.IsSuccess)
                return Ok(result.Value);

            return StatusCode(result.StatusCode);
        }

        [HttpPost]
        [Route("")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> Create([FromBody]CreateProductCommand request, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var result = await _mediator.Send(request, cancellationToken);

            if (result.IsSuccess)
            {
                ProductDTO value = (ProductDTO)result.Value;
                return CreatedAtAction(nameof(Get), new { value.Id }, value);
            }                

            return StatusCode(result.StatusCode);
        }

        [HttpPost]
        [Route("{id}")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> Edit([FromRoute] int id, [FromBody] EditProductCommand request, CancellationToken cancellationToken)
        {
            // Map Id as route values can not be bound to complex types.
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
        public async Task<IActionResult> Delete([FromRoute] int id, CancellationToken cancellationToken)
        {
            DeleteProductCommand request = new DeleteProductCommand()
            {
                Id = id
            };

            var result = await _mediator.Send(request, cancellationToken);

            if (result.IsSuccess)
                return Ok(result.Value);

            return StatusCode(result.StatusCode);
        }
    }
}
