using API.Data;
using API.Interfaces;
using API.Services;
using Microsoft.EntityFrameworkCore;

namespace API.Extensions
{
    public static class ApplicationServiceExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(opt => opt.UseNpgsql(configuration.GetConnectionString("Default")));

            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddScoped<IUsersService, UsersService>();
            services.AddScoped<IMessageService, MessageService>();

            return services;
        }
    }
}