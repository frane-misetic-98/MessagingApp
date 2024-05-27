using API.Data;
using API.DTOs;
using API.DTOs.Requests;
using API.DTOs.Responses;
using API.Entities;
using API.Interfaces;
using API.Services;
using AutoMapper;
using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace API_test.Services
{
    public class UsersServiceTest
    {
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly UsersService _usersService;
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;
        private readonly ITokenService _tokenService;
        private readonly ILogger<UsersService> _logger;

        public UsersServiceTest()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                    .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                    .Options;

            var dbContext = new ApplicationDbContext(options);
            dbContext.Database.EnsureCreated();

            _applicationDbContext = dbContext;
            _userManager = A.Fake<UserManager<User>>();
            _mapper = A.Fake<IMapper>();
            _tokenService = A.Fake<ITokenService>();
            _logger = A.Fake<ILogger<UsersService>>();

            _usersService = new UsersService(
                _applicationDbContext,
                _userManager,
                _mapper,
                _tokenService,
                _logger
            );
        }

        [Theory]
        [InlineData("username1", "password1")]
        [InlineData("username2", "password2")]
        public async void UsersService_CreateUser_ReturnsLoginResponse(string username, string password)
        {
            //ARRANGE
            var request = new RegisterUserRequest
            {
                UserName = username,
                Password = password
            };
            //ACT
            var result = await _usersService.CreateUser(request);
            //ASSERT
            result.Should().NotBeNull();
            result.Should().BeOfType<ActionResult<LoginResponse>>();
            result.Value.Should().NotBeNull();
            result.Value?.UserName.Should().Be(username);
            result.Value?.Token.Should().BeOfType<string>();

            await AfterEach();
        }

        [Fact]
        public async void UsersService_CreateUser_ReturnsBadRequestIfUserExists()
        {
            //ARRANGE
            var user = new User
            {
                UserName = "username",
                PasswordHash = "password"
            };

            _applicationDbContext.Users.Add(user);
            await _applicationDbContext.SaveChangesAsync();

            var request = new RegisterUserRequest()
            {
                UserName = user.UserName,
                Password = user.PasswordHash
            };

            //ACT
            var result = await _usersService.CreateUser(request);
            //ASSERT
            result.Should().NotBeNull();
            result.Should().BeOfType<ActionResult<LoginResponse>>();
            result.Result.Should().BeOfType<BadRequestObjectResult>();

            await AfterEach();
        }

        [Fact]
        public async void UsersService_CreateUser_ReturnsBadRequestIfUserManagerFailsToCreateUser()
        {
            //ARRANGE
            var user = new User
            {
                UserName = "username",
                PasswordHash = "password"
            };

            var request = new RegisterUserRequest()
            {
                UserName = user.UserName,
                Password = user.PasswordHash
            };

            A.CallTo(() => _userManager.CreateAsync(user, request.Password))
             .Returns(new IdentityResult());

            //ACT
            var result = await _usersService.CreateUser(request);
            //ASSERT
            result.Should().NotBeNull();
            result.Should().BeOfType<ActionResult<LoginResponse>>();
            result.Result.Should().BeOfType<BadRequestObjectResult>();

            await AfterEach();
        }

        [Theory]
        [InlineData(1, "username1", "password1")]
        [InlineData(2, "username2", "password2")]
        public async Task UsersService_GetUser_ReturnsUserDto(int id, string username, string password)
        {
            //ARRANGE
            var user = new User
            {
                Id = id,
                UserName = username,
                PasswordHash = password
            };

            _applicationDbContext.Users.Add(user);

            await _applicationDbContext.SaveChangesAsync();

            A.CallTo(() => _mapper.Map<UserDto>(user)).Returns(new UserDto { Id = user.Id, UserName = user.UserName });
            //ACT
            var result = await _usersService.GetUser(id);
            //ASSERT
            result.Should().NotBeNull();
            result.Should().BeOfType<ActionResult<UserDto>>();
            result.Value?.Id.Should().Be(id);
            result.Value?.UserName.Should().Be(username);

            await AfterEach();
        }

        [Fact]
        public async Task UsersService_GetUser_ReturnsNotFoundIfUserDoesNotExist()
        {
            //ARRANGE
            var id = 1;
            //ACT
            var result = await _usersService.GetUser(id);
            //ASSERT
            result.Should().NotBeNull();
            result.Should().BeOfType<ActionResult<UserDto>>();
            result.Result.Should().BeOfType<NotFoundObjectResult>();

            await AfterEach();
        }

        [Fact]
        public async Task UsersService_GetUsers_ReturnsUserDtoList()
        {
            //ARRANGE
            var users = new List<User> {
                new()
                {
                    Id = 1,
                    UserName = "username1",
                    PasswordHash = "password1"
                },
                new()
                {
                    Id = 2,
                    UserName = "username2",
                    PasswordHash = "password2"
                },
                new()
                {
                    Id = 3,
                    UserName = "username3",
                    PasswordHash = "password3"
                },
            };

            _applicationDbContext.Users.AddRange(users);

            await _applicationDbContext.SaveChangesAsync();

            A.CallTo(() => _mapper.Map<List<UserDto>>(users)).Returns(
                [
                    new() { Id = 1, UserName = "username1" },
                    new() { Id = 2, UserName = "username2" },
                    new() { Id = 3, UserName = "username3" },
                ]
            );
            //ACT
            var result = await _usersService.GetUsers();
            //ASSERT
            result.Should().NotBeNull();
            result.Should().BeOfType<ActionResult<List<UserDto>>>();
            result.Value.Should().NotBeNull();
            result.Value?.Count.Should().Be(3);
            result.Value?.ElementAt(0).Id.Should().Be(1);
            result.Value?.ElementAt(0).UserName.Should().Be("username1");
            result.Value?.ElementAt(1).Id.Should().Be(2);
            result.Value?.ElementAt(1).UserName.Should().Be("username2");
            result.Value?.ElementAt(2).Id.Should().Be(3);
            result.Value?.ElementAt(2).UserName.Should().Be("username3");

            await AfterEach();
        }


        private async Task AfterEach()
        {
            _applicationDbContext.Users.RemoveRange();
            await _applicationDbContext.SaveChangesAsync();
        }
    }
}