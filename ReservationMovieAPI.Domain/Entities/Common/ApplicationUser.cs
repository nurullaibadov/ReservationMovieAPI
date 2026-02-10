using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReservationMovieAPI.Domain.Entities.Common
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }       
        public string LastName { get; set;  }
        public string FullName => $"{FirstName}{LastName}";
        public DateTime?  DateOfBirth { get; set; } 
        public string? ProfilePictureUrl { get; set; }  


    }
}
