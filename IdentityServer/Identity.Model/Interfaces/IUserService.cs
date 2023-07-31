using Identity.Model.DTOs.Requests;
using Identity.Model.DTOs.Responses;
using IdentityModel.Client;
using Microsoft.AspNetCore.Identity;


namespace Identity.Model.Interfaces
{
    public interface IUserService
    {
        Task<IdentityResult> RegisterAsync(ApplicationUserRequest request);
        Task<UserResponse> GetUserAsync(string userId, string email);
        Task<IdentityResult> VerifyEmailAsync(EmailVerificationRequest request);
        Task<TokenResponse> TokenAsync(DTOs.Requests.TokenRequest request);
        Task<TokenResponse> RefreshTokenAsync(DTOs.Requests.RefreshTokenRequestDTO request);
        Task<TokenRevocationResponse> RevokeTokenAsync(RefreshTokenRequestDTO request);
    }
}
