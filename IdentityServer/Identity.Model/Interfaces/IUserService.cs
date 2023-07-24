using Identity.Model.DTOs.Requests;
using Identity.Model.DTOs.Responses;
using Microsoft.AspNetCore.Identity;


namespace Identity.Model.Interfaces
{
    public interface IUserService
    {
        Task<IdentityResult> RegisterAsync(ApplicationUserRequest request);
        Task<UserResponse> GetUserAsync(string userId, string email);
    }
}
