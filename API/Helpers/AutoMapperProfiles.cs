using API.DTOs;
using API.DTOs.Requests;
using API.Entities;
using AutoMapper;

namespace API.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<User, UserDto>();
            CreateMap<LoginRequest, User>();
            CreateMap<RegisterUserRequest, User>();
            CreateMap<Message, MessageDto>();
        }
    }
}