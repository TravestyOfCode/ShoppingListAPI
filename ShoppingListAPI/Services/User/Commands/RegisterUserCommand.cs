using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;

namespace ShoppingListAPI.Services.User.Commands
{
    public class RegisterUserCommand : IRequest<IResult>
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        [EmailAddress]
        public string EmailAddress { get; set; }

        [Required]
        public string Password { get; set; }

        public bool IsAdmin { get; set; }
    }

    public class RegisterCommandHandler : IRequestHandler<RegisterUserCommand, IResult>
    {
        private readonly UserManager<IdentityUser> _userManager;

        private readonly ILogger<RegisterCommandHandler> _logger;

        public RegisterCommandHandler(UserManager<IdentityUser> userManager, ILogger<RegisterCommandHandler> logger)
        {
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<IResult> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var userExists = await _userManager.FindByNameAsync(request.UserName);
                if (userExists != null)
                    return Result.BadRequest(nameof(request.UserName), "User already exists.");

                IdentityUser user = new()
                {
                    Email = request.EmailAddress,
                    SecurityStamp = Guid.NewGuid().ToString(),
                    UserName = request.UserName
                };

                var result = await _userManager.CreateAsync(user, request.Password);

                if (!result.Succeeded)
                    return Result.ServerError();

                await _userManager.AddToRoleAsync(user, Resource.User);
                if(request.IsAdmin)
                {
                    await _userManager.AddToRoleAsync(user, Resource.Admin);
                }

                return Result.Created();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error handling request: {request}", request);
                return Result.ServerError();
            }
        }
    }
}
