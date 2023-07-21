using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;
using System.Reflection;

namespace Identity.API.Extensions
{
    public static class SwaggerConfiguration
    {
        public static void AddSwaggerConfiguration(this IServiceCollection services)
        {
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Auth Api",
                    Version = "v1",
                    Description = "Auth Service"
                });

                options.EnableAnnotations();
                // Bearer token authentication                
                options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme,
                    new OpenApiSecurityScheme()
                    {
                        Name = JwtBearerDefaults.AuthenticationScheme,
                        BearerFormat = "JWT",
                        Scheme = JwtBearerDefaults.AuthenticationScheme,
                        Description = "Specify the authorization token.",
                        In = ParameterLocation.Header,
                        Type = SecuritySchemeType.Http,
                    }
                );

                options.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                    {
                        new OpenApiSecurityScheme()
                        {
                            Reference = new OpenApiReference()
                            {
                                Id = JwtBearerDefaults.AuthenticationScheme,
                                Type = ReferenceType.SecurityScheme
                            }
                        },
                        new string[] { }
                    },
                });
            });
        }

        public static void UseSwaggerConfiguration(this IApplicationBuilder app)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.DefaultModelsExpandDepth(-1); // Disable swagger schemas at bottom
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Identity.Api v1");
            });
        }
    }
}
