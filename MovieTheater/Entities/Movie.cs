using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MovieTheater.Entities
{
    public class Movie : IId
    {
        public int Id { get; set; }
        [Required]
        [StringLength(300)]
        public string Title { get; set; }
        public string Poster { get; set; }
        public bool AtCinema { get; set; }
        public DateTime PremiereDate { get; set; }
        public List<MovieActor> MovieActors { get; set; }
        public List<MovieGender> MovieGenders { get; set; }
    }
}
