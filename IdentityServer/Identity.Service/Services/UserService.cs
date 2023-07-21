using AutoMapper;
using Identity.Database;
using Identity.Model.Constants.Roles;
using Identity.Model.DTOs.Requests;
using Identity.Model.DTOs.Responses;
using Identity.Model.Entities;
using Identity.Service.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using System.Transactions;

namespace Identity.Service.Services
{
    public class UserService:IUserService
    {
        private readonly IConfiguration _config;
        private readonly IMapper _mapper;
        private readonly ILogger<UserService> _logger;
        private readonly AppDbContext _appDbContext;
        private readonly UserManager<ApplicationUser> _userManager;

        public UserService(IConfiguration config,IMapper mapper, ILogger<UserService> logger,
            AppDbContext dbContext, UserManager<ApplicationUser> userManager)
        {
            _config= config; 
            _mapper= mapper;
            _logger= logger;
            _appDbContext= dbContext;
            _userManager= userManager;
        }

        public async Task<UserResponse> GetUserAsync(string userId, string email)
        {
            try
            {
                ApplicationUser user = null;
                if (!string.IsNullOrEmpty(userId))
                {
                    user = await _userManager.FindByIdAsync(userId).ConfigureAwait(false);
                    return user is null ? null : _mapper.Map<ApplicationUser, UserResponse>(user);
                }

                if (!string.IsNullOrEmpty(email))
                {
                    user = await _userManager.FindByEmailAsync(email).ConfigureAwait(false);
                    return user is null ? null : new UserResponse()
                    {
                        IsEmailConfirmed = user.EmailConfirmed
                    };
                }

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, nameof(GetUserAsync));
                throw;
            }
        }

        public async Task<IdentityResult> RegisterAsync(ApplicationUserRequest request)
        {
            try
            {
                ApplicationUser applicationUser = _mapper.Map<ApplicationUserRequest, ApplicationUser>(request);
                IdentityResult identityResult = null;

                using (TransactionScope scope = new(scopeOption: TransactionScopeOption.Required,
                    transactionOptions: new TransactionOptions() { IsolationLevel = IsolationLevel.ReadUncommitted, Timeout = TimeSpan.Zero },
                    asyncFlowOption: TransactionScopeAsyncFlowOption.Enabled))
                {
                    identityResult = await _userManager.CreateAsync(applicationUser, request.Password).ConfigureAwait(false);

                    if (identityResult.Succeeded)
                    {
                        await _userManager.AddClaimsAsync(applicationUser, new List<Claim>() {
                            new Claim("email", applicationUser.Email)
                        }).ConfigureAwait(false);
                        await _userManager.AddToRolesAsync(applicationUser, new List<string>() { Roles.UserBasic })
                            .ConfigureAwait(false); // Define user roles on registration
                    }
                    scope.Complete();
                }
                return identityResult;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, nameof(RegisterAsync));
                throw;
            }

        }
    }
}
