using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Claims;
using System.Threading.Tasks;
using MakersPortal.Core.Exceptions;
using MakersPortal.Core.Models;
using MakersPortal.Core.Services;
using MakersPortal.Infrastructure.Options;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace MakersPortal.Infrastructure.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IKeyManager _keyManager;
        private readonly JwtIssuerOptions _issuerOptions;

        public UserService(UserManager<ApplicationUser> userManager, IKeyManager keyManager,
            IOptions<JwtIssuerOptions> issuerOptions)
        {
            Debug.Assert(userManager != null);
            Debug.Assert(keyManager != null);
            Debug.Assert(issuerOptions != null);

            _userManager = userManager;
            _keyManager = keyManager;
            _issuerOptions = issuerOptions.Value;
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">if <paramref name="user"/> is null.</exception>
        /// <exception cref="UserNotFoundException">if the user not exists.</exception>
        public async Task<string> CreateSessionAsync(ApplicationUser user)
        {
            if (user == null)
                throw new ArgumentNullException();

            var foundUser = await _userManager.FindByEmailAsync(user.Email);

            if (foundUser == null)
                throw new UserNotFoundException();

            var claims = new List<Claim>
            {
                new Claim("email", foundUser.Email),
                new Claim("given_name", foundUser.GivenName),
                new Claim("family_name", foundUser.Surname),
                new Claim("sub", foundUser.Id),
                new Claim("name", foundUser.CommonName)
            };

            IEnumerable<string> roles = await _userManager.GetRolesAsync(foundUser);

            foreach (var role in roles)
            {
                claims.Add(new Claim("roles", role));
            }

            string token = await _keyManager.SignJwtAsync(claims, _issuerOptions.Issuer, _issuerOptions.Audience,
                DateTime.Now, DateTime.Now.AddDays(7));

            return token;
        }
    }
}