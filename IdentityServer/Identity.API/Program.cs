using AutoMapper;
using Identity.API.AutoMapper;
using Identity.API.Extensions;
using Identity.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;

var builder = WebApplication.CreateBuilder(args);
 
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("IdentityDb")));
builder.Services.AddIdentityServerConfiguration(builder.Configuration);

//builder.Services.AddAutoMapper(typeof(AutoMapperProfile));
builder.Services.AddSingleton(sp => new MapperConfiguration(cfg =>
{
    cfg.AddProfile(new AutoMapperProfile());
}).CreateMapper());



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
app.UseIdentityServer();

await app.UseIdentityServerDataAsync(builder.Configuration)
            .ConfigureAwait(false);
        
app.Run();
