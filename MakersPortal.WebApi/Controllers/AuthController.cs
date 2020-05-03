using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
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
        private readonly IKeyManager _keyManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger _logger;

        public AuthController(IKeyManager keyManager, IUserService userService,
            UserManager<ApplicationUser> userManager, ILogger logger)
        {
            Debug.Assert(keyManager != null);
            Debug.Assert(userService != null);
            Debug.Assert(userManager != null);
            Debug.Assert(logger != null);

            _userService = userService;
            _keyManager = keyManager;
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
                    return Problem($"Unable to create the user. Request Id: {HttpContext.Connection.Id}");
                }
            }
            else if (user.LockoutEnabled)
                return new ForbidResult();
/*
            string sessionToken = _userService.CreateSessionAsync(user);

            return Ok(new JwtTokenDto
            {
                Token = new JwtSecurityTokenHandler().WriteToken(sessionToken)
            });*/
            return Ok();
        }
    }
}