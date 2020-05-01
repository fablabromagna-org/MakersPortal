using System.Linq;
using System.Security.Claims;
using MakersPortal.Core.Dtos;
using MakersPortal.Core.Models;
using MakersPortal.Infrastructure.Options;
using MakersPortal.Infrastructure.Services;
using Microsoft.Extensions.Options;
using Xunit;

namespace MakersPortal.Tests.Unit
{
    public class UserServiceTests : IClassFixture<TestsFixture>
    {
        private readonly TestsFixture _fixture;
        private readonly UserService _userService;

        public UserServiceTests(TestsFixture fixture)
        {
            _fixture = fixture;

            var idpOptions = Options.Create(new IdentityProvidersOptions()
            {
                IdentityProviders = new []
                {
                    new IdentityProvider
                    {
                        Audience = "https://client.example.com",
                        Issuer = "https://account.example.com",
                        SkipValidation = true
                    }
                }
            });
            _userService = new UserService(idpOptions);
        }

        [Fact]
        public async void ValidateExternalJwtToken_Success_WhenTokenIsValid()
        {
            string familyName = "Edoardo";
            var jwtDto = new JwtTokenDto
            {
                Token = _fixture.GetJwt(familyName: familyName)
            };

            var token = await _userService.ValidateExternalJwtToken(jwtDto);
            Assert.NotNull(token);

            Claim givenNameClaim = token.Claims.First(p => p.Type ==  ClaimTypes.GivenName);
            string givenName = givenNameClaim?.Value ?? "";
            Assert.Equal(familyName, givenName);
        }
    }
}