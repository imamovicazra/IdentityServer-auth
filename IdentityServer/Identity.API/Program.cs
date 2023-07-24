using AutoMapper;
using Identity.API.AutoMapper;
using Identity.API.Configuration;
using Identity.API.Extensions;
using Identity.Database;
using Identity.Model.DTOs.Email.Types;
using Identity.Model.Entities;
using Identity.Model.Interfaces;
using Identity.Service.Services;
using MailKit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NETCore.MailKit.Extensions;
using NETCore.MailKit.Infrastructure.Internal;

var builder = WebApplication.CreateBuilder(args);
 
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
//builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();
builder.Services.AddSwaggerConfiguration();
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("IdentityDb")));
builder.Services.AddIdentityServerConfiguration(builder.Configuration);

builder.Services.AddSingleton(sp => new MapperConfiguration(cfg =>
{
    cfg.AddProfile(new AutoMapperProfile());
}).CreateMapper());


var applicationConfiguration = builder.Configuration.GetSection("MailSettings").Get<MailSettings>();
builder.Services.AddMailKit(config =>
{
    config.UseMailKit(new MailKitOptions()
    {
        Server = applicationConfiguration.Host,
        Port = applicationConfiguration.Port,
        Security = applicationConfiguration.UseStartTls,
        SenderName = applicationConfiguration.DisplayName,
        SenderEmail = applicationConfiguration.From,
        Account =applicationConfiguration.UserName,
        Password = applicationConfiguration.Password
    });
});

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IIdentityUserService<ApplicationUser>, IdentityUserService<ApplicationUser>>();
builder.Services.AddSingleton<List<IEmailClassifier>>(_ => {
    return new List<IEmailClassifier>() {
                    new Verification(),
                    new ForgotPassword(),
                    new ChangePassword(),
                };
});

builder.Services.AddScoped<IAuthenticationEmailService, AuthenticationEmailService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    
    app.UseSwaggerConfiguration();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
app.UseIdentityServer();

await app.UseIdentityServerDataAsync(builder.Configuration)
            .ConfigureAwait(false);
        
app.Run();
