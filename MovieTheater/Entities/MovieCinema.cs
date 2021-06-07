﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieTheater.Entities
{
    public class MovieCinema
    {
        public int MovieId { get; set; }
        public int CinemaId { get; set; }
        public Movie Movie { get; set; }
        public Cinema Cinema { get; set; }
    }
}
