using IdentityServer4.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Model.DTOs.Requests
{
    public class RequestResetPasswordRequest
    {
        [Required]
        public string Email { get; set; }
    }
}
