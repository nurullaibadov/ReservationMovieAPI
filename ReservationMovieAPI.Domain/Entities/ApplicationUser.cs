using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReservationMovieAPI.Domain.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set;  }  
        public string LastName { get; set;  }   
        public string FullName => $"{FirstName}{LastName}";
        public DateTime? DateOfBirth { get; set;  }
        public string? ProfilePictureUrl { get; set;  } 
        public string? RefreshToken { get; set;  }  
        public DateTime? RefreshTokenExpiryTime { get; set;  }
        public bool IsActive { get; set;  }
        public DateTime CreatedAt { get; set;  }    
        public DateTime? LastLoginAt { get; set; }
        public virtual ICollection<Reservation> Reservations { get; set; } = new HashSet<Reservation>();
    }
}
