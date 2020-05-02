using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;
using MakersPortal.Core.Models;
using MakersPortal.Core.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace MakersPortal.Infrastructure.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IKeyManager _keyManager;
        
        public UserService(UserManager<ApplicationUser> userManager, IKeyManager keyManager)
        {
            Debug.Assert(userManager != null);
            Debug.Assert(keyManager != null);

            _userManager = userManager;
            _keyManager = keyManager;
        }

        public async Task<string> CreateSessionAsync(ApplicationUser user)
        {
            /*var roles = await _userManager.GetRolesAsync(user);

            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.GivenName, user.GivenName),
                new Claim(ClaimTypes.Surname, user.Surname),
                new Claim(ClaimTypes.Email, user.Email)
            };

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddDays(15),
                Issuer = "",
                Audience = ""
            };
            
            

            return handler.WriteToken(token);*/

            return string.Empty;
        }
    }
}