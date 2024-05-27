using API.Data;
using API.DTOs;
using API.DTOs.Requests;
using API.DTOs.Responses;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace API.Services
{
    public class UsersService(ApplicationDbContext context, UserManager<User> userManager, IMapper mapper, ITokenService tokenService, ILogger<UsersService> logger) : IUsersService
    {
        private readonly ApplicationDbContext _context = context;
        private readonly UserManager<User> _userManager = userManager;
        private readonly IMapper _mapper = mapper;
        private readonly ITokenService _tokenService = tokenService;
        private readonly ILogger<UsersService> _logger = logger;

        public async Task<ActionResult<UserDto>> GetUser(int id)
        {
            _logger.LogInformation("GetUser, (id: {id})", id);

            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                _logger.LogError("User with id: {id} not found", id);

                return new NotFoundObjectResult($"User with id: {id} not found");
            }

            return _mapper.Map<UserDto>(user);
        }

        public async Task<ActionResult<List<UserDto>>> GetUsers()
        {
            _logger.LogInformation("GetUsers");

            var users = await _context.Users.ToListAsync();

            return _mapper.Map<List<UserDto>>(users);
        }

        public async Task<ActionResult<LoginResponse>> CreateUser(RegisterUserRequest request)
        {
            _logger.LogInformation("CreateUser, request: {request}", JsonConvert.SerializeObject(request));

            if (await UserExists(request.UserName))
            {
                _logger.LogError("Username {username} is taken", request.UserName);

                return new BadRequestObjectResult("Username is taken");
            }

            var user = _mapper.Map<User>(request);

            var result = await _userManager.CreateAsync(user, request.Password);

            if (!result.Succeeded)
            {
                _logger.LogError("{}", result.Errors);

                return new BadRequestObjectResult(result.Errors);
            }

            return new LoginResponse
            {
                UserName = user.UserName,
                Token = _tokenService.CreateToken(user),
            };
        }
        private async Task<bool> UserExists(string username) => await _context.Users.AnyAsync(x => x.UserName == username);
    }
}