using System.Diagnostics;
using MakersPortal.Core.Dtos;
using MakersPortal.Core.Services;
using MakersPortal.WebApi.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MakersPortal.WebApi.Controllers
{
    public class AuthController : BaseApiController
    {
        private readonly IUserService _userService;
        private readonly IKeyManager _keyManager;

        public AuthController(IKeyManager keyManager, IUserService userService)
        {
            Debug.Assert(keyManager != null);
            Debug.Assert(userService != null);

            _userService = userService;
            _keyManager = keyManager;
        }
        
        [HttpGet]
        [Authorize(Policy = PoliciesConstants.EXTERNAL_IDP_ONLY_POLICY)]
        public IActionResult Login()
        {
            return Ok(new ContentResult() {Content = "Ciao"});
            /*
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
            */
        }
    }
}