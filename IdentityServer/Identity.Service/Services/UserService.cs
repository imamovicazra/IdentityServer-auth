using AutoMapper;
using Identity.Database;
using Identity.Model.Constants.Roles;
using Identity.Model.DTOs.Requests;
using Identity.Model.DTOs.Responses;
using Identity.Model.Entities;
using Identity.Model.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using System.Text;
using System.Transactions;
using Identity.Model.Extensions;
using IdentityServer4.Models;
using Org.BouncyCastle.Asn1.Ocsp;
using System;
using Microsoft.AspNetCore.Http;
using Identity.Model.Constants;
using IdentityModel;
using IdentityModel.Client;

namespace Identity.Service.Services
{
    public class UserService:IUserService
    {
        private readonly IConfiguration _config;
        private readonly IMapper _mapper;
        private readonly ILogger<UserService> _logger;
        private readonly AppDbContext _appDbContext;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly HttpRequest _request;
        private readonly IHttpClientFactory _clientFactory;
        IAuthenticationEmailService _authenticationEmailService;
        public UserService(IConfiguration config,IMapper mapper, ILogger<UserService> logger,
            AppDbContext dbContext, UserManager<ApplicationUser> userManager, IHttpContextAccessor httpContextAccessor,
            IAuthenticationEmailService authenticationEmailService, IHttpClientFactory clientFactory)
        {
            _config= config; 
            _mapper= mapper;
            _logger= logger;
            _appDbContext= dbContext;
            _userManager= userManager;
            _request=httpContextAccessor.HttpContext.Request;
            _authenticationEmailService = authenticationEmailService;
            _clientFactory= clientFactory;
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
                //A TransactionScope is used to create a transaction that encompasses multiple operations and ensures that
                //either all the operations within the scope succeed (commit) or all of them fail (rollback).
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
                            .ConfigureAwait(false);

                        //Sending confirmation email                      
                        
                        var token = await _userManager.GenerateEmailConfirmationTokenAsync(applicationUser).ConfigureAwait(false);
                        string url = $"{_request.Scheme}://{_request.Host}/api/Identity/verify?email={applicationUser.Email}&token={token}";

                        Dictionary<string, string> bodyParameters = new() { { nameof(url), url } };

                        _authenticationEmailService.SendAsync(Model.Constants.Email.Type.Verification, applicationUser.Email, bodyParameters);
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
        public async Task<IdentityResult> VerifyEmailAsync(EmailVerificationRequest request)
        {
            try
            {
                ApplicationUser user = await _userManager.FindByEmailAsync(request.Email);
                if (user is null)
                {
                    IdentityError error = new()
                    {
                        Code = StatusCodes.Status404NotFound.ToString(),
                        Description = ErrorDescriptions.UserDoesNotExistWithEmail
                    }; 

                    return IdentityResult.Failed(error);
                }

                var result = await _userManager.ConfirmEmailAsync(user, request.Token).ConfigureAwait(false);

                var claimsResult = await _userManager.AddClaimsAsync(user, new Claim[]{
                    new Claim(JwtClaimTypes.Email, user.Email),
                }).ConfigureAwait(false);

                if (!claimsResult.Succeeded)
                    throw new Exception(claimsResult.Errors.ToString());

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, nameof(VerifyEmailAsync));
                throw ex;
            }
        }

        public async Task<TokenResponse> TokenAsync(Model.DTOs.Requests.TokenRequest request)
        {
            try
            {
                var client = _clientFactory.CreateClient();
                var cache = new DiscoveryCache(_config["AuthApiUrl"]);
                var disco = await cache.GetAsync()
                    .ConfigureAwait(false);
                if (disco.IsError)
                    throw new Exception(disco.Error);

                switch (request.GrantType)
                {
                    case OidcConstants.GrantTypes.Password:
                        var passwordFlow = await client.RequestPasswordTokenAsync(new PasswordTokenRequest()
                        {
                            Address = disco.TokenEndpoint,
                            ClientId = request.ClientId,
                            ClientSecret = request.ClientSecret,
                            Scope = request.Scope,
                            UserName = request.Email,
                            Password = request.Password
                        }).ConfigureAwait(false);

                        return passwordFlow;
                    case OidcConstants.GrantTypes.ClientCredentials:
                        var clientCredentialsFlow = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest()
                        {
                            Address = disco.TokenEndpoint,
                            ClientId = request.ClientId,
                            ClientSecret = request.ClientSecret,
                            Scope = request.Scope,
                        }).ConfigureAwait(false);

                        return clientCredentialsFlow;
                    default:
                        return null;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, nameof(TokenAsync));
                throw;
            }
        }

        public async Task<TokenResponse> RefreshTokenAsync(RefreshTokenRequestDTO request)
        {
            try
            {
                var client = _clientFactory.CreateClient();
                var cache = new DiscoveryCache(_config["AuthApiUrl"]);
                var disco = await cache.GetAsync()
                                       .ConfigureAwait(false);
                if (disco.IsError)
                    throw new Exception(disco.Error);

                var refreshToken = await client.RequestRefreshTokenAsync(new RefreshTokenRequest()
                {
                    Address = disco.TokenEndpoint,
                    ClientId = request.ClientId,
                    ClientSecret = request.ClientSecret,
                    RefreshToken = request.RefreshToken
                }).ConfigureAwait(false);

                return refreshToken;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, nameof(RefreshTokenAsync));
                throw;
            }
        }

        public async Task<TokenRevocationResponse> RevokeTokenAsync(RefreshTokenRequestDTO request)
        {
            try
            {
                var client = _clientFactory.CreateClient();
                var cache = new DiscoveryCache(_config["AuthApiUrl"]);
                var disco = await cache.GetAsync()
                                       .ConfigureAwait(false);

                if (disco.IsError)
                    throw new Exception(disco.Error);

                var revokeResult = await client.RevokeTokenAsync(new TokenRevocationRequest
                {
                    Address = disco.RevocationEndpoint,
                    ClientId = request.ClientId,
                    ClientSecret = request.ClientSecret,
                    Token = request.RefreshToken
                }).ConfigureAwait(false);

                return revokeResult;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, nameof(RevokeTokenAsync));
                throw;
            }
        }

        public async Task<IdentityResult> EditUserAsync(UserUpdateRequest request, string userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId).ConfigureAwait(false);

                var validationResult = user.UserExistsAndConfirmed();

                if (validationResult.Succeeded)
                {
                    user.FirstName = request.FirstName;
                    user.LastName = request.LastName;
                    var result = await _userManager.UpdateAsync(user).ConfigureAwait(false);
                    return result;
                }

                return validationResult;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, nameof(EditUserAsync));
                throw;
            }
        }

        public async Task<IdentityResult> RequestResetPasswordTokenAsync(RequestResetPasswordRequest request)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(request.Email).ConfigureAwait(false);

                var validationResult = user.UserExistsAndConfirmed();

                if (validationResult.Succeeded)
                {
                    var result = await _userManager.UpdateSecurityStampAsync(user).ConfigureAwait(false);

                    if (result.Succeeded)
                    {
                        var token = await _userManager.GeneratePasswordResetTokenAsync(user).ConfigureAwait(false);

                        token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

                        string url = $"{_request.Scheme}://{_request.Host}/api/Identity/password/reset?email={user.Email}&token={token}";
                        Dictionary<string, string> bodyParameters = new()
                        {
                            { nameof(url), url }
                        };

                        _authenticationEmailService.SendAsync(Model.Constants.Email.Type.ForgotPassword, user.Email, bodyParameters);

                        return IdentityResult.Success;
                    }

                    return result;
                }
                return validationResult;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, nameof(RequestResetPasswordTokenAsync));
                throw;
            }
        }

        public async Task<IdentityResult> ResetPasswordAsync(ResetPasswordRequest request)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(request.Email).ConfigureAwait(false);

                var validationResult = user.UserExistsAndConfirmed();

                if (validationResult.Succeeded)
                {
                    var decodedToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(request.Token));

                    var result = await _userManager.ResetPasswordAsync(user, decodedToken, request.Password).ConfigureAwait(false);

                    return result;
                }

                return validationResult;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, nameof(ResetPasswordAsync));
                throw;
            }
        }
    }
}
