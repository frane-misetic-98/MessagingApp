using API.DTOs;
using API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize]
    public class UsersController(IUsersService usersService, ILogger<UsersController> logger, IHttpContextAccessor httpContextAccessor) : ApplicationBaseController
    {
        private readonly IUsersService _usersService = usersService;
        private readonly ILogger<UsersController> _logger = logger;
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

        [HttpGet("{id}")]
        public async Task<ActionResult<UserDto>> GetUser(int id)
        {
            _logger.LogInformation("GetUser(id: {id}), request user: {username}", id, _httpContextAccessor.HttpContext.User.Identity.Name);
            return await _usersService.GetUser(id);
        }

        [HttpGet]
        public async Task<ActionResult<List<UserDto>>> GetUsers()
        {
            _logger.LogInformation("GetUsers, request user: {}", _httpContextAccessor.HttpContext.User.Identity.Name);
            return await _usersService.GetUsers();
        }
    }
}