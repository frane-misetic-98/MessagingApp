using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace API.Extensions
{
    public static class IdentityServiceExtensions
    {
        public static IServiceCollection AddIdentityServices(this IServiceCollection services, IConfiguration config)
        {
            services.AddIdentityCore<User>(opt =>
            {
                opt.Password.RequireNonAlphanumeric = false;
                opt.Password.RequireDigit = false;
                opt.Password.RequireUppercase = false;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["TokenKey"])),
                        ValidateIssuer = false,
                        ValidateAudience = false
                    };

                    // options.Events = new JwtBearerEvents
                    // {
                    //     OnMessageReceived = context =>
                    //     {
                    //         var accessToken = context.Request.Query["access_token"];

                    //         var path = context.HttpContext.Request.Path;
                    //         if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs"))
                    //         {
                    //             context.Token = accessToken;
                    //         }

                    //         return Task.CompletedTask;
                    //     }
                    // };
                });

            services.AddAuthorization();

            return services;
        }
    }
}