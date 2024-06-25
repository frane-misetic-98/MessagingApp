using API.Data;
using API.DTOs.Requests;
using API.DTOs.Responses;
using API.Entities;
using API.Interfaces;
using API.Services;
using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace API_test.Services
{
    public class AuthenticationServiceTest
    {
        private readonly UserManager<User> _userManager;
        private readonly ITokenService _tokenService;
        private readonly ILogger<AuthenticationService> _logger;
        private readonly IUsersService _usersService;
        private readonly AuthenticationService _authenticationService;

        public AuthenticationServiceTest()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                  .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                  .Options;

            var dbContext = new ApplicationDbContext(options);
            dbContext.Database.EnsureCreated();

            _userManager = A.Fake<UserManager<User>>();
            _tokenService = A.Fake<ITokenService>();
            _logger = A.Fake<ILogger<AuthenticationService>>();
            _usersService = A.Fake<IUsersService>();

            _authenticationService = new AuthenticationService(
                _tokenService,
                _usersService,
                _userManager,
                _logger
            );
        }


        [Theory]
        [InlineData("username1", "token1")]
        [InlineData("username2", "token2")]
        public async void AuthenticationService_Login_ShouldReturnLoginResponse(string username, string token)
        {
            //ARRANGE
            LoginRequest request = new()
            {
                UserName = username,
                Password = "password",
            };

            User user = new() { Id = 1, UserName = username, PasswordHash = "password" };

            A.CallTo(() => _userManager.FindByNameAsync(request.UserName))
             .Returns(user);

            A.CallTo(() => _userManager.CheckPasswordAsync(user, request.Password))
                .Returns(true);

            A.CallTo(() => _tokenService.CreateToken(user)).Returns(token);
            //ACT
            var result = await _authenticationService.Login(request);
            //ASSERT
            result.Should().NotBeNull();
            result.Should().BeOfType<ActionResult<LoginResponse>>();
            result.Value.Should().NotBeNull();
            result.Value?.UserName.Should().Be(username);
            result.Value?.Token.Should().BeOfType<string>();
            result.Value?.Token.Should().Be(token);
        }

        [Fact]
        public async void AuthenticationService_Login_ShouldReturn401IfUsernameInvalid()
        {
            //ARRANGE
            LoginRequest request = new()
            {
                UserName = "username",
                Password = "password",
            };


            A.CallTo(() => _userManager.FindByNameAsync(request.UserName))
                .Returns<User?>(null);

            //ACT
            var result = await _authenticationService.Login(request);
            //ASSERT
            result.Should().NotBeNull();
            result.Should().BeOfType<ActionResult<LoginResponse>>();
            result.Value.Should().BeNull();
            result.Result.Should().BeOfType<UnauthorizedObjectResult>();
        }

        [Fact]
        public async void AuthenticationService_Login_ShouldReturn401IfPasswordInvalid()
        {
            //ARRANGE
            LoginRequest request = new()
            {
                UserName = "username",
                Password = "password",
            };

            User user = new() { Id = 1, UserName = request.UserName, PasswordHash = "password" };

            A.CallTo(() => _userManager.FindByNameAsync(request.UserName))
                .Returns(user);

            A.CallTo(() => _userManager.CheckPasswordAsync(user, request.Password))
                .Returns(false);

            //ACT
            var result = await _authenticationService.Login(request);
            //ASSERT
            result.Should().NotBeNull();
            result.Should().BeOfType<ActionResult<LoginResponse>>();
            result.Value.Should().BeNull();
            result.Result.Should().BeOfType<UnauthorizedObjectResult>();
        }

        [Fact]
        public async void AuthenticationService_RegisterUser_UsersServiceMethodShouldbeCalled()
        {
            //ARRANGE
            RegisterUserRequest request = new()
            {
                UserName = "username",
                Password = "password",
            };

            LoginResponse response = new()
            {
                UserName = "username",
                Token = "token"
            };

            A.CallTo(() => _usersService.CreateUser(request))
                .Returns(response);
            //ACT
            var result = await _authenticationService.RegisterUser(request);
            //ASSERT
            A.CallTo(() => _usersService.CreateUser(request)).MustHaveHappenedOnceExactly();
            result.Should().NotBeNull();
            result.Should().BeOfType<ActionResult<LoginResponse>>();
            result.Value.Should().BeEquivalentTo(response);
        }
    }
}