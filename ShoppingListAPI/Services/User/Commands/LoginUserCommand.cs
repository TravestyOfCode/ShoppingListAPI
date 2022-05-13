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
        private readonly SignInManager<IdentityUser> _signInManager;

        private readonly ILogger<LoginCommandHandler> _logger;

        private readonly IConfiguration _configuration;

        public LoginCommandHandler(SignInManager<IdentityUser> signInManager, ILogger<LoginCommandHandler> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
            _signInManager = signInManager;
        }

        public async Task<IResult> Handle(LoginUserCommand request, CancellationToken cancellationToken)
        {
            try
            {
                
                var user = await _signInManager.UserManager.FindByNameAsync(request.UserName);

                if(user == null || !await _signInManager.UserManager.CheckPasswordAsync(user, request.Password))
                {
                    return Result.Unauthorized();
                }

                var userPrincipal = await _signInManager.CreateUserPrincipalAsync(user);

                var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

                var token = new JwtSecurityToken(
                    issuer: _configuration["JWT:Issuer"],
                    audience: _configuration["JWT:Audience"],
                    expires: DateTime.Now.AddHours(5),
                    claims: userPrincipal.Claims,
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
