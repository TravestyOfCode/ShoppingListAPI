using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ShoppingListAPI.Services.User.Commands
{
    public class LoginUserCommand : IRequest<IResult>
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        public string Password { get; set; }
    }

    public class LoginCommandHandler : IRequestHandler<LoginUserCommand, IResult>
    {
        private readonly UserManager<IdentityUser> _userManager;

        private readonly ILogger<LoginCommandHandler> _logger;

        private readonly IConfiguration _configuration;

        public LoginCommandHandler(UserManager<IdentityUser> userManager, ILogger<LoginCommandHandler> logger, IConfiguration configuration)
        {
            _userManager = userManager;
            _logger = logger;
            _configuration = configuration;
        }

        public async Task<IResult> Handle(LoginUserCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _userManager.FindByNameAsync(request.UserName);

                if(user == null || !await _userManager.CheckPasswordAsync(user, request.Password))
                {
                    return Result.Unauthorized();
                }

                // Get all user claims and put into token
                var claims = await _userManager.GetClaimsAsync(user);

                var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

                var token = new JwtSecurityToken(
                    issuer: _configuration["JWT:Issuer"],
                    audience: _configuration["JWT:Audience"],
                    expires: DateTime.Now.AddHours(5),
                    claims: claims,
                    signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                    );

                return Result.Ok(new { token = new JwtSecurityTokenHandler().WriteToken(token), expiration = token.ValidTo } );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error handling request: {request}", request);
                return Result.ServerError();
            }
        }

        
    }
}
