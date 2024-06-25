using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using API.Entities;
using API.Services;
using FluentAssertions;
using Microsoft.Extensions.Configuration;

namespace API_test.Services
{
    public class TokenServiceTest
    {
        private readonly TokenService _tokenService;
        public TokenServiceTest()
        {
            var value = string.Concat(Enumerable.Repeat("a", 200));
            var initialData = new Dictionary<string, string?>
            {
                { "TokenKey", value }
            };

            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(initialData)
                .Build();

            _tokenService = new TokenService(config);
        }

        [Fact]
        public void TokensService_CreateToken_CreatesJwt()
        {
            //ARRANGE
            User user = new()
            {
                Id = 1,
                UserName = "username",
                PasswordHash = "pass"
            };
            //ACT
            var result = _tokenService.CreateToken(user);

            var handler = new JwtSecurityTokenHandler();

            var token = handler.ReadJwtToken(result);

            var nameId = token.Claims.First(c => c.Type == JwtRegisteredClaimNames.NameId).Value;
            var uniqueName = token.Claims.First(c => c.Type == JwtRegisteredClaimNames.UniqueName).Value;
            //ASSERT
            result.Should().NotBeNull();
            result.Should().BeOfType<string>();


            nameId.Should().Be("1");
            uniqueName.Should().Be("username");
        }
    }
}