using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;
using MakersPortal.Core.Dtos;
using MakersPortal.Core.Models;
using MakersPortal.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace MakersPortal.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : Controller
    {
        private IUserService _userService;
        private readonly IKeyManager _keyManager;

        public AuthController(IKeyManager keyManager /* IUserService userService */)
        {
            // _userService = userService;
            _keyManager = keyManager;
        }

        [AllowAnonymous]
        [Route("/Login")]
        [HttpPost]
        public IActionResult Login([FromBody] JwtTokenDto token)
        {
            SecurityToken validatedToken;

            if (!_userService.ValidateExternalJwtToken(token, out validatedToken))
                return Unauthorized();

            ApplicationUser user = _userService.Find(validatedToken);

            if (user == null)
                user = _userService.Create(validatedToken);

            SecurityToken sessionToken = _userService.CreateSession(user, validatedToken);

            return Ok(new JwtTokenDto
            {
                Token = new JwtSecurityTokenHandler().WriteToken(sessionToken)
            });
        }

        [HttpGet]
        [Route("~/.well-known/jwks.json")]
        [AllowAnonymous]
        public async Task<IActionResult> WellKnown()
        {
            JwkDto jwk = await _keyManager.GetPublicFromName("jwt");

            return Ok(new JwksDto
            {
                Keys = new[] {jwk}
            });
        }
    }
}