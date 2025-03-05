using BE._iservices;
using BE._services;
using BE.Context;
using BE.Controllers;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

namespace BE._extensions
{
    public static class ApplicationServiceExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection Services, IConfiguration configuration)
        {
            Services.AddControllers();
            Services.AddCors();
            Services.AddScoped<EmailController>();
            Services.AddScoped<UserCardController>();
            Services.AddScoped<IContentService, ContentService>();
            Services.AddScoped<ITokenService, TokenService>();
            Services.AddDbContext<DataContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
            });

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            Services.AddSwaggerGen(c => {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Your API", Version = "v1" });

                // Thêm xác thực JWT
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme.",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" },

                        },
                        new string[] {}
                    }
                });
            });
            
            return Services;
        }
    }
}