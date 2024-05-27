using API.DTOs;
using API.DTOs.Requests;
using API.DTOs.Responses;
using Microsoft.AspNetCore.Mvc;

namespace API.Interfaces
{
    public interface IUsersService
    {
        Task<ActionResult<List<UserDto>>> GetUsers();
        Task<ActionResult<UserDto>> GetUser(int id);
        Task<ActionResult<LoginResponse>> CreateUser(RegisterUserRequest request);
    }
}