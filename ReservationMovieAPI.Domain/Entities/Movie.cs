using ReservationMovieAPI.Domain.Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReservationMovieAPI.Domain.Entities
{
   public class Movie : BaseEntity
    {
         public string Title { get; set;  }   
        public string? LocalTitle { get; set;  }    
        public string Description { get; set;  }    
        public string? LongDescription { get; set;  }   
        public int DurationMinutes { get; set;  }   
        public DateTime ReleaseDate { get; set;  }  
        public int AgeRestriction { get; set;  }    
        public string Director { get; set; }    
        public string CastJson { get; set;  }
        public string Language { get; set;  }
        public string? SubtitleLanguage { get; set;  }
        public string? PosterUrl { get; set; }      
        public string? TrailerUrl { get; set;  }    
        public decimal? ImdbRating { get; set;  }   
        public decimal AverageRating { get; set; }      
        public int TotalRatings { get; set;  }  
        public bool IsNowShowing { get; set;  } 
        public bool IsComingSoon { get; set;  }
        public virtual ICollection<Genre> Genres { get; set; } = new HashSet<Genre>();
        public virtual ICollection<Screening> Screenings { get; set; } = new HashSet<Screening>();

    }
}
 