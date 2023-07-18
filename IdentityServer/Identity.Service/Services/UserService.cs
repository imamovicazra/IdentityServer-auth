using AutoMapper;
using Identity.Database;
using Identity.Model.DTOs.Requests;
using Identity.Model.DTOs.Responses;
using Identity.Model.Entities;
using Identity.Service.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Identity.Service.Services
{
    public class UserService:IUserService
    {
        private readonly IConfiguration _config;
        private readonly IMapper _mapper;
        private readonly ILogger<UserService> _logger;
        private readonly AppDbContext _appDbContext;

        public UserService(IConfiguration config,IMapper mapper, ILogger<UserService> logger,
            AppDbContext dbContext)
        {
            _config= config; 
            _mapper= mapper;
            _logger= logger;
            _appDbContext= dbContext;
        }

        public Task<UserResponse> GetUserAsync(string userId, string email)
        {
            throw new NotImplementedException();
        }

        public Task<IdentityResult> RegisterAsync(ApplicationUserRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
