﻿using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MovieTheater.DTOs
{
    public class ActorCreateDTO
    {
        [Required]
        [StringLength(120)]
        public string Name { get; set; }
        public DateTime DateOfBirth { get; set; }
        public IFormFile PhotoFile { get; set; }
    }
}
