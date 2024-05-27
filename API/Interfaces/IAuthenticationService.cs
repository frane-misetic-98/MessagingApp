using API.DTOs.Requests;
using API.DTOs.Responses;
using Microsoft.AspNetCore.Mvc;

namespace API.Interfaces
{
    public interface IAuthenticationService
    {
        Task<ActionResult<LoginResponse>> RegisterUser(RegisterUserRequest request);
        Task<ActionResult<LoginResponse>> Login(LoginRequest request);
    }
}