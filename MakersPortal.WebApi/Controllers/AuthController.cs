using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using MakersPortal.Core.Dtos;
using MakersPortal.Core.Models;
using MakersPortal.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.KeyVault.WebKey;
using Microsoft.IdentityModel.Tokens;
using JsonWebKey = Microsoft.IdentityModel.Tokens.JsonWebKey;

namespace MakersPortal.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : Controller
    {
        private IUserService _userService;

        public AuthController(/* IUserService userService */ )
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
        
        [HttpGet]
        public string Index()
        {
            return "Hello, World.";
        }
        
        [HttpGet]
        [Route("~/.well-known/jwks.json")]
        [AllowAnonymous]
        public IActionResult WellKnown()
        {
            var key = RSA.Create(2048);
            var parameters = key.ExportParameters(false);
            
            var temp = new JwkDto
            {
                E = Convert.ToBase64String(parameters.Exponent),
                Kty = JsonWebKeyType.Rsa,
                Alg = JsonWebKeySignatureAlgorithm.RS256,
                Use = JsonWebKeyUseNames.Sig,
                N =  Convert.ToBase64String(parameters.Modulus),
                Kid = Guid.NewGuid().ToString()
            };
            
            return Ok(new JwksDto
            {
                Keys = new [] { temp }
            });
        }
    }
}