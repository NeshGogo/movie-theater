using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieTheater.Entities
{
    public class MovieGender
    {
        public int GenderId { get; set; }
        public int MovieId { get; set; }

        public Movie Movie { get; set; }
        public Gender Gender { get; set; }
    }
}
