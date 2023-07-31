using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Model.DTOs.Responses
{
    public class RefreshTokenResponse
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public int ExpiresIn { get; set; }
        public RefreshTokenResponse(string accesToken, string refreshToken, int expiresIn)
        {
            AccessToken = accesToken;
            RefreshToken = refreshToken;
            ExpiresIn = expiresIn;
        }
        public RefreshTokenResponse() { }
    }
}
