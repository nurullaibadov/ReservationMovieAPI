using ReservationMovieAPI.Domain.Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReservationMovieAPI.Domain.Entities
{
    public class Cinema : BaseEntity
    {
        public string Name { get; set; }
        public string? Description { get; set;  }    
        public string Address { get; set;  }    
        public string City { get; set;  }   
        public string? District { get; set;  }  
        public string Country { get; set;  }    
        public string? PostalCode { get; set;  }    
        public double? Latitude { get; set;  }  
        public double? Longitude { get; set;  } 
        public string? PhoneNumber { get; set;  }       
        public string? Email { get; set;  } 
        public string? Website { get; set;  }   
        public TimeSpan OpeningTime { get; set;  }  
        public TimeSpan ClosingTime { get; set;  }  
        public bool IsActive { get; set;  }
        public string? LogoUrl { get; set;  }
        public virtual ICollection<Hall> Halls { get; set; } = new HashSet<Hall>();
    }
}
