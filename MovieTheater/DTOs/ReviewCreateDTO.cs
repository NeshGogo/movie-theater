using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MovieTheater.DTOs
{
    public class ReviewCreateDTO
    {
        public string Comment { get; set; }
        [Range(1,5)]
        public int Score { get; set; }
    }
}
