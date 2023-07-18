using IdentityModel.Client;
using Identity.Model.Constants.TokenProviders;
using Identity.Model.Entities;
using Microsoft.EntityFrameworkCore;
using Identity.Service.Services;
using Identity.Model.Constants.Assemblies;
using Identity.Database;
using Microsoft.AspNetCore.Identity;


namespace Identity.API.Extensions
{
    public static class IdentityServerConfiguration
    {
        public static void AddIdentityServerConfiguration(this IServiceCollection services, IConfiguration _config)
        {
            string identityConnectionString = _config.GetConnectionString("IdentityDb");

            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.User.RequireUniqueEmail = true;
                options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+!#$%&'*+-/=?^_`{|}~.";
                options.Password.RequiredLength = 8;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireDigit = true;
                options.Tokens.EmailConfirmationTokenProvider = CustomTokenProviders.EmailDataProtectorTokenProvider;
                options.Tokens.PasswordResetTokenProvider = CustomTokenProviders.PasswordDataProtectorTokenProvider;
            })
             .AddEntityFrameworkStores<AppDbContext>()
             .AddDefaultTokenProviders()
             .AddTokenProvider<EmailConfirmationTokenProvider<ApplicationUser>>(CustomTokenProviders.EmailDataProtectorTokenProvider)
             .AddTokenProvider<PasswordResetTokenProvider<ApplicationUser>>(CustomTokenProviders.PasswordDataProtectorTokenProvider);


            services.AddIdentityServer()
                   .AddDeveloperSigningCredential()
                   .AddAspNetIdentity<ApplicationUser>()
                   //Configuration Store: clients and resources
                   .AddConfigurationStore(options =>
                   {
                       options.ConfigureDbContext = db =>
                       db.UseSqlServer(identityConnectionString,
                           sql => sql.MigrationsAssembly(InternalAssemblies.Database));
                   })
                   //Operational Store: tokens, codes etc.
                   .AddOperationalStore(options =>
                   {
                       options.ConfigureDbContext = db =>
                       db.UseSqlServer(identityConnectionString,
                           sql => sql.MigrationsAssembly(InternalAssemblies.Database));
                   })
                   .AddProfileService<IdentityProfileService>(); // custom claims 

            //Cache Discovery document HttpClient
            services.AddSingleton<IDiscoveryCache>(r =>
            {
                var factory = r.GetRequiredService<IHttpClientFactory>();
                return new DiscoveryCache(_config["AuthApiUrl"], () => factory.CreateClient());
            });


        }
    }
}
