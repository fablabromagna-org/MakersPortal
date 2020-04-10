using System;
using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;
using AutoMapper;
using MakersPortal.Core.Dtos;
using MakersPortal.Core.Models;
using MakersPortal.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.KeyVault.WebKey;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using JsonWebKey = Microsoft.IdentityModel.Tokens.JsonWebKey;

namespace MakersPortal.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : Controller
    {
        private IUserService _userService;
        private readonly IKeyManager _keyManager;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public AuthController(IKeyManager keyManager, IMapper mapper,
            IConfiguration configuration /* IUserService userService */)
        {
            // _userService = userService;
            _keyManager = keyManager;
            _mapper = mapper;
            _configuration = configuration;
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
        public string Index()
        {
            return "Hello, World.";
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