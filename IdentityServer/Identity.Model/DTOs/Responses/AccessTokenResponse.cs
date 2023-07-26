using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Model.DTOs.Responses
{
    public class AccessTokenResponse
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public int ExpiresIn { get; set; }
        public AccessTokenResponse(string accesToken, string refreshToken, int expiresIn)
        {
            AccessToken = accesToken;
            RefreshToken = refreshToken;
            ExpiresIn = expiresIn;
        }
        public AccessTokenResponse() { }
    }
}
