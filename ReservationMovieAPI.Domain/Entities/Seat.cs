using ReservationMovieAPI.Domain.Entities.Common;
using ReservationMovieAPI.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReservationMovieAPI.Domain.Entities
{
    public class Seat : BaseEntity
    {
        public Guid HallId { get; set;  }   
        public virtual Hall Hall { get; set;  } 
        public string RowLabel { get; set;  }       
        public int SeatNumber { get; set;  }    
        public SeatType SeatType { get; set;  } 
        public bool IsActive { get; set;  } 
        public bool IsAccessible { get; set;  }
        public virtual ICollection<ReservationSeat> ReservationSeats { get; set; } = new HashSet<ReservationSeat>();

    }
}
