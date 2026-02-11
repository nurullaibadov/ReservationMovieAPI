using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReservationMovieAPI.Domain.Entities.Common
{
    public abstract class BaseEntity
    {
        public Guid Id { get; set;  }    
        public DateTime CreatedAt { get; set;  }    
        public string? CreatedBy { get; set;  } 
        public DateTime? UpdatedAt { get; set;  }   
        public string? UpdatedBy { get; set;  }     
        public bool IsDeleted { get; set;  }    
        public DateTime? DeletedAt { get; set;  }   
        public string? DeletedBy { get; set;  }
        [Timestamp]
        public byte[]? RowVersion { get; set; }
    }
}
