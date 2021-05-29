using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieTheater.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieTheater.Controllers
{
    [ApiController]
    [Route("api/genders")]
    public class GendersController : ControllerBase
    {
        private readonly MovieTheaterDbContext context;

        public GendersController(MovieTheaterDbContext context)
        {
            this.context = context;
        }

        [HttpGet]
        public async Task<ActionResult<List<Gender>>> Get()
        {
            var results = await context.Genders.ToListAsync();
            return results;
        }

    }
}
