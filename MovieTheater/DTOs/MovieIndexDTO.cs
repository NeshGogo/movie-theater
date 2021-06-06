using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieTheater.DTOs
{
    public class MovieIndexDTO
    {
        public List<MovieDTO> FuturePremiere { get; set; }
        public List<MovieDTO> AtCinema { get; set; }
    }
}
