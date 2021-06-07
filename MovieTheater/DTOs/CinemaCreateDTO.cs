using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MovieTheater.DTOs
{
    public class CinemaCreateDTO
    {
        [Required]
        [StringLength(120)]
        public string Name { get; set; }
    }
}
