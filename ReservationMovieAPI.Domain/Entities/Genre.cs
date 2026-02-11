using ReservationMovieAPI.Domain.Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReservationMovieAPI.Domain.Entities
{
    public class Genre : BaseEntity
    {
        public string Name { get; set;  }
        public string? Description { get; set;  }
        public virtual ICollection<Movie> Movies { get; set; } = new HashSet<Movie>();
    }
}
