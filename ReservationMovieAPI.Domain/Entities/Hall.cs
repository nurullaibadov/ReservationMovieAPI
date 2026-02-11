using ReservationMovieAPI.Domain.Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReservationMovieAPI.Domain.Entities
{
    public class Hall : BaseEntity
    {
        public Guid CinemaId { get; set;  } 
        public virtual Cinema Cinema { get; set;  } 
        public string Name { get; set;  }   
        public string HallType { get; set;  }   
        public int TotalSeats { get; set;  }    
        public int Rows { get; set;  }  
        public int SeatsPerRow { get; set;  }   
        public bool IsActive { get; set;  } 
        public bool Has3D { get; set;  }    
        public bool HasDolbyAtmos { get; set;  }
        public virtual ICollection<Seat> Seats { get; set; } = new HashSet<Seat>();
        public virtual ICollection<Screening> Screenings { get; set; } = new HashSet<Screening>();

    }
}
