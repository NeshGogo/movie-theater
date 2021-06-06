using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieTheater.DTOs
{
    public class MovieDetailDTO : MovieDTO
    {
        public List<GenderDTO> Genders { get; set; }
        public List<MovieActorDetailDTO> Arctors { get; set; }
    }
}
