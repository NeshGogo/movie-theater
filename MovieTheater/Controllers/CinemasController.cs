using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using MovieTheater.DTOs;
using MovieTheater.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieTheater.Controllers
{
    [ApiController]
    [Route("api/cinemas")]
    public class CinemasController : CustomBaseController
    {
        public CinemasController(MovieTheaterDbContext context, IMapper mapper)
            : base(context, mapper){}

        [HttpGet]
        public async Task<ActionResult<List<CinemaDTO>>> Get()
        {
            return await Get<Cinema, CinemaDTO>();
        }

        [HttpGet("{id:int}", Name = "GetCinema")]
        public async Task<ActionResult<CinemaDTO>> Get(int id)
        {
            return await Get<Cinema, CinemaDTO>(id);
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] CinemaCreateDTO cinemaCreateDTO)
        {
            return await Post<Cinema, CinemaDTO, CinemaCreateDTO>(cinemaCreateDTO, "GetCinema");
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put(int id, [FromBody] CinemaCreateDTO cinemaCreateDTO)
        {
            return await Put<Cinema, CinemaCreateDTO>(id, cinemaCreateDTO);
        }

        [HttpPatch("{id:int}")]
        public async Task<ActionResult> Patch(int id, [FromBody] JsonPatchDocument<CinemaCreateDTO> patchDocument)
        {
            return await Patch<Cinema, CinemaCreateDTO>(id, patchDocument);
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            return await Delete<Cinema>(id);
        }
    }
}
