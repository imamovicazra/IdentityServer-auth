using AutoMapper;
using Identity.API.AutoMapper;
using Identity.API.Extensions;
using Identity.Database;
using Identity.Model.Entities;
using Identity.Service.Interfaces;
using Identity.Service.Services;
using IdentityServer4.AspNetIdentity;
using IdentityServer4.Validation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;

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


builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IIdentityUserService<ApplicationUser>, IdentityUserService<ApplicationUser>>();

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
