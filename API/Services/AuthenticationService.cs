using API.DTOs.Requests;
using API.DTOs.Responses;
using API.Entities;
using API.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace API.Services
{
    public class AuthenticationService(ITokenService tokenService, IUsersService usersService, UserManager<User> userManager, ILogger<AuthenticationService> logger) : IAuthenticationService
    {
        private readonly ITokenService _tokenService = tokenService;
        private readonly IUsersService _usersService = usersService;
        private readonly UserManager<User> _userManager = userManager;
        private readonly ILogger<AuthenticationService> _logger = logger;

        public async Task<ActionResult<LoginResponse>> Login(LoginRequest request)
        {
            var user = await _userManager.FindByNameAsync(request.UserName);
            if (user == null)
            {
                _logger.LogError("User with username: {} already exists", request.UserName);
                return new UnauthorizedObjectResult("Invalid username");
            }

            var result = await _userManager.CheckPasswordAsync(user, request.Password);
            if (!result)
            {
                _logger.LogError("Invalid password");
                return new UnauthorizedObjectResult("Invalid password");
            }

            return new LoginResponse
            {
                UserName = user.UserName,
                Token = _tokenService.CreateToken(user),
            };
        }

        public Task<ActionResult<LoginResponse>> RegisterUser(RegisterUserRequest request)
        {
            _logger.LogInformation("Req: {req}", JsonConvert.SerializeObject(request));

            return _usersService.CreateUser(request);
        }
    }
}