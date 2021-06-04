using Microsoft.AspNetCore.Http;
using MovieTheater.Validations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MovieTheater.DTOs
{
    public class MovieCreateDTO : MoviePatchDTO
    {

        [FileTypeValidation(FileTypeGroup.image)]
        [FileWeightValidation(4)]
        public IFormFile Poster { get; set; }
    }
}
