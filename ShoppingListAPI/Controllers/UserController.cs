using MediatR;
using Microsoft.AspNetCore.Mvc;
using ShoppingListAPI.Services.User.Commands;
using System.IdentityModel.Tokens.Jwt;
using System.Threading;
using System.Threading.Tasks;
using static ShoppingListAPI.Models.Strings;

namespace ShoppingListAPI.Controllers
{
    [ApiController]
    [Route("")]
    public class UserController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UserController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody]LoginUserCommand request, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var result = await _mediator.Send(request, cancellationToken);

            if (result.IsSuccess)
            {
                var token = (JwtSecurityToken)result.Value;

                return Ok(new { token, expires = token.ValidTo });
            }

            return StatusCode(result.StatusCode);
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody]RegisterUserCommand request, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            // If the user is not an admin, they can't set the IsAdmin value
            if (!User.IsInRole(Roles.Admin))
                request.IsAdmin = false;

            var result = await _mediator.Send(request, cancellationToken);

            return StatusCode(result.StatusCode);
        }
    }
}
