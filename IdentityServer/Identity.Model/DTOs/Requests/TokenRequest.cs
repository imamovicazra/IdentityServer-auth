using IdentityServer4.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Model.DTOs.Requests
{
    public class TokenRequest
    {
        [Required]
        public string GrantType { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public string ClientId { get; set; }

        [Required]
        public string ClientSecret { get; set; }

        [Required]
        public string Scope { get; set; }

        public string RefreshToken { get; set; }
    }

    public class RefreshTokenRequestDTO
    {
        [Required]
        public string ClientId { get; set; }

        [Required]
        public string ClientSecret { get; set; }

        [Required]
        public string RefreshToken { get; set; }
    }
}
