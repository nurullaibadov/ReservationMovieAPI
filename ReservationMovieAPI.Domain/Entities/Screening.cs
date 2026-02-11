using ReservationMovieAPI.Domain.Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReservationMovieAPI.Domain.Entities
{
    public class Screening : BaseEntity
    {
        public Guid MovieId { get; set;  }
        public virtual Movie Movie { get; set;  }   
        public Guid HallId { get; set;  }   
        public virtual Hall Hall { get; set; }  
        public DateTime StartTime { get; set;  }    
        public DateTime EndTime { get; set;  }  
        public decimal StandardPrice { get; set;  } 
        public decimal PremiumPrice { get; set;  }  
        public decimal VipPrice { get; set;  }  
        public bool Is3D { get; set;  } 
        public bool IsSubtitled { get; set;  }  
        public bool IsActive { get; set;  } 
        public int AvailableSeats { get; set;  }
        public virtual ICollection<Reservation> Reservations { get; set; } = new HashSet<Reservation>();
    }
}
