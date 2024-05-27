using API.DTOs.Requests;
using API.DTOs.Responses;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace API.Controllers
{
    public class AuthenticationController(IAuthenticationService authenticationService, ILogger<AuthenticationController> logger) : ApplicationBaseController
    {
        private readonly IAuthenticationService _authenticationService = authenticationService;
        private readonly ILogger<AuthenticationController> _logger = logger;

        [HttpPost("login")]
        public async Task<ActionResult<LoginResponse>> Login(LoginRequest request)
        {
            _logger.LogInformation("Login, request: {}", JsonConvert.SerializeObject(request));
            return await _authenticationService.Login(request);
        }

        [HttpPost("register")]
        public async Task<ActionResult<LoginResponse>> Register(RegisterUserRequest request)
        {
            _logger.LogInformation("Register, request: {}", JsonConvert.SerializeObject(request));
            return await _authenticationService.RegisterUser(request);
        }
    }
}