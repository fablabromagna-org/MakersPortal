using MakersPortal.Core.Dtos;
using MakersPortal.Core.Models;
using MakersPortal.Infrastructure.Services;
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
            _userService = new UserService(new[]
            {
                new IdentityProvider
                {
                    Audience = "https://client.example.com",
                    Issuer = "https://account.example.com",
                    Name = "sample-idp",
                    SkipValidation = true
                }
            });
        }

        [Fact]
        public void ValidateExternalJwtToken_Success_WhenTokenIsValid()
        {
            var jwtDto = new JwtTokenDto
            {
                Token = _fixture.GetJwt()
            };

            Assert.True(_userService.ValidateExternalJwtToken(jwtDto, out var validatedToken));
            
            // It's useless checking the result, if we will do that we will test Microsoft's validator
            Assert.NotNull(validatedToken);
        }
    }
}