using EntityFrameworkCore.Triggers;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Model.Entities
{
    public class ApplicationUser : IdentityUser, ITrackable
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime UpdatedAt { get; set; }

        public ApplicationUser()
        {
            Triggers<ApplicationUser>.Updating += entry => entry.Entity.UpdatedAt = DateTime.Now;
        }
    }
}
