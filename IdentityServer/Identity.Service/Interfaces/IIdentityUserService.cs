using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Service.Interfaces
{
    public interface IIdentityUserService<TUser> where TUser : IdentityUser
    {
        Task<TUser> FindByNameAsync(string userName);
        Task<IList<Claim>> GetClaimsAsync(TUser user);
        Task<IList<string>> GetRolesAsync(TUser user);
        PasswordVerificationResult VerifyHashedPassword(TUser user, string password);
    }
}
