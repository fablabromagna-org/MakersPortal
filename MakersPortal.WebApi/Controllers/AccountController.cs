using System.IdentityModel.Tokens.Jwt;
using MakersPortal.Core.Dtos;
using MakersPortal.Core.Models;
using MakersPortal.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace MakersPortal.WebApi.Controllers
{
    [ApiController]
    [Route("/Account")]
    public class AccountController : Controller
    {
        private IUserService _userService;

        public AccountController(/* IUserService userService */ )
        {
           // _userService = userService;
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
        
        [Route("/Account")]
        public IActionResult Index()
        {
            return Ok(new ContentResult()
            {
                Content = "Hello, World."
            });
        }
    }
}