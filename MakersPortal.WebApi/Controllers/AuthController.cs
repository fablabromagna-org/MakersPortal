using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using MakersPortal.Core.Dtos;
using MakersPortal.Core.Exceptions.HttpExceptions;
using MakersPortal.Core.Models;
using MakersPortal.Core.Services;
using MakersPortal.WebApi.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace MakersPortal.WebApi.Controllers
{
    public class AuthController : BaseApiController
    {
        private readonly IUserService _userService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger _logger;

        public AuthController(IUserService userService, UserManager<ApplicationUser> userManager,
            ILogger<AuthController> logger)
        {
            Debug.Assert(userService != null);
            Debug.Assert(userManager != null);
            Debug.Assert(logger != null);

            _userService = userService;
            _userManager = userManager;
            _logger = logger;
        }

        [HttpGet]
        [Authorize(Policy = PoliciesConstants.EXTERNAL_IDP_ONLY_POLICY)]
        public async Task<IActionResult> Login()
        {
            ApplicationUser tmpUser = new ApplicationUser
            {
                Email = User.Claims.First(p => p.Type == ClaimTypes.Email).Value,
                GivenName = User.Claims.First(p => p.Type == ClaimTypes.GivenName).Value,
                Surname = User.Claims.First(p => p.Type == ClaimTypes.Surname).Value,
                UserName = User.Claims.First(p => p.Type == ClaimTypes.NameIdentifier).Value,
                EmailConfirmed = true,
            };

            ApplicationUser user =
                await _userManager.FindByEmailAsync(User.Claims.First(p => p.Type == ClaimTypes.Email).Value);

            if (user == null)
            {
                var result = await _userManager.CreateAsync(tmpUser);

                if (!result.Succeeded)
                {
                    _logger.LogError(
                        $"Unable to create the user. Reasons: {result.Errors}. Request ID: {HttpContext.Connection.Id}");
                    throw new InternalServerErrorException();
                }

                user = tmpUser;
            }
            else if (user.LockoutEnabled)
                return new ForbidResult();

            var token = await _userService.CreateSessionAsync(user);

            return Ok(new JwtTokenDto()
            {
                Token = token
            });
        }
    }
}